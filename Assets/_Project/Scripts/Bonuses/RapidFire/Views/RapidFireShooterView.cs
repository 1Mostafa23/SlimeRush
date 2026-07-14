using System.Collections;
using UnityEngine;
using Zenject;

public class RapidFireShooterView : MonoBehaviour
{
    [SerializeField] private Transform firePointLeft;
    [SerializeField] private Transform firePointCenter;
    [SerializeField] private Transform firePointRight;
    [SerializeField] private float fireInterval = 0.1f;

    private IRapidFireService rapidFireService;
    private RapidFireProjectile.Pool projectilePool;
    private Coroutine shootRoutine;

    [Inject]
    private void Construct(
        IRapidFireService rapidFireService,
        [Inject(Optional = true)] RapidFireProjectile.Pool projectilePool)
    {
        this.rapidFireService = rapidFireService;
        this.projectilePool = projectilePool;
    }

    private void OnEnable()
    {
        if (rapidFireService == null)
            return;

        rapidFireService.Activated += StartShooting;
        rapidFireService.Expired += StopShooting;
        rapidFireService.Paused += StopShooting;
        rapidFireService.Resumed += ResumeShooting;

        if (rapidFireService.IsActive && !rapidFireService.IsPaused)
            StartShooting();
    }

    private void OnDisable()
    {
        if (rapidFireService != null)
        {
            rapidFireService.Activated -= StartShooting;
            rapidFireService.Expired -= StopShooting;
            rapidFireService.Paused -= StopShooting;
            rapidFireService.Resumed -= ResumeShooting;
        }

        StopShooting();
    }

    private void StartShooting()
    {
        if (rapidFireService != null && rapidFireService.IsPaused)
            return;

        if (shootRoutine != null || projectilePool == null || !HasFirePoint())
            return;

        shootRoutine = StartCoroutine(ShootRoutine());
    }

    private void StopShooting()
    {
        if (shootRoutine == null)
            return;

        StopCoroutine(shootRoutine);
        shootRoutine = null;
    }

    private void ResumeShooting()
    {
        if (rapidFireService != null && rapidFireService.IsActive)
            StartShooting();
    }

    private IEnumerator ShootRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(fireInterval);

        while (true)
        {
            Shoot(firePointLeft);
            Shoot(firePointCenter);
            Shoot(firePointRight);
            yield return wait;
        }
    }

    private void Shoot(Transform firePoint)
    {
        if (firePoint == null)
            return;

        Vector3 direction = ResolveShootDirection(firePoint);
        projectilePool.Spawn(firePoint.position, firePoint.rotation, direction);
    }

    private bool HasFirePoint()
    {
        return firePointLeft != null || firePointCenter != null || firePointRight != null;
    }

    private Vector3 ResolveShootDirection(Transform firePoint)
    {
        Vector3 direction = firePoint.forward;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
            return direction.normalized;

        return Vector3.forward;
    }
}
