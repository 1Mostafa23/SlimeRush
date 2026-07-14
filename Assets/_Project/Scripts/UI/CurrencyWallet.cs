using System;

public class CurrencyWallet : ICurrencyWallet
{
    private readonly IPlayerProfileService profileService;

    public int Coins => profileService.Profile.coins;
    public int Gems => profileService.Profile.gems;

    public event Action CoinsChanged;
    public event Action GemsChanged;

    public CurrencyWallet(IPlayerProfileService profileService)
    {
        this.profileService = profileService;
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0)
            return;

        profileService.Profile.coins += amount;
        Save();
        CoinsChanged?.Invoke();
    }

    public bool SpendCoins(int amount)
    {
        if (amount <= 0 || Coins < amount)
            return false;

        profileService.Profile.coins -= amount;
        Save();
        CoinsChanged?.Invoke();
        return true;
    }

    public void SetCoins(int amount)
    {
        profileService.Profile.coins = Math.Max(0, amount);
        Save();
        CoinsChanged?.Invoke();
    }

    public void AddGems(int amount)
    {
        if (amount <= 0)
            return;

        profileService.Profile.gems += amount;
        Save();
        GemsChanged?.Invoke();
    }

    public void SetGems(int amount)
    {
        profileService.Profile.gems = Math.Max(0, amount);
        Save();
        GemsChanged?.Invoke();
    }

    public bool SpendGems(int amount)
    {
        if (amount <= 0 || Gems < amount)
            return false;

        profileService.Profile.gems -= amount;
        Save();
        GemsChanged?.Invoke();
        return true;
    }

    private void Save()
    {
        profileService.Save();
    }
}
