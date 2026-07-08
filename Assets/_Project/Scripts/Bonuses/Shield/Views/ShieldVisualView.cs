using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using Zenject;

public class ShieldVisualView : MonoBehaviour
{
    [SerializeField] private Transform shieldBubble;
    [SerializeField] private float baseScale = 2.4f;
    [SerializeField] private float scalePerSlime = 0.035f;
    [SerializeField] private float radiusScalePerSqrtSlime = 0.55f;
    [SerializeField] private float minScale = 2.4f;
    [SerializeField] private float maxScale = 5.2f;
    [SerializeField] private float adaptiveMaxScale = 8f;
    [SerializeField] private float breakScaleMultiplier = 1.12f;
    [SerializeField] private float breakDuration = 0.12f;

    private IShieldService shieldService;
    private ISlimeCrowd slimeCrowd;
    private bool isSubscribed;

    [Inject]
    private void Construct(IShieldService shieldService, [InjectOptional] ISlimeCrowd slimeCrowd)
    {
        this.shieldService = shieldService;
        this.slimeCrowd = slimeCrowd;
        TrySubscribe();
    }

    private void Awake()
    {
        if (shieldBubble == null)
            shieldBubble = transform;

        Hide();
    }

    private void OnEnable()
    {
        TrySubscribe();
    }

    private void OnDisable()
    {
        if (!isSubscribed)
            return;

        if (shieldService != null)
        {
            shieldService.Activated -= HandleActivated;
            shieldService.Consumed -= HandleConsumed;
        }

        if (slimeCrowd != null)
            slimeCrowd.OnSlimeCountChanged -= HandleSlimeCountChanged;

        isSubscribed = false;
    }

    private void HandleActivated()
    {
        if (shieldBubble == null)
            return;

        shieldBubble.gameObject.SetActive(true);
        UpdateSize(GetCurrentSlimeCount());
    }

    private void HandleConsumed()
    {
        PlayBreakAsync().Forget();
    }

    private void UpdateSize(int slimeCount)
    {
        float safeSlimeCount = Mathf.Max(0, slimeCount);
        float crowdRadiusScale = Mathf.Sqrt(safeSlimeCount) * radiusScalePerSqrtSlime;
        float crowdPaddingScale = safeSlimeCount * scalePerSlime;
        float scale = Mathf.Clamp(
            baseScale + crowdRadiusScale + crowdPaddingScale,
            minScale,
            Mathf.Max(maxScale, adaptiveMaxScale)
        );

        shieldBubble.localScale = Vector3.one * scale;
    }

    private int GetCurrentSlimeCount()
    {
        return slimeCrowd != null ? slimeCrowd.SlimeCount : 0;
    }

    private async UniTaskVoid PlayBreakAsync()
    {
        try
        {
            if (shieldBubble == null)
                return;

            Vector3 startScale = shieldBubble.localScale;
            shieldBubble.localScale = startScale * breakScaleMultiplier;

            await UniTask.Delay(
                Mathf.RoundToInt(breakDuration * 1000f),
                cancellationToken: destroyCancellationToken
            );

            if (shieldBubble == null)
                return;

            shieldBubble.localScale = startScale;
            Hide();
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void Hide()
    {
        if (shieldBubble != null)
            shieldBubble.gameObject.SetActive(false);
    }

    private void TrySubscribe()
    {
        if (!isActiveAndEnabled || shieldService == null || isSubscribed)
            return;

        shieldService.Activated += HandleActivated;
        shieldService.Consumed += HandleConsumed;

        if (slimeCrowd != null)
            slimeCrowd.OnSlimeCountChanged += HandleSlimeCountChanged;

        isSubscribed = true;

        if (shieldService.IsActive)
            HandleActivated();
        else
            Hide();
    }

    private void HandleSlimeCountChanged(int slimeCount)
    {
        if (shieldService == null || !shieldService.IsActive)
            return;

        UpdateSize(slimeCount);
    }
}
