using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BossProjectile : MonoBehaviour
{
    private SlimeDamageApplier slimeDamageApplier;
    private BossRangedAttackSettings settings;
    private Pool pool;
    private readonly HashSet<GameObject> hitSlimes = new HashSet<GameObject>();
    private Vector3 direction = Vector3.back;
    private float lifeTimer;
    private float traveledDistance;
    private bool isDespawned;

    [Inject]
    private void Construct(SlimeDamageApplier slimeDamageApplier, BossRangedAttackSettings settings)
    {
        this.slimeDamageApplier = slimeDamageApplier;
        this.settings = settings;
    }

    public void Launch(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.01f)
            this.direction = direction.normalized;

        lifeTimer = 0f;
        traveledDistance = 0f;
        hitSlimes.Clear();
        isDespawned = false;
    }

    private void SetPool(Pool pool)
    {
        this.pool = pool;
    }

    private void Update()
    {
        Vector3 movement = direction * settings.ProjectileSpeed * Time.deltaTime;
        transform.position += movement;

        traveledDistance += movement.magnitude;
        lifeTimer += Time.deltaTime;

        if (lifeTimer >= settings.ProjectileLifetime || traveledDistance >= settings.ProjectileMaxDistance)
            Despawn();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out SlimeHitbox slimeHitbox))
            slimeHitbox = other.GetComponentInParent<SlimeHitbox>();

        if (slimeHitbox == null)
            return;

        GameObject slimeObject = slimeHitbox.SlimeObject;
        if (slimeObject == null || hitSlimes.Contains(slimeObject))
            return;

        hitSlimes.Add(slimeObject);
        slimeDamageApplier.TryKillSlime(slimeHitbox);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<BossDetectionZone>() == null)
            return;

        Despawn();
    }

    private void Despawn()
    {
        if (isDespawned)
            return;

        isDespawned = true;

        if (pool != null)
            pool.Despawn(this);
    }

    public class Pool : MonoMemoryPool<Vector3, Quaternion, Vector3, BossProjectile>
    {
        protected override void Reinitialize(
            Vector3 position,
            Quaternion rotation,
            Vector3 direction,
            BossProjectile projectile)
        {
            projectile.SetPool(this);
            projectile.transform.SetPositionAndRotation(position, rotation);
            projectile.Launch(direction);
        }
    }
}
