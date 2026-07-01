using System;
using System.Collections.Generic;
using UnityEngine;

public class SlimePool : ISlimePool, IDisposable
{
    private readonly ISlimeFactory slimeFactory;
    private readonly Queue<GameObject> inactiveObjects = new();
    private readonly HashSet<GameObject> activeObjects = new();
    private readonly HashSet<GameObject> allObjects = new();

    public SlimePool(ISlimeFactory slimeFactory)
    {
        this.slimeFactory = slimeFactory;
    }

    public int ActiveCount => activeObjects.Count;
    public int InactiveCount => inactiveObjects.Count;
    public int TotalCount => allObjects.Count;

    public void Prewarm(int count, Transform parent)
    {
        if (count <= 0)
            return;

        for (int i = 0; i < count; i++)
        {
            GameObject slime = slimeFactory.Create(parent);
            ResetSlime(slime, parent);
            slime.SetActive(false);

            inactiveObjects.Enqueue(slime);
            allObjects.Add(slime);
        }
    }

    public GameObject Rent(Transform parent)
    {
        GameObject slime = inactiveObjects.Count > 0
            ? inactiveObjects.Dequeue()
            : slimeFactory.Create(parent);

        allObjects.Add(slime);
        activeObjects.Add(slime);

        ResetSlime(slime, parent);
        slime.SetActive(true);

        return slime;
    }

    public void Return(GameObject slime)
    {
        if (slime == null)
            return;

        if (!activeObjects.Remove(slime))
        {
            Debug.LogWarning("SlimePool: Tried to return a slime that is not active.");
            return;
        }

        slime.SetActive(false);
        inactiveObjects.Enqueue(slime);
    }

    public void Clear()
    {
        foreach (GameObject slime in allObjects)
        {
            if (slime != null)
                UnityEngine.Object.Destroy(slime);
        }

        activeObjects.Clear();
        inactiveObjects.Clear();
        allObjects.Clear();
    }

    public void Dispose()
    {
        Clear();
    }

    private static void ResetSlime(GameObject slime, Transform parent)
    {
        Transform slimeTransform = slime.transform;
        slimeTransform.SetParent(parent, false);
        slimeTransform.localPosition = Vector3.zero;
        slimeTransform.localRotation = Quaternion.identity;
        slimeTransform.localScale = Vector3.one;
    }
}
