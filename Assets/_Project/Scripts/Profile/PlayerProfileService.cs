using UnityEngine;

public class PlayerProfileService : IPlayerProfileService
{
    private readonly IPlayerProfileStorage profileStorage;

    public PlayerProfileData Profile { get; }

    public PlayerProfileService(
        IPlayerProfileStorage profileStorage,
        CurrencyWalletSettings currencyWalletSettings)
    {
        this.profileStorage = profileStorage;

        if (profileStorage.TryLoad(out PlayerProfileData loadedProfile))
        {
            Profile = Sanitize(loadedProfile);
            return;
        }

        Profile = new PlayerProfileData
        {
            coins = currencyWalletSettings.StartingCoins,
            gems = currencyWalletSettings.StartingGems,
            currentLevel = 1
        };

        Save();
    }

    public void Save()
    {
        profileStorage.Save(Profile);
    }

    private static PlayerProfileData Sanitize(PlayerProfileData profileData)
    {
        profileData.coins = Mathf.Max(0, profileData.coins);
        profileData.gems = Mathf.Max(0, profileData.gems);
        profileData.currentLevel = Mathf.Max(1, profileData.currentLevel);
        return profileData;
    }
}
