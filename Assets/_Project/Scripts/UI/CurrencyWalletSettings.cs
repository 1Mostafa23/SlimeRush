using UnityEngine;

[CreateAssetMenu(
    fileName = "CurrencyWalletSettings",
    menuName = "SlimeRush/Economy/Currency Wallet Settings")]
public class CurrencyWalletSettings : ScriptableObject
{
    [SerializeField] private int startingCoins = 12580;
    [SerializeField] private int startingGems = 860;

    public int StartingCoins => startingCoins;
    public int StartingGems => startingGems;
}
