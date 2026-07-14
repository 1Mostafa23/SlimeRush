using TMPro;
using UnityEditor;
using UnityEngine;

public static class DestructibleWallSetupUtility
{
    private const string WallPrefabPath = "Assets/_Project/Prefabs/levelsprefabs/DestructibleWall.prefab";

    [MenuItem("SlimeRush/Bonuses/Ensure Destructible Wall Setup")]
    public static void EnsureDestructibleWallSetup()
    {
        GameObject wallRoot = PrefabUtility.LoadPrefabContents(WallPrefabPath);

        if (wallRoot == null)
        {
            Debug.LogError($"DestructibleWallSetupUtility: Wall prefab not found at {WallPrefabPath}.");
            return;
        }

        try
        {
            DestructibleWallView wallView = wallRoot.GetComponent<DestructibleWallView>();

            if (wallView == null)
                wallView = wallRoot.AddComponent<DestructibleWallView>();

            Collider wallCollider = wallRoot.GetComponentInChildren<Collider>(true);

            if (wallCollider != null)
            {
                wallCollider.isTrigger = true;

                DestructibleWallContactDamageZone contactDamageZone =
                    wallCollider.GetComponent<DestructibleWallContactDamageZone>();

                if (contactDamageZone == null)
                    contactDamageZone = wallCollider.gameObject.AddComponent<DestructibleWallContactDamageZone>();

                SerializedObject zoneSerialized = new SerializedObject(contactDamageZone);
                zoneSerialized.FindProperty("wallView").objectReferenceValue = wallView;
                zoneSerialized.ApplyModifiedPropertiesWithoutUndo();
            }

            TMP_Text hpText = wallRoot.GetComponentInChildren<TMP_Text>(true);

            SerializedObject wallSerialized = new SerializedObject(wallView);
            wallSerialized.FindProperty("maxHp").intValue = 30;
            wallSerialized.FindProperty("hpText").objectReferenceValue = hpText;
            wallSerialized.FindProperty("rootToDisable").objectReferenceValue = wallRoot;
            wallSerialized.FindProperty("damageSlimesOnContact").boolValue = true;
            wallSerialized.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(wallRoot, WallPrefabPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("DestructibleWallSetupUtility: Destructible wall setup completed.");
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(wallRoot);
        }
    }
}
