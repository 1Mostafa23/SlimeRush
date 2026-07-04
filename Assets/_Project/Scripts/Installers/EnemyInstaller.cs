using Zenject;

public class EnemyInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Install(Container);
    }

    public static void Install(DiContainer container)
    {
        if (!container.HasBinding<PlayerCrowdController>())
            container.Bind<PlayerCrowdController>().FromComponentInHierarchy().AsSingle();

        if (!container.HasBinding<IPlayerCrowdSpeedProvider>())
            container.Bind<IPlayerCrowdSpeedProvider>().To<PlayerCrowdController>().FromResolve();

        if (!container.HasBinding<AILaneJumperEnemySystem>())
            container.BindInterfacesAndSelfTo<AILaneJumperEnemySystem>().AsSingle();

        if (!container.HasBinding<ILaneTargetProvider>())
            container.Bind<ILaneTargetProvider>().To<PlayerLaneTargetProvider>().AsSingle();

        if (!container.HasBinding<IEnemyLaneSelector>())
            container.Bind<IEnemyLaneSelector>().To<NearestPlayerLaneSelector>().AsSingle();

        if (!container.HasBinding<IEnemyDashSpeedProvider>())
            container.Bind<IEnemyDashSpeedProvider>().To<AdaptiveEnemyDashSpeedProvider>().AsSingle();

        if (!container.HasBinding<IEnemyDefeatHandler>())
            container.Bind<IEnemyDefeatHandler>().To<DisableEnemyDefeatHandler>().AsSingle();

        if (!container.HasBinding<IPlayerPassedEnemyCondition>())
            container.Bind<IPlayerPassedEnemyCondition>().To<PlayerPassedEnemyCondition>().AsSingle();

        if (!container.HasBinding<ICameraImpactService>())
            container.Bind<ICameraImpactService>().To<CameraImpactService>().AsSingle();

        if (!container.HasBinding<IEnemyClashService>())
            container.Bind<IEnemyClashService>().To<EnemyClashService>().AsSingle();

        if (!container.HasBinding<IEnemyClashTargetDetector>())
            container.Bind<IEnemyClashTargetDetector>().To<SlimeCrowdClashTargetDetector>().AsSingle();
    }
}
