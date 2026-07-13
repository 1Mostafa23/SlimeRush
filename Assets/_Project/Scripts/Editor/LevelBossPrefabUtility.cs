using UnityEditor;
using UnityEngine;

public static class LevelBossPrefabUtility
{
    private const string LevelPrefabPath = "Assets/_Project/Prefabs/LevelChunks/Level_01_FullChunk.prefab";
    private const string BossPrefabPath = "Assets/_Project/Prefabs/boss/Boss_SlimeKing_01.prefab";
    private const string BossName = "Boss_SlimeKing_01";
    private static readonly Vector3 BossLocalPosition = new Vector3(0.30051f, 0f, 189.79216f);

    [MenuItem("SlimeRush/Levels/Add Boss To Level 01 Prefab")]
    public static void AddBossToLevel01Prefab()
    {
        GameObject levelRoot = PrefabUtility.LoadPrefabContents(LevelPrefabPath);

        if (levelRoot == null)
        {
            Debug.LogError($"LevelBossPrefabUtility: Level prefab not found at {LevelPrefabPath}.");
            return;
        }

        try
        {
            Transform existingBoss = levelRoot.transform.Find(BossName);

            if (existingBoss != null)
                Object.DestroyImmediate(existingBoss.gameObject);

            GameObject bossPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(BossPrefabPath);

            if (bossPrefab == null)
            {
                Debug.LogError($"LevelBossPrefabUtility: Boss prefab not found at {BossPrefabPath}.");
                return;
            }

            GameObject bossInstance = (GameObject)PrefabUtility.InstantiatePrefab(bossPrefab, levelRoot.transform);
            bossInstance.name = BossName;

            Transform bossTransform = bossInstance.transform;
            bossTransform.localPosition = BossLocalPosition;
            bossTransform.localRotation = Quaternion.identity;
            bossTransform.localScale = Vector3.one;

            PrefabUtility.SaveAsPrefabAsset(levelRoot, LevelPrefabPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"LevelBossPrefabUtility: Added {BossName} to {LevelPrefabPath}.");
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(levelRoot);
        }
    }
}
