using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class RapidFireBonusSetupUtility
{
    private const string ScenePath = "Assets/GameplayPrototype.unity";
    private const string PlayerCrowdPrefabPath = "Assets/_Project/Prefabs/Player/PlayerCrowd.prefab";
    private const string RapidFirePickupPrefabPath = "Assets/_Project/Prefabs/bonuses/RapidFireBonusPickup Variant.prefab";
    private const string RapidFireProjectilePrefabPath = "Assets/_Project/Prefabs/Projectiles/RapidFireProjectile_Proto.prefab";

    [MenuItem("SlimeRush/Bonuses/Ensure Rapid Fire Bonus Setup")]
    public static void EnsureRapidFireBonusSetup()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(RapidFireProjectilePrefabPath));

        RapidFireProjectile projectilePrefab = EnsureProjectilePrefab();
        EnsurePlayerShooter();
        EnsurePickupPrefab();
        AssignProjectileToGameplayInstaller(projectilePrefab);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("RapidFireBonusSetupUtility: Rapid fire bonus setup completed.");
    }

    private static RapidFireProjectile EnsureProjectilePrefab()
    {
        RapidFireProjectile existingProjectile = AssetDatabase.LoadAssetAtPath<RapidFireProjectile>(RapidFireProjectilePrefabPath);

        if (existingProjectile != null)
            return existingProjectile;

        GameObject projectileObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projectileObject.name = "RapidFireProjectile_Proto";
        projectileObject.transform.localScale = Vector3.one * 0.28f;

        SphereCollider sphereCollider = projectileObject.GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;

        Rigidbody rigidbody = projectileObject.AddComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;

        RapidFireProjectile projectile = projectileObject.AddComponent<RapidFireProjectile>();
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(projectileObject, RapidFireProjectilePrefabPath);
        Object.DestroyImmediate(projectileObject);

        return prefab.GetComponent<RapidFireProjectile>();
    }

    private static void EnsurePlayerShooter()
    {
        GameObject playerRoot = PrefabUtility.LoadPrefabContents(PlayerCrowdPrefabPath);

        try
        {
            RapidFireShooterView shooterView = playerRoot.GetComponent<RapidFireShooterView>();

            if (shooterView == null)
                shooterView = playerRoot.AddComponent<RapidFireShooterView>();

            Transform firePointsRoot = FindOrCreateChild(playerRoot.transform, "RapidFireFirePoints");
            Transform left = FindOrCreateChild(firePointsRoot, "FirePoint_Left");
            Transform center = FindOrCreateChild(firePointsRoot, "FirePoint_Center");
            Transform right = FindOrCreateChild(firePointsRoot, "FirePoint_Right");

            left.localPosition = new Vector3(-0.7f, 0.8f, 1.1f);
            center.localPosition = new Vector3(0f, 0.8f, 1.15f);
            right.localPosition = new Vector3(0.7f, 0.8f, 1.1f);

            left.localRotation = Quaternion.identity;
            center.localRotation = Quaternion.identity;
            right.localRotation = Quaternion.identity;

            SerializedObject shooterSerialized = new SerializedObject(shooterView);
            shooterSerialized.FindProperty("firePointLeft").objectReferenceValue = left;
            shooterSerialized.FindProperty("firePointCenter").objectReferenceValue = center;
            shooterSerialized.FindProperty("firePointRight").objectReferenceValue = right;
            shooterSerialized.FindProperty("fireInterval").floatValue = 0.1f;
            shooterSerialized.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(playerRoot, PlayerCrowdPrefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(playerRoot);
        }
    }

    private static void EnsurePickupPrefab()
    {
        GameObject pickupRoot = PrefabUtility.LoadPrefabContents(RapidFirePickupPrefabPath);

        try
        {
            RapidFireBonusPickupView pickupView = pickupRoot.GetComponent<RapidFireBonusPickupView>();

            if (pickupView == null)
                pickupView = pickupRoot.AddComponent<RapidFireBonusPickupView>();

            SerializedObject pickupSerialized = new SerializedObject(pickupView);
            pickupSerialized.FindProperty("duration").floatValue = 2f;
            pickupSerialized.ApplyModifiedPropertiesWithoutUndo();

            Collider pickupCollider = pickupRoot.GetComponent<Collider>();

            if (pickupCollider == null)
                pickupCollider = pickupRoot.AddComponent<BoxCollider>();

            pickupCollider.isTrigger = true;

            PrefabUtility.SaveAsPrefabAsset(pickupRoot, RapidFirePickupPrefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(pickupRoot);
        }
    }

    private static void AssignProjectileToGameplayInstaller(RapidFireProjectile projectilePrefab)
    {
        EditorSceneManager.OpenScene(ScenePath);

        GameplayInstaller installer = Object.FindFirstObjectByType<GameplayInstaller>();

        if (installer == null)
        {
            Debug.LogWarning("RapidFireBonusSetupUtility: GameplayInstaller was not found in scene.");
            return;
        }

        SerializedObject installerSerialized = new SerializedObject(installer);
        installerSerialized.FindProperty("rapidFireProjectilePrefab").objectReferenceValue = projectilePrefab;
        installerSerialized.FindProperty("rapidFireProjectilePoolInitialSize").intValue = 24;
        installerSerialized.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(installer.gameObject.scene);
        EditorSceneManager.SaveScene(installer.gameObject.scene);
    }

    private static Transform FindOrCreateChild(Transform parent, string childName)
    {
        Transform child = parent.Find(childName);

        if (child != null)
            return child;

        GameObject childObject = new GameObject(childName);
        child = childObject.transform;
        child.SetParent(parent, false);
        return child;
    }
}
