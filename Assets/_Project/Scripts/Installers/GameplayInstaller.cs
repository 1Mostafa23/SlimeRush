using UnityEngine;
using Zenject;

public class GameplayInstaller : MonoInstaller
{
    [Header("Scene References")]
    [SerializeField] private SlimeCrowdManager slimeCrowdManager;
    [SerializeField] private SlimeCrowdSettings slimeCrowdSettings;
    [SerializeField] private CrowdFormationSettings crowdFormationSettings;

    [Header("Addressables")]
    [SerializeField] private string slimePrefabAddress = "SlimePrefab";

    [Header("Boss")]
    [SerializeField] private BossProjectile bossProjectilePrefab;
    [SerializeField] private BossRangedAttackSettings bossRangedAttackSettings;
    [SerializeField] private BossClashSettings bossClashSettings;

    [Header("Rapid Fire Bonus")]
    [SerializeField] private RapidFireProjectile rapidFireProjectilePrefab;
    [SerializeField] private int rapidFireProjectilePoolInitialSize = 24;

    [Header("Falling Icicle Trap")]
    [SerializeField] private FallingIcicleProjectileView fallingIcicleProjectilePrefab;
    [SerializeField] private int fallingIcicleProjectilePoolInitialSize = 24;

    [Header("Level Generation")]
    [SerializeField] private LevelConfigLibrary levelConfigLibrary;
    [SerializeField] private Transform generatedLevelRoot;
    [SerializeField] private bool generateLevelOnStart;
    [SerializeField] private bool clearGeneratedRootBeforeGeneration = true;

    public override void InstallBindings()
    {
        if (slimeCrowdManager == null)
        {
            Debug.LogError("GameplayInstaller: SlimeCrowdManager is not assigned.");
            return;
        }

        if (crowdFormationSettings == null)
        {
            Debug.LogError("GameplayInstaller: CrowdFormationSettings is not assigned.");
            return;
        }

        if (slimeCrowdSettings == null)
        {
            Debug.LogError("GameplayInstaller: SlimeCrowdSettings is not assigned.");
            return;
        }

        if (string.IsNullOrWhiteSpace(slimePrefabAddress))
        {
            Debug.LogError("GameplayInstaller: Slime prefab address is not assigned.");
            return;
        }

        if (bossProjectilePrefab == null)
        {
            Debug.LogError("GameplayInstaller: Boss projectile prefab is not assigned.");
            return;
        }

        if (bossRangedAttackSettings == null)
        {
            Debug.LogError("GameplayInstaller: Boss ranged attack settings is not assigned.");
            return;
        }

        if (bossClashSettings == null)
        {
            Debug.LogError("GameplayInstaller: Boss clash settings is not assigned.");
            return;
        }

        Container.BindInstance(new SlimePrefabAddress(slimePrefabAddress)).AsSingle();
        Container.Bind<IGameplayStartService>().To<GameplayStartService>().AsSingle();
        Container.Bind<IRunStatsService>().To<RunStatsService>().AsSingle();
        Container.Bind<IRunRewardCalculator>().To<RunRewardCalculator>().AsSingle();
        Container.Bind<IRunResultService>().To<RunResultService>().AsSingle();
        Container.BindInterfacesAndSelfTo<RunDefeatWatcher>().AsSingle();
        Container.Bind<ISlimeFactory>().To<AddressableSlimeFactory>().AsSingle();
        Container.Bind<ISlimePool>().To<SlimePool>().AsSingle();
        Container.BindInstance(slimeCrowdSettings).AsSingle();
        Container.BindInstance(crowdFormationSettings).AsSingle();
        EnemyInstaller.Install(Container);
        Container.Bind<ICrowdMovementStateMachine>().To<CrowdMovementStateMachine>().AsSingle();
        Container.Bind<CrowdFollowFormationState>().AsSingle();
        Container.Bind<ICrowdRowAllocator>().To<CountMastersCrowdRowAllocator>().AsSingle();
        Container.Bind<ICrowdFormation>().To<CountMastersCrowdFormation>().AsSingle();
        Container.Bind<SlimeCrowdManager>().FromInstance(slimeCrowdManager).AsSingle();
        Container.Bind<ISlimeCrowd>().FromInstance(slimeCrowdManager).AsSingle();
        Container.Bind<ISlimeCrowdCommands>().FromInstance(slimeCrowdManager).AsSingle();
        Container.Bind<ISlimeCrowdDamageCommands>().FromInstance(slimeCrowdManager).AsSingle();
        Container.Bind<IBossCrowdFormationController>().FromInstance(slimeCrowdManager).AsSingle();
        Container.BindInterfacesTo<PlayerUpgradeRuntimeApplier>().AsSingle().NonLazy();
        Container.Bind<ShieldInactiveState>().AsSingle();
        Container.Bind<ShieldActiveState>().AsSingle();
        Container.Bind<ShieldConsumedState>().AsSingle();
        Container.Bind<IShieldStateMachine>().To<ShieldStateMachine>().AsSingle();
        Container.Bind<IShieldService>().To<ShieldService>().AsSingle();
        Container.Bind<IDamageBlocker>().To<ShieldDamageBlocker>().AsSingle();
        InstallRapidFireBonus();
        Container.Bind<SlimeDamageApplier>().AsSingle();
        InstallFallingIcicleTrap();
        Container.Bind<AddGateOperation>().AsSingle();
        Container.Bind<MultiplyGateOperation>().AsSingle();
        Container.Bind<SubtractGateOperation>().AsSingle();
        Container.Bind<GateOperationResolver>().AsSingle();
        Container.Bind<CrowdCountChangeApplier>().AsSingle();
        Container.Bind<IGateEffectApplier>().To<GateEffectApplier>().AsSingle();
        InstallLevelGeneration();
        BossInstaller.Install(Container, bossProjectilePrefab, bossRangedAttackSettings, bossClashSettings);
    }

