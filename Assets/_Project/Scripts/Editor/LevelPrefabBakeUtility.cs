using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class LevelPrefabBakeUtility
{
    private const string ScenePath = "Assets/GameplayPrototype.unity";
    private const string LevelObjectName = "Level";
    private const string LevelPrefabPath = "Assets/_Project/Prefabs/LevelChunks/Level_01_FullChunk.prefab";
    private const string LevelConfigPath = "Assets/_Project/ScriptableObjects/Levels/DefaultLevel01.asset";

    [MenuItem("SlimeRush/Levels/Bake Scene Level 01 Prefab")]
    public static void BakeSceneLevel01Prefab()
    {
        EditorSceneManager.OpenScene(ScenePath);

        GameObject levelObject = GameObject.Find(LevelObjectName);

        if (levelObject == null)
        {
            Debug.LogError($"LevelPrefabBakeUtility: Scene object '{LevelObjectName}' was not found.");
            return;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(LevelPrefabPath));

        LevelChunkView levelChunkView = levelObject.GetComponent<LevelChunkView>();

        if (levelChunkView == null)
            levelChunkView = levelObject.AddComponent<LevelChunkView>();

        SerializedObject chunkViewSerialized = new SerializedObject(levelChunkView);
        chunkViewSerialized.FindProperty("length").floatValue = 190f;
        chunkViewSerialized.ApplyModifiedPropertiesWithoutUndo();

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(levelObject, LevelPrefabPath);

        if (prefab == null)
        {
            Debug.LogError("LevelPrefabBakeUtility: Failed to save level prefab.");
            return;
        }

        LevelChunkView prefabChunkView = prefab.GetComponent<LevelChunkView>();
        LevelConfig levelConfig = AssetDatabase.LoadAssetAtPath<LevelConfig>(LevelConfigPath);

        if (levelConfig == null)
        {
            Debug.LogError($"LevelPrefabBakeUtility: LevelConfig not found at {LevelConfigPath}.");
            return;
        }

        SerializedObject configSerialized = new SerializedObject(levelConfig);
        configSerialized.FindProperty("startChunk").objectReferenceValue = prefabChunkView;
        configSerialized.FindProperty("middleChunks").arraySize = 0;
        configSerialized.FindProperty("middleChunkCount").intValue = 0;
        configSerialized.FindProperty("finishChunk").objectReferenceValue = null;
        configSerialized.ApplyModifiedPropertiesWithoutUndo();

        levelObject.SetActive(false);
        SetGameplayInstallerGenerationEnabled();

        EditorSceneManager.MarkSceneDirty(levelObject.scene);
        EditorSceneManager.SaveScene(levelObject.scene);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"LevelPrefabBakeUtility: Saved {LevelPrefabPath} and assigned it to DefaultLevel01.");
    }

    private static void SetGameplayInstallerGenerationEnabled()
    {
        GameplayInstaller installer = Object.FindFirstObjectByType<GameplayInstaller>();

        if (installer == null)
        {
            Debug.LogWarning("LevelPrefabBakeUtility: GameplayInstaller was not found.");
            return;
        }

        SerializedObject installerSerialized = new SerializedObject(installer);
        installerSerialized.FindProperty("generateLevelOnStart").boolValue = true;
        installerSerialized.FindProperty("clearGeneratedRootBeforeGeneration").boolValue = true;
        installerSerialized.ApplyModifiedPropertiesWithoutUndo();
    }
}
