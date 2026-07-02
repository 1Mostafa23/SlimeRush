using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

public class AddressableSlimeFactory : ISlimeFactory, IDisposable
{
    private readonly SlimePrefabAddress slimePrefabAddress;
    private readonly DiContainer container;

    private AsyncOperationHandle<GameObject> loadHandle;
    private GameObject loadedSlimePrefab;
    private bool hasLoadHandle;

    public AddressableSlimeFactory(SlimePrefabAddress slimePrefabAddress, DiContainer container)
    {
        this.slimePrefabAddress = slimePrefabAddress;
        this.container = container;
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

        GameObject slime = container.InstantiatePrefab(loadedSlimePrefab);

        Transform slimeTransform = slime.transform;
        slimeTransform.SetParent(parent, false);
        slimeTransform.localPosition = Vector3.zero;
        slimeTransform.localRotation = Quaternion.identity;

        slime.SetActive(true);
        return slime;
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
