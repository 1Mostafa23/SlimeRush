using UnityEngine;
using Zenject;

public class GameplayInstaller : MonoInstaller
{
    [Header("Scene References")]
    [SerializeField] private SlimeCrowdManager slimeCrowdManager;
    [SerializeField] private SlimeCrowdSettings slimeCrowdSettings;
    [SerializeField] private CrowdFormationSettings crowdFormationSettings;
    [SerializeField] private CurrencyWalletSettings currencyWalletSettings;

    [Header("Addressables")]
    [SerializeField] private string slimePrefabAddress = "SlimePrefab";

    [Header("Boss")]
    [SerializeField] private BossProjectile bossProjectilePrefab;
    [SerializeField] private BossRangedAttackSettings bossRangedAttackSettings;

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

        if (currencyWalletSettings == null)
        {
            Debug.LogError("GameplayInstaller: CurrencyWalletSettings is not assigned.");
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

        Container.BindInstance(new SlimePrefabAddress(slimePrefabAddress)).AsSingle();
        Container.Bind<ICurrencyWallet>().To<CurrencyWallet>().AsSingle().WithArguments(
            currencyWalletSettings.StartingCoins,
            currencyWalletSettings.StartingGems);
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
        Container.Bind<ShieldInactiveState>().AsSingle();
        Container.Bind<ShieldActiveState>().AsSingle();
        Container.Bind<ShieldConsumedState>().AsSingle();
        Container.Bind<IShieldStateMachine>().To<ShieldStateMachine>().AsSingle();
        Container.Bind<IShieldService>().To<ShieldService>().AsSingle();
        Container.Bind<IDamageBlocker>().To<ShieldDamageBlocker>().AsSingle();
        Container.Bind<SlimeDamageApplier>().AsSingle();
        Container.Bind<AddGateOperation>().AsSingle();
        Container.Bind<MultiplyGateOperation>().AsSingle();
        Container.Bind<SubtractGateOperation>().AsSingle();
        Container.Bind<GateOperationResolver>().AsSingle();
        Container.Bind<CrowdCountChangeApplier>().AsSingle();
        Container.Bind<IGateEffectApplier>().To<GateEffectApplier>().AsSingle();
        BossInstaller.Install(Container, bossProjectilePrefab, bossRangedAttackSettings);
    }
}
