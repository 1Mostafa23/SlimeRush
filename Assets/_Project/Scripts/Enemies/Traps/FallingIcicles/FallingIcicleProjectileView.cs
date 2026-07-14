using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class FallingIcicleProjectileView : MonoBehaviour
{
    private readonly Collider[] overlapResults = new Collider[32];
    private readonly HashSet<SlimeHitbox> damagedSlimes = new();

    private SlimeDamageApplier slimeDamageApplier;
    private FallingIcicleTrapSettings settings;
    private Pool pool;
    private Vector3 baseScale;
    private bool isInitialized;
    private bool isDespawned;

    [Inject]
    private void Construct(SlimeDamageApplier slimeDamageApplier)
    {
        this.slimeDamageApplier = slimeDamageApplier;
    }

    private void Awake()
    {
        baseScale = transform.localScale;
    }

    public void Initialize(
        FallingIcicleTrapSettings settings,
        Vector3 spawnPosition)
    {
        this.settings = settings;
        transform.position = spawnPosition;
        transform.localScale = baseScale * settings.IcicleScaleMultiplier;
        damagedSlimes.Clear();
        isInitialized = true;
        isDespawned = false;
    }

    private void SetPool(Pool pool)
    {
        this.pool = pool;
    }

    private void Update()
    {
        if (!isInitialized || settings == null)
            return;

        transform.position += Vector3.down * settings.FallSpeed * Time.deltaTime;

        if (transform.position.y <= settings.ImpactY)
            Impact();
    }

    private void Impact()
    {
        if (slimeDamageApplier != null)
            DamageSlimesInArea();

        Despawn();
    }

    private void Despawn()
    {
        if (isDespawned)
            return;

        isDespawned = true;
        isInitialized = false;

        if (pool != null)
            pool.Despawn(this);
        else
            gameObject.SetActive(false);
    }

    private void DamageSlimesInArea()
    {
        Vector3 center = new(transform.position.x, settings.ImpactY + settings.ImpactDamageHeight * 0.5f, transform.position.z);
        Vector3 halfExtents = new(
            settings.ImpactRadius,
            settings.ImpactDamageHeight * 0.5f,
            settings.ImpactRadius + settings.ImpactForwardPadding);
        int hitCount = Physics.OverlapBoxNonAlloc(
            center,
            halfExtents,
            overlapResults,
            Quaternion.identity,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Collide);

        damagedSlimes.Clear();

        for (int i = 0; i < hitCount; i++)
        {
            Collider hitCollider = overlapResults[i];
            if (hitCollider == null)
                continue;

            if (!hitCollider.TryGetComponent(out SlimeHitbox slimeHitbox))
                slimeHitbox = hitCollider.GetComponentInParent<SlimeHitbox>();

            if (slimeHitbox == null || !damagedSlimes.Add(slimeHitbox))
                continue;

            slimeDamageApplier.TryKillSlime(slimeHitbox);
        }
    }

    public class Pool : MonoMemoryPool<Vector3, Quaternion, FallingIcicleTrapSettings, FallingIcicleProjectileView>
    {
        protected override void Reinitialize(
            Vector3 position,
            Quaternion rotation,
            FallingIcicleTrapSettings settings,
            FallingIcicleProjectileView projectile)
        {
            projectile.SetPool(this);
            projectile.transform.SetPositionAndRotation(position, rotation);
            projectile.Initialize(settings, position);
        }
    }
}
