using System;
using System.Collections.Generic;
using UnityEngine;

public class SlimeCrowdManager : MonoBehaviour
{
    [Header("Slime Prefab")]
    [SerializeField] private GameObject slimePrefab;

    [Header("Crowd Settings")]
    [SerializeField] private int startingSlimeCount = 20;
    [SerializeField] private float baseSpacing = 1.0f;
    [SerializeField] private float ellipseWidth = 1.15f;
    [SerializeField] private float ellipseDepth = 0.9f;
    [SerializeField] private float frontOffsetZ = -0.35f;

    [Header("Organic Look")]
    [SerializeField] private float randomOffsetAmount = 0.05f;

    private readonly List<GameObject> slimes = new();

    public int SlimeCount => slimes.Count;

    public event Action<int> OnSlimeCountChanged;

    private void Start()
    {
        CreateStartingCrowd();
    }

    private void CreateStartingCrowd()
    {
        ClearCrowd();
        AddSlimes(startingSlimeCount);
    }

    public void AddSlimes(int amount)
    {
        if (amount <= 0)
            return;

        for (int i = 0; i < amount; i++)
        {
            GameObject slime = Instantiate(slimePrefab, transform);
            slimes.Add(slime);
        }

        RearrangeCrowd();
        NotifySlimeCountChanged();
    }

    public void RemoveSlimes(int amount)
    {
        if (amount <= 0)
            return;

        int removeCount = Mathf.Min(amount, slimes.Count);

        for (int i = 0; i < removeCount; i++)
        {
            int lastIndex = slimes.Count - 1;
            GameObject slimeToRemove = slimes[lastIndex];

            slimes.RemoveAt(lastIndex);
            Destroy(slimeToRemove);
        }

        RearrangeCrowd();
        NotifySlimeCountChanged();
    }

    public void MultiplySlimes(int multiplier)
    {
        if (multiplier <= 1)
            return;

        int currentCount = SlimeCount;
        int amountToAdd = currentCount * (multiplier - 1);

        AddSlimes(amountToAdd);
    }

    private void RearrangeCrowd()
    {
        List<Vector3> positions = GenerateCrowdPositions(slimes.Count);

        for (int i = 0; i < slimes.Count; i++)
        {
            Transform slimeTransform = slimes[i].transform;
            slimeTransform.localPosition = positions[i];
            slimeTransform.localRotation = Quaternion.identity;
        }
    }

    private List<Vector3> GenerateCrowdPositions(int count)
    {
        List<Vector3> positions = new();

        if (count <= 0)
            return positions;

        positions.Add(new Vector3(0f, 0f, frontOffsetZ));

        if (count == 1)
            return positions;

        int placed = 1;
        int ring = 1;

        while (placed < count)
        {
            float radiusX = ring * baseSpacing * ellipseWidth;
            float radiusZ = ring * baseSpacing * ellipseDepth;

            float circumferenceEstimate =
                2f * Mathf.PI * Mathf.Sqrt((radiusX * radiusX + radiusZ * radiusZ) * 0.5f);

            int ringCapacity = Mathf.Max(6, Mathf.RoundToInt(circumferenceEstimate / baseSpacing));
            int placeThisRing = Mathf.Min(ringCapacity, count - placed);

            float angleOffset = ring % 2 == 0 ? 0f : Mathf.PI / placeThisRing;

            for (int i = 0; i < placeThisRing; i++)
            {
                float angle = ((float)i / placeThisRing) * Mathf.PI * 2f + angleOffset;

                float x = Mathf.Cos(angle) * radiusX;
                float z = Mathf.Sin(angle) * radiusZ + frontOffsetZ;

                Vector2 jitter = GetStableJitter(placed + i) * randomOffsetAmount;

                positions.Add(new Vector3(
                    x + jitter.x,
                    0f,
                    z + jitter.y
                ));
            }

            placed += placeThisRing;
            ring++;
        }

        ShiftFormationForward(positions);
        return positions;
    }

    private Vector2 GetStableJitter(int index)
    {
        float x = Mathf.PerlinNoise(index * 0.173f, 0.371f) * 2f - 1f;
        float y = Mathf.PerlinNoise(index * 0.521f, 0.137f) * 2f - 1f;
        return new Vector2(x, y);
    }

    private void ShiftFormationForward(List<Vector3> positions)
    {
        if (positions.Count == 0)
            return;

        float maxZ = float.MinValue;

        for (int i = 0; i < positions.Count; i++)
        {
            if (positions[i].z > maxZ)
                maxZ = positions[i].z;
        }

        float targetFrontZ = 0.2f;
        float shift = targetFrontZ - maxZ;

        for (int i = 0; i < positions.Count; i++)
        {
            Vector3 pos = positions[i];
            pos.z += shift;
            positions[i] = pos;
        }
    }

    private void ClearCrowd()
    {
        for (int i = slimes.Count - 1; i >= 0; i--)
        {
            if (slimes[i] != null)
                Destroy(slimes[i]);
        }

        slimes.Clear();
        NotifySlimeCountChanged();
    }

    private void NotifySlimeCountChanged()
    {
        OnSlimeCountChanged?.Invoke(SlimeCount);
    }
}