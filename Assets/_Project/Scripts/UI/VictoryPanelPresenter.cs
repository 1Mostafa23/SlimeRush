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

    private IRunResultService runResultService;
    private Coroutine rewardAnimation;
    private bool isSubscribed;
    private bool isPausedByVictory;

    [Inject]
    private void Construct(IRunResultService runResultService)
    {
        this.runResultService = runResultService;
    }

    private void Awake()
    {
        SetResultVisible(false);
        SetPanelsVisible(false, false);
        SetRewardAmount(victoryRewardAmountText, 0);
        SetRewardAmount(defeatRewardAmountText, 0);

        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(ReturnToStartState);

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
            nextLevelButton.onClick.RemoveListener(ReturnToStartState);

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
