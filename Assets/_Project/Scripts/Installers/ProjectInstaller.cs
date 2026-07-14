using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [SerializeField] private CurrencyWalletSettings currencyWalletSettings;
    [SerializeField] private SlimeCountUpgradeSettings slimeCountUpgradeSettings;

    [Header("Music")]
    [SerializeField] private AudioClip musicClip;
    [SerializeField, Range(0f, 1f)] private float menuMusicVolume = 0.08f;
    [SerializeField, Range(0f, 1f)] private float gameplayMusicVolume = 0.18f;
    [SerializeField] private float musicFadeDuration = 0.5f;

    [Header("SFX")]
    [SerializeField] private AudioClip slimeIncreaseClip;
    [SerializeField] private AudioClip slimeCountUpgradeBoughtClip;
    [SerializeField] private AudioClip victoryRewardClip;
    [SerializeField, Range(0f, 1f)] private float slimeIncreaseSfxVolume = 0.055f;
    [SerializeField, Range(0f, 1f)] private float uiSfxVolume = 0.035f;

    public override void InstallBindings()
    {
        if (currencyWalletSettings == null)
        {
            Debug.LogError("ProjectInstaller: CurrencyWalletSettings is not assigned.");
            return;
        }

        if (slimeCountUpgradeSettings == null)
        {
            Debug.LogError("ProjectInstaller: SlimeCountUpgradeSettings is not assigned.");
            return;
        }

        if (musicClip == null)
            Debug.LogWarning("ProjectInstaller: Music clip is not assigned.");

        Container.BindInstance(currencyWalletSettings).AsSingle();
        Container.BindInstance(slimeCountUpgradeSettings).AsSingle();
        Container.Bind<IPlayerProfileStorage>().To<JsonPlayerProfileStorage>().AsSingle();
        Container.Bind<IPlayerProfileService>().To<PlayerProfileService>().AsSingle();
        Container.Bind<ILevelProgressService>().To<LevelProgressService>().AsSingle();
        Container.Bind<ICurrencyWallet>().To<CurrencyWallet>().AsSingle();
        Container.Bind<IPlayerUpgradeService>().To<PlayerUpgradeService>().AsSingle();
        Container.BindInstance(musicClip).AsSingle();
        Container.BindInstance(new MusicSettings(menuMusicVolume, gameplayMusicVolume, musicFadeDuration)).AsSingle();
        Container.BindInterfacesAndSelfTo<MusicService>().AsSingle();
        Container.BindInstance(new SfxSettings(
            slimeIncreaseClip,
            slimeCountUpgradeBoughtClip,
            victoryRewardClip,
            slimeIncreaseSfxVolume,
            uiSfxVolume)).AsSingle();
        Container.BindInterfacesAndSelfTo<SfxService>().AsSingle();
    }
}
