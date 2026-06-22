using System;
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

        return UnityEngine.Object.Instantiate(loadedSlimePrefab, parent);
    }

    public void Dispose()
    {
        if (!hasLoadHandle)
            return;

        Addressables.Release(loadHandle);
        hasLoadHandle = false;
        loadedSlimePrefab = null;
    }
}
