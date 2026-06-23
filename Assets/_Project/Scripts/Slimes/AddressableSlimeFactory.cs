using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableSlimeFactory : ISlimeFactory, IDisposable
{
    private readonly SlimePrefabAddress slimePrefabAddress;

    private AsyncOperationHandle<GameObject> loadHandle;
    private GameObject loadedSlimePrefab;
    private bool hasLoadHandle;
    private readonly Stack<GameObject> inactiveSlimes = new();

    public AddressableSlimeFactory(SlimePrefabAddress slimePrefabAddress)
    {
        this.slimePrefabAddress = slimePrefabAddress;
    }

    public async UniTask InitializeAsync(CancellationToken cancellationToken)
    {
        if (loadedSlimePrefab != null)
            return;

        loadHandle = Addressables.LoadAssetAsync<GameObject>(slimePrefabAddress.Value);
        hasLoadHandle = true;

        await loadHandle.Task;

        cancellationToken.ThrowIfCancellationRequested();

        if (loadHandle.Status != AsyncOperationStatus.Succeeded || loadHandle.Result == null)
            throw new InvalidOperationException($"Failed to load slime prefab address '{slimePrefabAddress.Value}'.");

        loadedSlimePrefab = loadHandle.Result;
    }

    public GameObject Create(Transform parent)
    {
        if (loadedSlimePrefab == null)
            throw new InvalidOperationException("Slime prefab is not loaded. Call InitializeAsync before Create.");

        GameObject slime = inactiveSlimes.Count > 0
            ? inactiveSlimes.Pop()
            : UnityEngine.Object.Instantiate(loadedSlimePrefab);

        Transform slimeTransform = slime.transform;
        slimeTransform.SetParent(parent, false);
        slimeTransform.localPosition = Vector3.zero;
        slimeTransform.localRotation = Quaternion.identity;
        slimeTransform.localScale = Vector3.one;

        slime.SetActive(true);
        return slime;
    }

    public void Release(GameObject slime)
    {
        if (slime == null)
            return;

        Transform slimeTransform = slime.transform;
        slimeTransform.SetParent(null, false);
        slimeTransform.localPosition = Vector3.zero;
        slimeTransform.localRotation = Quaternion.identity;
        slimeTransform.localScale = Vector3.one;

        slime.SetActive(false);
        inactiveSlimes.Push(slime);
    }

    public void Dispose()
    {
        while (inactiveSlimes.Count > 0)
        {
            GameObject slime = inactiveSlimes.Pop();

            if (slime != null)
                UnityEngine.Object.Destroy(slime);
        }

        if (!hasLoadHandle)
            return;

        Addressables.Release(loadHandle);
        hasLoadHandle = false;
        loadedSlimePrefab = null;
    }
}
