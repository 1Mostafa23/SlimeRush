using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SlimeCountUpgradeButtonPresenter : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text labelText;
    [SerializeField] private CanvasGroup visualGroup;
    [SerializeField, Range(0.1f, 1f)] private float disabledAlpha = 0.45f;

    private IPlayerUpgradeService upgradeService;
    private ICurrencyWallet currencyWallet;
    private bool isSubscribed;

    [Inject]
    private void Construct(
        IPlayerUpgradeService upgradeService,
        ICurrencyWallet currencyWallet)
    {
        this.upgradeService = upgradeService;
        this.currencyWallet = currencyWallet;
    }

    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (labelText == null)
            labelText = GetComponentInChildren<TMP_Text>(true);

        if (visualGroup == null)
            visualGroup = GetComponent<CanvasGroup>();

        if (visualGroup == null)
            visualGroup = gameObject.AddComponent<CanvasGroup>();

        if (disabledAlpha <= 0f)
            disabledAlpha = 0.45f;

        ConfigureLabel();

        if (button != null)
            button.onClick.AddListener(BuyUpgrade);
    }

    private void OnEnable()
    {
        Subscribe();
        Refresh();
    }

    private void Start()
    {
        Subscribe();
        Refresh();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(BuyUpgrade);
    }

    private void BuyUpgrade()
    {
        if (upgradeService == null)
            return;

        bool bought = upgradeService.TryBuySlimeCountUpgrade();

        if (!bought)
            Debug.Log("Not enough coins for Slime Count upgrade.");

        Refresh();
    }

    private void Refresh()
    {
        if (upgradeService == null || currencyWallet == null)
            return;

        if (upgradeService.IsSlimeCountUpgradeMaxed)
        {
            SetLabel($"MAX +{upgradeService.SlimeCountBonus}");
            SetInteractable(false);
            return;
        }

        int cost = upgradeService.NextSlimeCountUpgradeCost;
        SetLabel($"+{upgradeService.SlimesPerUpgrade} SLIME - {cost}");
        SetInteractable(currencyWallet.Coins >= cost);
    }

    private void ConfigureLabel()
    {
        if (labelText == null)
            return;

        labelText.enableAutoSizing = true;
        labelText.fontSizeMin = 18f;
        labelText.fontSizeMax = 38f;
        labelText.alignment = TextAlignmentOptions.Center;
        labelText.textWrappingMode = TextWrappingModes.NoWrap;
        labelText.overflowMode = TextOverflowModes.Ellipsis;
        labelText.lineSpacing = 0f;
        labelText.margin = Vector4.zero;
    }

    private void Subscribe()
    {
        if (isSubscribed)
            return;

        if (upgradeService == null || currencyWallet == null)
            return;

        if (upgradeService != null)
            upgradeService.Changed += Refresh;

        if (currencyWallet != null)
            currencyWallet.CoinsChanged += Refresh;

        isSubscribed = true;
    }

    private void Unsubscribe()
    {
        if (!isSubscribed)
            return;

        if (upgradeService != null)
            upgradeService.Changed -= Refresh;

        if (currencyWallet != null)
            currencyWallet.CoinsChanged -= Refresh;

        isSubscribed = false;
    }

    private void SetLabel(string value)
    {
        if (labelText != null)
            labelText.text = value;
    }

    private void SetInteractable(bool value)
    {
        if (button != null)
            button.interactable = value;

        if (visualGroup != null)
            visualGroup.alpha = value ? 1f : disabledAlpha;
    }
}
