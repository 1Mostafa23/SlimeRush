using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

public class AddressableEnemyFactory : IEnemyFactory, IDisposable
{
    private readonly DiContainer container;
    private readonly Dictionary<string, AsyncOperationHandle<GameObject>> loadHandles = new();
    private readonly Dictionary<string, GameObject> loadedPrefabs = new();

    public AddressableEnemyFactory(DiContainer container)
    {
        this.container = container;
    }

    public async UniTask<GameObject> CreateAsync(
        EnemyAddress enemyAddress,
        Vector3 position,
        Quaternion rotation,
        Transform parent,
        CancellationToken cancellationToken)
    {
        GameObject prefab = await LoadPrefabAsync(enemyAddress.Value, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();

        GameObject enemy = container.InstantiatePrefab(prefab, position, rotation, parent);
        enemy.SetActive(true);
        return enemy;
    }

    public void Dispose()
    {
        foreach (AsyncOperationHandle<GameObject> handle in loadHandles.Values)
            Addressables.Release(handle);

        loadHandles.Clear();
        loadedPrefabs.Clear();
    }

    private async UniTask<GameObject> LoadPrefabAsync(string address, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Enemy address is empty.", nameof(address));

        if (loadedPrefabs.TryGetValue(address, out GameObject loadedPrefab))
            return loadedPrefab;

        AsyncOperationHandle<GameObject> loadHandle = Addressables.LoadAssetAsync<GameObject>(address);
        loadHandles[address] = loadHandle;

        await loadHandle.Task;
        cancellationToken.ThrowIfCancellationRequested();

        if (loadHandle.Status != AsyncOperationStatus.Succeeded || loadHandle.Result == null)
        {
            loadHandles.Remove(address);
            Addressables.Release(loadHandle);
            throw new InvalidOperationException($"Failed to load enemy prefab address '{address}'.");
        }

        loadedPrefabs[address] = loadHandle.Result;
        return loadHandle.Result;
    }
}
