using UnityEngine;
using Zenject;

public class GameplayInstaller : MonoInstaller
{
    [Header("Scene References")]
    [SerializeField] private SlimeCrowdManager slimeCrowdManager;
    [SerializeField] private CrowdFormationSettings crowdFormationSettings;

    [Header("Addressables")]
    [SerializeField] private string slimePrefabAddress = "SlimePrefab";

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

        if (string.IsNullOrWhiteSpace(slimePrefabAddress))
        {
            Debug.LogError("GameplayInstaller: Slime prefab address is not assigned.");
            return;
        }

        Container.BindInstance(new SlimePrefabAddress(slimePrefabAddress)).AsSingle();
        Container.Bind<ISlimeFactory>().To<AddressableSlimeFactory>().AsSingle();
        Container.Bind<ISlimePool>().To<SlimePool>().AsSingle();
        Container.BindInstance(crowdFormationSettings).AsSingle();
        Container.Bind<ICrowdRowAllocator>().To<CountMastersCrowdRowAllocator>().AsSingle();
        Container.Bind<ICrowdFormation>().To<CountMastersCrowdFormation>().AsSingle();
        Container.Bind<SlimeCrowdManager>().FromInstance(slimeCrowdManager).AsSingle();
        Container.Bind<ISlimeCrowd>().FromInstance(slimeCrowdManager).AsSingle();
        Container.Bind<ISlimeCrowdCommands>().FromInstance(slimeCrowdManager).AsSingle();
        Container.Bind<ISlimeCrowdDamageCommands>().FromInstance(slimeCrowdManager).AsSingle();
        Container.Bind<SlimeDamageApplier>().AsSingle();
        Container.Bind<AddGateOperation>().AsSingle();
        Container.Bind<MultiplyGateOperation>().AsSingle();
        Container.Bind<SubtractGateOperation>().AsSingle();
        Container.Bind<GateOperationResolver>().AsSingle();
        Container.Bind<CrowdCountChangeApplier>().AsSingle();
        Container.Bind<IGateEffectApplier>().To<GateEffectApplier>().AsSingle();
    }
}
