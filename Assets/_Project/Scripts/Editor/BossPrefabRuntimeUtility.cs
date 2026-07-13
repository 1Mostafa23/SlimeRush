using UnityEditor;
using UnityEngine;

public static class BossPrefabRuntimeUtility
{
    private const string BossPrefabPath = "Assets/_Project/Prefabs/boss/Boss_SlimeKing_01.prefab";

    [MenuItem("SlimeRush/Boss/Ensure Runtime Binder On Boss Prefab")]
    public static void EnsureRuntimeBinderOnBossPrefab()
    {
        GameObject bossRoot = PrefabUtility.LoadPrefabContents(BossPrefabPath);

        if (bossRoot == null)
        {
            Debug.LogError($"BossPrefabRuntimeUtility: Boss prefab not found at {BossPrefabPath}.");
            return;
        }

        try
        {
            BossRuntimeBinder binder = bossRoot.GetComponent<BossRuntimeBinder>();

            if (binder == null)
                binder = bossRoot.AddComponent<BossRuntimeBinder>();

            SerializedObject binderSerialized = new SerializedObject(binder);
            binderSerialized.FindProperty("rangedAttackController").objectReferenceValue =
                bossRoot.GetComponentInChildren<BossRangedAttackController>(true);
            binderSerialized.FindProperty("cameraController").objectReferenceValue =
                bossRoot.GetComponentInChildren<BossCameraController>(true);
            binderSerialized.FindProperty("hitFeedback").objectReferenceValue =
                bossRoot.GetComponentInChildren<BossHitFeedback>(true);
            binderSerialized.FindProperty("defeatView").objectReferenceValue =
                bossRoot.GetComponentInChildren<BossDefeatView>(true);
            binderSerialized.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(bossRoot, BossPrefabPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"BossPrefabRuntimeUtility: Runtime binder is configured on {BossPrefabPath}.");
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(bossRoot);
        }
    }
}
