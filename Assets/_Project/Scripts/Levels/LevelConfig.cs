using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "LevelConfig",
    menuName = "SlimeRush/Levels/Level Config")]
public class LevelConfig : ScriptableObject
{
    [SerializeField] private int levelNumber = 1;
    [SerializeField] private int randomSeed = 1000;

    [Header("Chunks")]
    [SerializeField] private LevelChunkView startChunk;
    [SerializeField] private List<LevelChunkEntry> middleChunks = new();
    [SerializeField] private int middleChunkCount = 5;
    [SerializeField] private LevelChunkView finishChunk;

    [Header("Boss")]
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private bool spawnBossPrefab;
    [SerializeField] private Vector3 bossSpawnOffset;

    public int LevelNumber => Mathf.Max(1, levelNumber);
    public int RandomSeed => randomSeed;
    public LevelChunkView StartChunk => startChunk;
    public IReadOnlyList<LevelChunkEntry> MiddleChunks => middleChunks;
    public int MiddleChunkCount => Mathf.Max(0, middleChunkCount);
    public LevelChunkView FinishChunk => finishChunk;
    public GameObject BossPrefab => bossPrefab;
    public bool SpawnBossPrefab => spawnBossPrefab;
    public Vector3 BossSpawnOffset => bossSpawnOffset;
}
