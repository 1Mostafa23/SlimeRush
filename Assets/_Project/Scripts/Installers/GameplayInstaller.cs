using UnityEngine;
using Zenject;

public class GameplayInstaller : MonoInstaller
{
    [Header("Scene References")]
    [SerializeField] private SlimeCrowdManager slimeCrowdManager;
    [SerializeField] private CrowdFormationSettings crowdFormationSettings;

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

        Container.BindInstance(crowdFormationSettings).AsSingle();
        Container.Bind<ICrowdFormation>().To<EllipseCrowdFormation>().AsSingle();
        Container.Bind<SlimeCrowdManager>().FromInstance(slimeCrowdManager).AsSingle();
        Container.Bind<ISlimeCrowd>().FromInstance(slimeCrowdManager).AsSingle();
        Container.Bind<ISlimeCrowdCommands>().FromInstance(slimeCrowdManager).AsSingle();
    }
}
