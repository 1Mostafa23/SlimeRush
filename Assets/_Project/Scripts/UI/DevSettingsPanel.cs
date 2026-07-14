using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DevSettingsPanel : IDevSettingsPanel
{
    private readonly ICurrencyWallet wallet;
    private readonly ILevelProgressService levelProgressService;
    private readonly IPlayerUpgradeService upgradeService;
    private readonly ILevelConfigProvider levelConfigProvider;
    private readonly ILevelGenerator levelGenerator;

    private GameObject root;
    private TMP_Text statusText;

    public DevSettingsPanel(
        ICurrencyWallet wallet,
        ILevelProgressService levelProgressService,
        IPlayerUpgradeService upgradeService,
        [Inject(Optional = true)] ILevelConfigProvider levelConfigProvider,
        [Inject(Optional = true)] ILevelGenerator levelGenerator)
    {
        this.wallet = wallet;
        this.levelProgressService = levelProgressService;
        this.upgradeService = upgradeService;
        this.levelConfigProvider = levelConfigProvider;
        this.levelGenerator = levelGenerator;
    }

    public void Toggle(Transform parent)
    {
        if (parent == null)
            return;

        if (root == null)
            Build(parent);

        root.SetActive(!root.activeSelf);
        Refresh();
    }

    public void Hide()
    {
        if (root != null)
            root.SetActive(false);
    }

    private void Build(Transform parent)
    {
        root = CreateRect("DevSettingsPanel", parent, new Color(0f, 0f, 0f, 0.72f));
        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;
        root.AddComponent<Button>().transition = Selectable.Transition.None;

        GameObject frame = CreateRect("Frame", root.transform, new Color(0.08f, 0.09f, 0.12f, 0.96f));
        RectTransform frameRect = frame.GetComponent<RectTransform>();
        frameRect.anchorMin = new Vector2(0.5f, 0.5f);
        frameRect.anchorMax = new Vector2(0.5f, 0.5f);
        frameRect.pivot = new Vector2(0.5f, 0.5f);
        frameRect.sizeDelta = new Vector2(760f, 720f);

        VerticalLayoutGroup layout = frame.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(32, 32, 28, 28);
        layout.spacing = 14f;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;

        AddText(frame.transform, "DEV SETTINGS", 44f, FontStyles.Bold);
        statusText = AddText(frame.transform, string.Empty, 28f, FontStyles.Normal);

        AddButton(frame.transform, "+100 000 COINS", () => wallet.AddCoins(100000));
        AddButton(frame.transform, "+1 000 GEMS", () => wallet.AddGems(1000));
        AddButton(frame.transform, "LEVEL -1", () => SetLevel(levelProgressService.CurrentLevel - 1));
        AddButton(frame.transform, "LEVEL +1", () => SetLevel(levelProgressService.CurrentLevel + 1));
        AddButton(frame.transform, "SLIME UPGRADE -1", () => upgradeService.SetSlimeCountUpgradeLevel(upgradeService.SlimeCountUpgradeLevel - 1));
        AddButton(frame.transform, "SLIME UPGRADE +1", () => upgradeService.SetSlimeCountUpgradeLevel(upgradeService.SlimeCountUpgradeLevel + 1));
        AddButton(frame.transform, "CLOSE", Hide);

        root.SetActive(false);
    }

    private void SetLevel(int level)
    {
        int maxLevel = levelConfigProvider?.MaxAvailableLevel ?? int.MaxValue;
        levelProgressService.SetCurrentLevel(Mathf.Clamp(level, 1, maxLevel));
        levelGenerator?.GenerateCurrentLevel();
    }

    private void Refresh()
    {
        if (statusText == null)
            return;

        statusText.text =
            $"Coins: {wallet.Coins:N0}   Gems: {wallet.Gems:N0}\n" +
            $"Level: {levelProgressService.CurrentLevel}   Slime Upgrade: {upgradeService.SlimeCountUpgradeLevel} (+{upgradeService.SlimeCountBonus})";
    }

    private GameObject CreateRect(string name, Transform parent, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = color;
        return go;
    }

    private TMP_Text AddText(Transform parent, string text, float fontSize, FontStyles fontStyle)
    {
        GameObject go = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);

        TMP_Text label = go.GetComponent<TMP_Text>();
        label.text = text;
        label.fontSize = fontSize;
        label.fontStyle = fontStyle;
        label.alignment = TextAlignmentOptions.Center;
        label.color = Color.white;

        LayoutElement layout = go.AddComponent<LayoutElement>();
        layout.preferredHeight = fontSize * 1.6f;

        return label;
    }

    private void AddButton(Transform parent, string label, UnityEngine.Events.UnityAction action)
    {
        GameObject go = CreateRect(label, parent, new Color(0.18f, 0.55f, 0.95f, 1f));

        Button button = go.AddComponent<Button>();
        button.onClick.AddListener(() =>
        {
            action.Invoke();
            Refresh();
        });

        LayoutElement layout = go.AddComponent<LayoutElement>();
        layout.preferredHeight = 72f;

        TMP_Text text = AddText(go.transform, label, 28f, FontStyles.Bold);
        RectTransform textRect = text.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
    }
}
