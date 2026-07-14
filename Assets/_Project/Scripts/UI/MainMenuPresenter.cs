using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class MainMenuPresenter : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject darkOverlay;
    [SerializeField] private GameObject tapToStartPanel;
    [SerializeField] private CanvasGroup tapToStartCanvasGroup;

    [Header("Tap To Start Fade")]
    [SerializeField] private float tapFadeMinAlpha = 0.45f;
    [SerializeField] private float tapFadeMaxAlpha = 1f;
    [SerializeField] private float tapFadeSpeed = 1.8f;

    [Header("Gameplay")]
    [SerializeField] private PlayerCrowdController playerCrowdControllerFallback;

    [Header("Level")]
    [SerializeField] private TMP_Text levelText;

    [Header("Buttons")]
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button skinsButton;
    [SerializeField] private Button factoryButton;
    [SerializeField] private Button battlePassButton;
    [SerializeField] private Button coinPlusButton;
    [SerializeField] private Button gemPlusButton;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button bottomOfferPanelButton;

    private IPlayerCrowdMovementController playerMovementController;
    private IMusicService musicService;
    private ILevelProgressService levelProgressService;
    private ILevelConfigProvider levelConfigProvider;
    private IGameplayStartService gameplayStartService;
    private IDevSettingsPanel devSettingsPanel;
    private IPlayerCrowdMovementController PlayerMovementController =>
        playerMovementController ?? playerCrowdControllerFallback;

    [Inject]
    private void Construct(
        IPlayerCrowdMovementController playerMovementController,
        IMusicService musicService,
        ILevelProgressService levelProgressService,
        IGameplayStartService gameplayStartService,
        [Inject(Optional = true)] IDevSettingsPanel devSettingsPanel,
        [Inject(Optional = true)] ILevelConfigProvider levelConfigProvider)
    {
        this.playerMovementController = playerMovementController;
        this.musicService = musicService;
        this.levelProgressService = levelProgressService;
        this.gameplayStartService = gameplayStartService;
        this.devSettingsPanel = devSettingsPanel;
        this.levelConfigProvider = levelConfigProvider;
    }

    private void Awake()
    {
        CacheComponents();
        BindButtons();
        UpdateLevelText();
        ShowMenu();
    }

    private void Start()
    {
        PlayerMovementController?.SetInputEnabled(false);
        musicService?.SetMenuMode();

        if (levelProgressService != null)
            levelProgressService.Changed += UpdateLevelText;
    }

    private void Update()
    {
        if (gameplayStartService != null && gameplayStartService.IsGameplayStarted)
            return;

        UpdateTapToStartFade();

        if (HasStartInputOutsideUi())
            StartGameplay();
    }

    private void OnDestroy()
    {
        UnbindButtons();

        if (levelProgressService != null)
            levelProgressService.Changed -= UpdateLevelText;
    }

    private void StartGameplay()
    {
        if (gameplayStartService != null && gameplayStartService.IsGameplayStarted)
            return;

        gameplayStartService?.StartGameplay();
        devSettingsPanel?.Hide();
        SetMenuVisible(false);
        musicService?.SetGameplayMode();
        PlayerMovementController?.SetInputEnabled(true);
    }

    private void ShowMenu()
    {
        gameplayStartService?.ResetGameplay();
        musicService?.SetMenuMode();
        SetMenuVisible(true);
    }

    private void SetMenuVisible(bool visible)
    {
        if (mainMenuCanvas != null)
            mainMenuCanvas.SetActive(visible);

        if (darkOverlay != null)
            darkOverlay.SetActive(visible);

        if (tapToStartPanel != null)
            tapToStartPanel.SetActive(visible);

        if (tapToStartCanvasGroup != null)
            tapToStartCanvasGroup.alpha = tapFadeMaxAlpha;
    }

    private void UpdateLevelText()
    {
        if (levelText != null)
            levelText.text = $"Level {levelConfigProvider?.CurrentLevel ?? levelProgressService?.CurrentLevel ?? 1}";
    }

    private void CacheComponents()
    {
        if (tapToStartCanvasGroup == null && tapToStartPanel != null)
            tapToStartCanvasGroup = tapToStartPanel.GetComponent<CanvasGroup>();
    }

    private void UpdateTapToStartFade()
    {
        if (tapToStartCanvasGroup == null || !tapToStartCanvasGroup.gameObject.activeInHierarchy)
            return;

        float normalizedAlpha = (Mathf.Sin(Time.unscaledTime * tapFadeSpeed) + 1f) * 0.5f;
        tapToStartCanvasGroup.alpha = Mathf.Lerp(tapFadeMinAlpha, tapFadeMaxAlpha, normalizedAlpha);
    }

    private bool HasStartInputOutsideUi()
    {
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                if (touch.phase == TouchPhase.Began && !IsPointerOverStartBlockingUi(touch.fingerId, touch.position))
                    return true;
            }
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0) && !IsPointerOverStartBlockingUi(-1, Input.mousePosition))
            return true;
#endif

        return false;
    }

    private bool IsPointerOverStartBlockingUi(int pointerId, Vector2 screenPosition)
    {
        if (EventSystem.current == null)
            return false;

        bool isPointerOverUi = pointerId >= 0
            ? EventSystem.current.IsPointerOverGameObject(pointerId)
            : EventSystem.current.IsPointerOverGameObject();

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = screenPosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        if (!isPointerOverUi && results.Count == 0)
            return false;

        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].gameObject.GetComponentInParent<Button>() != null)
                return true;
        }

        return false;
    }

    private void BindButtons()
    {
        AddClick(settingsButton, OpenSettings);
        AddClick(skinsButton, LogOpenSkins);
        AddClick(factoryButton, LogOpenSlimeFactory);
        AddClick(battlePassButton, LogOpenBattlePass);
        AddClick(coinPlusButton, LogOpenCoinShop);
        AddClick(gemPlusButton, LogOpenGemShop);
        AddClick(buyButton, LogBuyOfferOfTheDay);
        AddClick(bottomOfferPanelButton, LogBuyOfferOfTheDay);
    }

    private void UnbindButtons()
    {
        RemoveClick(settingsButton, OpenSettings);
        RemoveClick(skinsButton, LogOpenSkins);
        RemoveClick(factoryButton, LogOpenSlimeFactory);
        RemoveClick(battlePassButton, LogOpenBattlePass);
        RemoveClick(coinPlusButton, LogOpenCoinShop);
        RemoveClick(gemPlusButton, LogOpenGemShop);
        RemoveClick(buyButton, LogBuyOfferOfTheDay);
        RemoveClick(bottomOfferPanelButton, LogBuyOfferOfTheDay);
    }

    private void AddClick(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button != null)
            button.onClick.AddListener(action);
    }

    private void RemoveClick(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button != null)
            button.onClick.RemoveListener(action);
    }

    private void OpenSettings()
    {
        Debug.Log("Open Settings");
        devSettingsPanel?.Toggle(mainMenuCanvas != null ? mainMenuCanvas.transform : transform);
    }

    private void LogOpenSkins() => Debug.Log("Open Skins");
    private void LogOpenSlimeFactory() => Debug.Log("Open Slime Factory");
    private void LogOpenBattlePass() => Debug.Log("Open Battle Pass");
    private void LogOpenCoinShop() => Debug.Log("Open Coin Shop");
    private void LogOpenGemShop() => Debug.Log("Open Gem Shop");
    private void LogBuyOfferOfTheDay() => Debug.Log("Buy Offer Of The Day");
}
