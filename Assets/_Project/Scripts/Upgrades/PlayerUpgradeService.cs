using System;

public class PlayerUpgradeService : IPlayerUpgradeService
{
    private readonly IPlayerProfileService profileService;
    private readonly ICurrencyWallet currencyWallet;
    private readonly SlimeCountUpgradeSettings settings;

    public int SlimeCountUpgradeLevel => profileService.Profile.slimeCountUpgradeLevel;
    public int SlimesPerUpgrade => settings.SlimesPerUpgrade;
    public int SlimeCountBonus => SlimeCountUpgradeLevel * settings.SlimesPerUpgrade;
    public int NextSlimeCountUpgradeCost => settings.GetCost(SlimeCountUpgradeLevel);
    public bool IsSlimeCountUpgradeMaxed => SlimeCountUpgradeLevel >= settings.MaxUpgradeLevel;

    public event Action Changed;

    public PlayerUpgradeService(
        IPlayerProfileService profileService,
        ICurrencyWallet currencyWallet,
        SlimeCountUpgradeSettings settings)
    {
        this.profileService = profileService;
        this.currencyWallet = currencyWallet;
        this.settings = settings;
    }

    public int GetStartingSlimeCount(int baseSlimeCount)
    {
        return Math.Max(1, baseSlimeCount + SlimeCountBonus);
    }

    public bool TryBuySlimeCountUpgrade()
    {
        if (IsSlimeCountUpgradeMaxed)
            return false;

        int cost = NextSlimeCountUpgradeCost;

        if (!currencyWallet.SpendCoins(cost))
            return false;

        profileService.Profile.slimeCountUpgradeLevel++;
        profileService.Save();
        Changed?.Invoke();
        return true;
    }

    public void SetSlimeCountUpgradeLevel(int level)
    {
        profileService.Profile.slimeCountUpgradeLevel = Math.Clamp(level, 0, settings.MaxUpgradeLevel);
        profileService.Save();
        Changed?.Invoke();
    }
}
