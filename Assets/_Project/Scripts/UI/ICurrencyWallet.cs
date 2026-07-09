using System;

public interface ICurrencyWallet
{
    int Coins { get; }
    int Gems { get; }

    event Action CoinsChanged;
    event Action GemsChanged;

    void AddCoins(int amount);
    bool SpendCoins(int amount);
    void AddGems(int amount);
    bool SpendGems(int amount);
}
