using Zenject;

public class BossInstaller : Installer<BossProjectile, BossRangedAttackSettings, BossClashSettings, BossInstaller>
{
    private readonly BossProjectile projectilePrefab;
    private readonly BossRangedAttackSettings rangedAttackSettings;
    private readonly BossClashSettings clashSettings;

    public BossInstaller(
        BossProjectile projectilePrefab,
        BossRangedAttackSettings rangedAttackSettings,
        BossClashSettings clashSettings)
    {
        this.projectilePrefab = projectilePrefab;
        this.rangedAttackSettings = rangedAttackSettings;
        this.clashSettings = clashSettings;
    }

    private static void InstallInternal(
        DiContainer container,
        BossProjectile projectilePrefab,
        BossRangedAttackSettings rangedAttackSettings,
        BossClashSettings clashSettings)
    {
        if (!container.HasBinding<BossRangedAttackSettings>())
            container.BindInstance(rangedAttackSettings).AsSingle();

        if (!container.HasBinding<BossClashSettings>())
            container.BindInstance(clashSettings).AsSingle();

        if (!container.HasBinding<BossProjectile.Pool>())
        {
            container.BindMemoryPool<BossProjectile, BossProjectile.Pool>()
                .WithInitialSize(rangedAttackSettings.ProjectilePoolInitialSize)
                .FromComponentInNewPrefab(projectilePrefab)
                .UnderTransformGroup("BossProjectilePool");
        }

        if (!container.HasBinding<BossCombatant>())
            container.Bind<BossCombatant>().AsSingle().WithArguments(50);

        if (!container.HasBinding<IBossRuntimeContext>())
            container.Bind<IBossRuntimeContext>().To<BossRuntimeContext>().AsSingle();

        if (!container.HasBinding<BossStateContext>())
            container.Bind<BossStateContext>().AsSingle();

        if (!container.HasBinding<BossIdleState>())
            container.Bind<BossIdleState>().AsSingle();

        if (!container.HasBinding<BossRangedState>())
            container.Bind<BossRangedState>().AsSingle();

        if (!container.HasBinding<BossClashState>())
            container.Bind<BossClashState>().AsSingle();

        if (!container.HasBinding<BossDefeatedState>())
            container.Bind<BossDefeatedState>().AsSingle();

        if (!container.HasBinding<IBossStateMachine>())
            container.BindInterfacesAndSelfTo<BossStateMachine>().AsSingle();

        if (!container.HasBinding<IBossFightService>())
            container.BindInterfacesAndSelfTo<BossFightService>().AsSingle();
    }

    public override void InstallBindings()
    {
        InstallInternal(Container, projectilePrefab, rangedAttackSettings, clashSettings);
    }
}
