using System;
using UnityEngine;

public class CurrencyWallet : ICurrencyWallet
{
    public int Coins { get; private set; }
    public int Gems { get; private set; }

    public event Action CoinsChanged;
    public event Action GemsChanged;

    public CurrencyWallet(int startingCoins, int startingGems)
    {
        Coins = Mathf.Max(0, startingCoins);
        Gems = Mathf.Max(0, startingGems);
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0)
            return;

        Coins += amount;
        CoinsChanged?.Invoke();
    }

    public bool SpendCoins(int amount)
    {
        if (amount <= 0 || Coins < amount)
            return false;

        Coins -= amount;
        CoinsChanged?.Invoke();
        return true;
    }

    public void AddGems(int amount)
    {
        if (amount <= 0)
            return;

        Gems += amount;
        GemsChanged?.Invoke();
    }

    public bool SpendGems(int amount)
    {
        if (amount <= 0 || Gems < amount)
            return false;

        Gems -= amount;
        GemsChanged?.Invoke();
        return true;
    }
}
