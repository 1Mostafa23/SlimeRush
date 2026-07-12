using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [SerializeField] private CurrencyWalletSettings currencyWalletSettings;

    public override void InstallBindings()
    {
        if (currencyWalletSettings == null)
        {
            Debug.LogError("ProjectInstaller: CurrencyWalletSettings is not assigned.");
            return;
        }

        Container.BindInstance(currencyWalletSettings).AsSingle();
        Container.Bind<IPlayerProfileStorage>().To<JsonPlayerProfileStorage>().AsSingle();
        Container.Bind<IPlayerProfileService>().To<PlayerProfileService>().AsSingle();
        Container.Bind<ICurrencyWallet>().To<CurrencyWallet>().AsSingle();
    }
}
