using UnityEngine;
using Zenject;

public class RapidFireProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 40f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float maxDistance = 90f;
    [SerializeField] private int damage = 1;
    [SerializeField] private bool despawnOnHit = true;

    private Pool pool;
    private Vector3 direction = Vector3.forward;
    private float lifeTimer;
    private float traveledDistance;
    private bool isDespawned;

    public void Launch(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.01f)
            this.direction = direction.normalized;

        lifeTimer = 0f;
        traveledDistance = 0f;
        isDespawned = false;
    }

    private void SetPool(Pool pool)
    {
        this.pool = pool;
    }

    private void Update()
    {
        Vector3 movement = direction * speed * Time.deltaTime;
        transform.position += movement;

        traveledDistance += movement.magnitude;
        lifeTimer += Time.deltaTime;

        if (lifeTimer >= lifetime || traveledDistance >= maxDistance)
            Despawn();
    }

    private void OnTriggerEnter(Collider other)
    {
        IDestructibleTarget destructibleTarget = other.GetComponentInParent<IDestructibleTarget>();

        if (destructibleTarget == null || destructibleTarget.IsDestroyed)
            return;

        destructibleTarget.TakeDamage(damage);

        if (despawnOnHit)
            Despawn();
    }

    private void Despawn()
    {
        if (isDespawned)
            return;

        isDespawned = true;

        if (pool != null)
            pool.Despawn(this);
        else
            gameObject.SetActive(false);
    }

    public class Pool : MonoMemoryPool<Vector3, Quaternion, Vector3, RapidFireProjectile>
    {
        protected override void Reinitialize(
            Vector3 position,
            Quaternion rotation,
            Vector3 direction,
            RapidFireProjectile projectile)
        {
            projectile.SetPool(this);
            projectile.transform.SetPositionAndRotation(position, rotation);
            projectile.Launch(direction);
        }
    }
}