    private void InstallFallingIcicleTrap()
    {
        if (fallingIcicleProjectilePrefab == null)
            return;

        Container.BindMemoryPool<FallingIcicleProjectileView, FallingIcicleProjectileView.Pool>()
            .WithInitialSize(Mathf.Max(1, fallingIcicleProjectilePoolInitialSize))
            .FromComponentInNewPrefab(fallingIcicleProjectilePrefab)
            .UnderTransformGroup("FallingIcicleProjectilePool");
    }

    private void InstallRapidFireBonus()
    {
        Container.Bind<RapidFireInactiveState>().AsSingle();
        Container.Bind<RapidFireActiveState>().AsSingle();
        Container.Bind<RapidFireExpiredState>().AsSingle();
        Container.BindInterfacesAndSelfTo<RapidFireStateMachine>().AsSingle();
        Container.Bind<IRapidFireService>().To<RapidFireService>().AsSingle();

        if (rapidFireProjectilePrefab == null)
            return;

        Container.BindMemoryPool<RapidFireProjectile, RapidFireProjectile.Pool>()
            .WithInitialSize(Mathf.Max(1, rapidFireProjectilePoolInitialSize))
            .FromComponentInNewPrefab(rapidFireProjectilePrefab)
            .UnderTransformGroup("RapidFireProjectilePool");
    }

    private void InstallLevelGeneration()
    {
        if (levelConfigLibrary == null)
            return;

        Container.BindInstance(levelConfigLibrary).AsSingle();
        Container.BindInstance(new LevelGenerationRuntimeSettings(
            generatedLevelRoot,
            generateLevelOnStart,
            clearGeneratedRootBeforeGeneration)).AsSingle();
        Container.Bind<ILevelConfigProvider>().To<LevelConfigProvider>().AsSingle();
        Container.BindInterfacesAndSelfTo<LevelGeneratorService>().AsSingle();
    }
}
