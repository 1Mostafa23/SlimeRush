using System.Collections;
using UnityEngine;
using Zenject;

public class BossRangedAttackController : MonoBehaviour, IBossRangedAttackController
{
    [SerializeField] private Transform firePointLeft;
    [SerializeField] private Transform firePointCenter;
    [SerializeField] private Transform firePointRight;
    [SerializeField] private Transform visual;
    [SerializeField] private Vector3 shootDirection = Vector3.back;

    private BossProjectile.Pool projectilePool;
    private BossRangedAttackSettings settings;
    private Coroutine shootRoutine;
    private Vector3 initialVisualScale;

    [Inject]
    private void Construct(
        BossProjectile.Pool projectilePool,
        BossRangedAttackSettings settings)
    {
        this.projectilePool = projectilePool;
        this.settings = settings;
    }

    private void Awake()
    {
        if (visual == null)
            visual = transform;

        initialVisualScale = visual.localScale;
    }

    public void StartShooting()
    {
        if (shootRoutine != null || projectilePool == null || !HasFirePoint())
            return;

        shootRoutine = StartCoroutine(ShootRoutine());
    }

    public void StopShooting()
    {
        if (shootRoutine == null)
            return;

        StopCoroutine(shootRoutine);
        shootRoutine = null;

        if (visual != null)
            visual.localScale = initialVisualScale;
    }

    private IEnumerator ShootRoutine()
    {
        while (true)
        {
            yield return PlayPreFireFeedback();
            Shoot(firePointLeft);
            Shoot(firePointRight);
            yield return new WaitForSeconds(settings.FireInterval);

            yield return PlayPreFireFeedback();
            Shoot(firePointCenter);
            yield return new WaitForSeconds(settings.FireInterval);
        }
    }

    private IEnumerator PlayPreFireFeedback()
    {
        if (visual == null)
            yield break;

        visual.localScale = initialVisualScale * settings.PreFireScaleMultiplier;
        yield return new WaitForSeconds(settings.PreFireDuration);
        visual.localScale = initialVisualScale;
    }

    private void Shoot(Transform firePoint)
    {
        if (firePoint == null)
            return;

        Vector3 direction = ResolveShootDirection();
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        projectilePool.Spawn(firePoint.position, rotation, direction);
    }

    private bool HasFirePoint()
    {
        return firePointLeft != null || firePointCenter != null || firePointRight != null;
    }

    private Vector3 ResolveShootDirection()
    {
        Vector3 direction = shootDirection;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
            return direction.normalized;

        return Vector3.back;
    }
}
