using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [SerializeField] private CurrencyWalletSettings currencyWalletSettings;

    [Header("Music")]
    [SerializeField] private AudioClip musicClip;
    [SerializeField, Range(0f, 1f)] private float menuMusicVolume = 0.08f;
    [SerializeField, Range(0f, 1f)] private float gameplayMusicVolume = 0.18f;
    [SerializeField] private float musicFadeDuration = 0.5f;

    public override void InstallBindings()
    {
        if (currencyWalletSettings == null)
        {
            Debug.LogError("ProjectInstaller: CurrencyWalletSettings is not assigned.");
            return;
        }

        if (musicClip == null)
            Debug.LogWarning("ProjectInstaller: Music clip is not assigned.");

        Container.BindInstance(currencyWalletSettings).AsSingle();
        Container.Bind<IPlayerProfileStorage>().To<JsonPlayerProfileStorage>().AsSingle();
        Container.Bind<IPlayerProfileService>().To<PlayerProfileService>().AsSingle();
        Container.Bind<ILevelProgressService>().To<LevelProgressService>().AsSingle();
        Container.Bind<ICurrencyWallet>().To<CurrencyWallet>().AsSingle();
        Container.BindInstance(musicClip).AsSingle();
        Container.BindInstance(new MusicSettings(menuMusicVolume, gameplayMusicVolume, musicFadeDuration)).AsSingle();
        Container.BindInterfacesAndSelfTo<MusicService>().AsSingle();
    }
}
