using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class VictoryPanelPresenter : MonoBehaviour
{
    [SerializeField] private GameObject resultCanvas;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private TMP_Text victoryRewardAmountText;
    [SerializeField] private TMP_Text defeatRewardAmountText;
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button homeButton;
    [SerializeField] private float rewardAnimationDuration = 1f;
    [Header("Responsive Layout")]
    [SerializeField] private float panelMaxWidthPercent = 0.96f;
    [SerializeField] private float panelMaxHeightPercent = 0.9f;
    [SerializeField] private float panelMinScale = 1.05f;
    [SerializeField] private float panelMaxScale = 2.2f;

    private IRunResultService runResultService;
    private ILevelProgressService levelProgressService;
    private ILevelConfigProvider levelConfigProvider;
    private Coroutine rewardAnimation;
    private bool isSubscribed;
    private bool isPausedByVictory;
    private Vector3 victoryPanelInitialScale = Vector3.one;
    private Vector3 defeatPanelInitialScale = Vector3.one;

    [Inject]
    private void Construct(
        IRunResultService runResultService,
        ILevelProgressService levelProgressService,
        [Inject(Optional = true)] ILevelConfigProvider levelConfigProvider)
    {
        this.runResultService = runResultService;
        this.levelProgressService = levelProgressService;
        this.levelConfigProvider = levelConfigProvider;
    }

    private void Awake()
    {
        SetResultVisible(false);
        SetPanelsVisible(false, false);
        SetRewardAmount(victoryRewardAmountText, 0);
        SetRewardAmount(defeatRewardAmountText, 0);
        CachePanelScales();

        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(GoToNextLevel);

        if (retryButton != null)
            retryButton.onClick.AddListener(RetryRun);

        if (homeButton != null)
            homeButton.onClick.AddListener(ReturnToStartState);
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void Start()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        if (nextLevelButton != null)
            nextLevelButton.onClick.RemoveListener(GoToNextLevel);

        if (retryButton != null)
            retryButton.onClick.RemoveListener(RetryRun);

        if (homeButton != null)
            homeButton.onClick.RemoveListener(ReturnToStartState);

        ResumeGameIfPaused();
    }

    private void ShowResult(RunResultData resultData)
    {
        PauseGame();
        SetResultVisible(true);
        SetPanelsVisible(resultData.IsVictory, !resultData.IsVictory);
        ApplyResponsivePanelScale(resultData.IsVictory ? victoryPanel : defeatPanel);

        if (rewardAnimation != null)
            StopCoroutine(rewardAnimation);

        TMP_Text rewardText = resultData.IsVictory
            ? victoryRewardAmountText
            : defeatRewardAmountText;

        SetRewardAmount(rewardText, 0);
        rewardAnimation = StartCoroutine(AnimateReward(rewardText, resultData.TotalCoins));
    }

    private void Subscribe()
    {
        if (isSubscribed || runResultService == null)
            return;

        runResultService.RunCompleted += ShowResult;
        isSubscribed = true;
    }

    private void Unsubscribe()
    {
        if (!isSubscribed || runResultService == null)
            return;

        runResultService.RunCompleted -= ShowResult;
        isSubscribed = false;
    }

    private IEnumerator AnimateReward(TMP_Text rewardText, int targetCoins)
    {
        float elapsedTime = 0f;

        while (elapsedTime < rewardAnimationDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float progress = rewardAnimationDuration <= 0f
                ? 1f
                : Mathf.Clamp01(elapsedTime / rewardAnimationDuration);

            SetRewardAmount(rewardText, Mathf.RoundToInt(Mathf.Lerp(0, targetCoins, progress)));
            yield return null;
        }

        SetRewardAmount(rewardText, targetCoins);
        rewardAnimation = null;
    }

    private void SetResultVisible(bool visible)
    {
        if (resultCanvas != null)
            resultCanvas.SetActive(visible);
    }

    private void SetPanelsVisible(bool victoryVisible, bool defeatVisible)
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(victoryVisible);

        if (defeatPanel != null)
            defeatPanel.SetActive(defeatVisible);
    }

    private void CachePanelScales()
    {
        if (victoryPanel != null)
            victoryPanelInitialScale = victoryPanel.transform.localScale;

        if (defeatPanel != null)
            defeatPanelInitialScale = defeatPanel.transform.localScale;
    }

    private void ApplyResponsivePanelScale(GameObject panel)
    {
        if (resultCanvas == null || panel == null)
            return;

        RectTransform canvasRect = resultCanvas.GetComponent<RectTransform>();
        RectTransform panelRect = panel.GetComponent<RectTransform>();

        if (canvasRect == null || panelRect == null)
            return;

        Canvas.ForceUpdateCanvases();

        Vector2 canvasSize = canvasRect.rect.size;
        Vector2 panelSize = panelRect.sizeDelta;

        if (canvasSize.x <= 0f || canvasSize.y <= 0f || panelSize.x <= 0f || panelSize.y <= 0f)
            return;

        float widthScale = canvasSize.x * panelMaxWidthPercent / panelSize.x;
        float heightScale = canvasSize.y * panelMaxHeightPercent / panelSize.y;
        float targetScale = Mathf.Clamp(Mathf.Min(widthScale, heightScale), panelMinScale, panelMaxScale);
        Vector3 initialScale = panel == victoryPanel ? victoryPanelInitialScale : defeatPanelInitialScale;

        panelRect.localScale = initialScale * targetScale;
    }

    private void SetRewardAmount(TMP_Text rewardText, int amount)
    {
        if (rewardText != null)
            rewardText.text = $"+{amount}";
    }

    private void ReturnToStartState()
    {
        ResumeGameIfPaused();
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    private void GoToNextLevel()
    {
        if (levelConfigProvider != null)
            levelProgressService?.AdvanceToNextLevel(levelConfigProvider.MaxAvailableLevel);
        else
            levelProgressService?.AdvanceToNextLevel();

        ReturnToStartState();
    }

    private void RetryRun()
    {
        ReturnToStartState();
    }

    private void PauseGame()
    {
        if (isPausedByVictory)
            return;

        Time.timeScale = 0f;
        isPausedByVictory = true;
    }

    private void ResumeGameIfPaused()
    {
        if (!isPausedByVictory)
            return;

        Time.timeScale = 1f;
        isPausedByVictory = false;
    }
}
