using System.Globalization;
using TMPro;
using UnityEngine;
using Zenject;

public class CurrencyPresenter : MonoBehaviour
{
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private TMP_Text gemsText;

    private ICurrencyWallet wallet;

    [Inject]
    private void Construct(ICurrencyWallet wallet)
    {
        this.wallet = wallet;
    }

    private void Start()
    {
        if (wallet == null)
        {
            Debug.LogError("CurrencyPresenter: Currency wallet is not injected.");
            return;
        }

        wallet.CoinsChanged += RefreshCoins;
        wallet.GemsChanged += RefreshGems;
        Refresh();
    }

    private void OnDestroy()
    {
        if (wallet == null)
            return;

        wallet.CoinsChanged -= RefreshCoins;
        wallet.GemsChanged -= RefreshGems;
    }

    private void Refresh()
    {
        RefreshCoins();
        RefreshGems();
    }

    private void RefreshCoins()
    {
        if (coinsText != null)
            coinsText.text = wallet.Coins.ToString("N0", CultureInfo.InvariantCulture);
    }

    private void RefreshGems()
    {
        if (gemsText != null)
            gemsText.text = wallet.Gems.ToString("N0", CultureInfo.InvariantCulture);
    }
}
