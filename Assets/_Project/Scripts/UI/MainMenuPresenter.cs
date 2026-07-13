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
    [SerializeField] private int currentLevel = 1;

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
    private bool isGameplayStarted;
    private IPlayerCrowdMovementController PlayerMovementController =>
        playerMovementController ?? playerCrowdControllerFallback;

    [Inject]
    private void Construct(
        IPlayerCrowdMovementController playerMovementController,
        IMusicService musicService)
    {
        this.playerMovementController = playerMovementController;
        this.musicService = musicService;
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
    }

    private void Update()
    {
        if (isGameplayStarted)
            return;

        UpdateTapToStartFade();

        if (HasStartInputOutsideUi())
            StartGameplay();
    }

    private void OnDestroy()
    {
        UnbindButtons();
    }

    private void StartGameplay()
    {
        if (isGameplayStarted)
            return;

        isGameplayStarted = true;
        SetMenuVisible(false);
        musicService?.SetGameplayMode();
        PlayerMovementController?.SetInputEnabled(true);
    }

    private void ShowMenu()
    {
        isGameplayStarted = false;
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
            levelText.text = $"Level {currentLevel}";
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
        AddClick(settingsButton, LogOpenSettings);
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
        RemoveClick(settingsButton, LogOpenSettings);
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

    private void LogOpenSettings() => Debug.Log("Open Settings");
    private void LogOpenSkins() => Debug.Log("Open Skins");
    private void LogOpenSlimeFactory() => Debug.Log("Open Slime Factory");
    private void LogOpenBattlePass() => Debug.Log("Open Battle Pass");
    private void LogOpenCoinShop() => Debug.Log("Open Coin Shop");
    private void LogOpenGemShop() => Debug.Log("Open Gem Shop");
    private void LogBuyOfferOfTheDay() => Debug.Log("Buy Offer Of The Day");
}
