using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Collider))]
public class FallingIcicleTrapView : MonoBehaviour
{
    [SerializeField] private FallingIcicleTrapSettings settings;
    [SerializeField] private GameObject iciclePrefab;
    [SerializeField] private Transform[] leftImpactPoints;
    [SerializeField] private Transform[] rightImpactPoints;
    [SerializeField] private Transform warningsRoot;
    [SerializeField] private Transform projectilesRoot;
    [SerializeField] private bool playOnEnable = true;
    [SerializeField] private bool startWithLeftSide = true;

    private readonly List<FallingIcicleWarningView> activeWarnings = new();

    private SlimeDamageApplier slimeDamageApplier;
    private FallingIcicleProjectileView.Pool projectilePool;
    private Coroutine trapRoutine;

    [Inject]
    private void Construct(
        SlimeDamageApplier slimeDamageApplier,
        [Inject(Optional = true)] FallingIcicleProjectileView.Pool projectilePool)
    {
        this.slimeDamageApplier = slimeDamageApplier;
        this.projectilePool = projectilePool;
    }

    private void Awake()
    {
        Collider triggerCollider = GetComponent<Collider>();
        triggerCollider.isTrigger = true;

        if (warningsRoot == null)
            warningsRoot = transform;

        if (projectilesRoot == null)
            projectilesRoot = transform;
    }

    private void Start()
    {
        if (playOnEnable)
            StartTrap();
    }

    private void StartTrap()
    {
        if (trapRoutine != null)
            return;

        trapRoutine = StartCoroutine(TrapRoutine());
    }

    private IEnumerator TrapRoutine()
    {
        if (settings == null || iciclePrefab == null)
        {
            Debug.LogError("FallingIcicleTrapView: Settings or icicle prefab is not assigned.");
            trapRoutine = null;
            yield break;
        }

        int waveIndex = 0;

        while (true)
        {
            bool useLeftSide = startWithLeftSide
                ? waveIndex % 2 == 0
                : waveIndex % 2 != 0;

            Transform[] impactPoints = useLeftSide ? leftImpactPoints : rightImpactPoints;
            ShowWarnings(impactPoints);

            yield return new WaitForSeconds(settings.WarningDuration);

            SpawnIcicles(impactPoints);
            HideWarnings();

            if (settings.WaveInterval > 0f)
                yield return new WaitForSeconds(settings.WaveInterval);

            waveIndex++;
        }
    }

    private void ShowWarnings(Transform[] impactPoints)
    {
        HideWarnings();

        if (impactPoints == null)
            return;

        for (int i = 0; i < impactPoints.Length; i++)
        {
            Transform point = impactPoints[i];
            if (point == null)
                continue;

            activeWarnings.Add(FallingIcicleWarningView.Create(ResolveImpactPosition(point), settings, warningsRoot));
        }
    }

    private void HideWarnings()
    {
        for (int i = 0; i < activeWarnings.Count; i++)
        {
            if (activeWarnings[i] != null)
                activeWarnings[i].Hide();
        }

        activeWarnings.Clear();
    }

    private void SpawnIcicles(Transform[] impactPoints)
    {
        if (impactPoints == null)
            return;

        for (int i = 0; i < impactPoints.Length; i++)
        {
            Transform point = impactPoints[i];
            if (point == null)
                continue;

            Vector3 spawnPosition = ResolveImpactPosition(point) + Vector3.up * settings.SpawnHeight;
            Quaternion rotation = iciclePrefab != null ? iciclePrefab.transform.rotation : Quaternion.identity;

            if (projectilePool != null)
            {
                projectilePool.Spawn(spawnPosition, rotation, settings);
                continue;
            }

            GameObject icicle = Instantiate(iciclePrefab, spawnPosition, rotation, projectilesRoot);
            FallingIcicleProjectileView projectileView = icicle.GetComponent<FallingIcicleProjectileView>();

            if (projectileView != null)
                projectileView.Initialize(settings, spawnPosition);
        }
    }

    private Vector3 ResolveImpactPosition(Transform point)
    {
        return new Vector3(point.position.x, settings.ImpactY, point.position.z);
    }

    private void OnDisable()
    {
        if (trapRoutine != null)
        {
            StopCoroutine(trapRoutine);
            trapRoutine = null;
        }

        HideWarnings();
    }
}
