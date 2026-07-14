using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LevelGeneratorService : ILevelGenerator, IInitializable
{
    private readonly ILevelConfigProvider levelConfigProvider;
    private readonly LevelGenerationRuntimeSettings runtimeSettings;
    private readonly DiContainer container;

    private Transform generatedRoot;

    public LevelGeneratorService(
        ILevelConfigProvider levelConfigProvider,
        LevelGenerationRuntimeSettings runtimeSettings,
        DiContainer container)
    {
        this.levelConfigProvider = levelConfigProvider;
        this.runtimeSettings = runtimeSettings;
        this.container = container;
    }

    public void Initialize()
    {
        if (runtimeSettings.GenerateOnStart)
            GenerateCurrentLevel();
    }

    public void GenerateCurrentLevel()
    {
        LevelConfig config = levelConfigProvider.GetCurrentConfig();

        if (config == null)
        {
            Debug.LogWarning("LevelGeneratorService: No LevelConfig found for current level.");
            return;
        }

        generatedRoot = ResolveRoot();

        if (runtimeSettings.ClearRootBeforeGeneration)
            ClearRoot();

        System.Random random = new System.Random(config.RandomSeed + levelConfigProvider.CurrentLevel);
        float cursorZ = 0f;

        cursorZ = SpawnChunk(config.StartChunk, cursorZ);

        for (int i = 0; i < config.MiddleChunkCount; i++)
            cursorZ = SpawnChunk(SelectMiddleChunk(config.MiddleChunks, random), cursorZ);

        cursorZ = SpawnChunk(config.FinishChunk, cursorZ);
        SpawnBoss(config, cursorZ);

        Debug.Log($"LevelGeneratorService: Generated level {levelConfigProvider.CurrentLevel}.");
    }

    private Transform ResolveRoot()
    {
        if (runtimeSettings.GeneratedLevelRoot != null)
            return runtimeSettings.GeneratedLevelRoot;

        if (generatedRoot != null)
            return generatedRoot;

        GameObject rootObject = new GameObject("GeneratedLevelRoot");
        return rootObject.transform;
    }

    private void ClearRoot()
    {
        for (int i = generatedRoot.childCount - 1; i >= 0; i--)
            UnityEngine.Object.Destroy(generatedRoot.GetChild(i).gameObject);
    }

    private float SpawnChunk(LevelChunkView chunkPrefab, float cursorZ)
    {
        if (chunkPrefab == null)
            return cursorZ;

        Vector3 position = generatedRoot.position + Vector3.forward * cursorZ;
        GameObject chunkObject = container.InstantiatePrefab(
            chunkPrefab.gameObject,
            position,
            generatedRoot.rotation,
            generatedRoot);

        LevelChunkView spawnedChunk = chunkObject.GetComponent<LevelChunkView>();
        float chunkLength = spawnedChunk != null ? spawnedChunk.Length : chunkPrefab.Length;
        return cursorZ + chunkLength;
    }

    private LevelChunkView SelectMiddleChunk(IReadOnlyList<LevelChunkEntry> chunks, System.Random random)
    {
        int totalWeight = 0;

        for (int i = 0; i < chunks.Count; i++)
        {
            if (chunks[i]?.prefab != null)
                totalWeight += Mathf.Max(1, chunks[i].weight);
        }

        if (totalWeight <= 0)
            return null;

        int roll = random.Next(0, totalWeight);

        for (int i = 0; i < chunks.Count; i++)
        {
            if (chunks[i]?.prefab == null)
                continue;

            roll -= Mathf.Max(1, chunks[i].weight);

            if (roll < 0)
                return chunks[i].prefab;
        }

        return null;
    }

    private void SpawnBoss(LevelConfig config, float cursorZ)
    {
        if (!config.SpawnBossPrefab || config.BossPrefab == null)
            return;

        Vector3 position = generatedRoot.position + Vector3.forward * cursorZ + config.BossSpawnOffset;
        container.InstantiatePrefab(config.BossPrefab, position, generatedRoot.rotation, generatedRoot);
    }
}
