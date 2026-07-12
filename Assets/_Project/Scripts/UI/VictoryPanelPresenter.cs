using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class VictoryPanelPresenter : MonoBehaviour
{
    [SerializeField] private GameObject victoryCanvas;
    [SerializeField] private TMP_Text rewardAmountText;
    [SerializeField] private Button nextLevelButton;
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
        SetVictoryVisible(false);
        SetRewardAmount(0);

        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(ReturnToStartState);
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

        ResumeGameIfPaused();
    }

    private void ShowResult(RunResultData resultData)
    {
        PauseGame();
        SetVictoryVisible(true);

        if (rewardAnimation != null)
            StopCoroutine(rewardAnimation);

        rewardAnimation = StartCoroutine(AnimateReward(resultData.TotalCoins));
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

    private IEnumerator AnimateReward(int targetCoins)
    {
        float elapsedTime = 0f;

        while (elapsedTime < rewardAnimationDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float progress = rewardAnimationDuration <= 0f
                ? 1f
                : Mathf.Clamp01(elapsedTime / rewardAnimationDuration);

            SetRewardAmount(Mathf.RoundToInt(Mathf.Lerp(0, targetCoins, progress)));
            yield return null;
        }

        SetRewardAmount(targetCoins);
        rewardAnimation = null;
    }

    private void SetVictoryVisible(bool visible)
    {
        if (victoryCanvas != null)
            victoryCanvas.SetActive(visible);
    }

    private void SetRewardAmount(int amount)
    {
        if (rewardAmountText != null)
            rewardAmountText.text = $"+{amount}";
    }

    private void ReturnToStartState()
    {
        ResumeGameIfPaused();
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
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
