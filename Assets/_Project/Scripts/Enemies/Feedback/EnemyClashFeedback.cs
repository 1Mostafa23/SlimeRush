using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemyClashFeedback : MonoBehaviour, IEnemyClashFeedback
{
    [SerializeField] private Transform visual;
    [SerializeField] private float tickPunchDistance = 0.12f;
    [SerializeField] private float tickSquashAmount = 0.12f;
    [SerializeField] private float blockedPunchDistance = 0.28f;
    [SerializeField] private float blockedSquashAmount = 0.18f;
    [SerializeField] private float feedbackDuration = 0.08f;
    [SerializeField] private float defeatScaleAmount = 0.25f;

    private Vector3 baseLocalPosition;
    private Vector3 baseLocalScale = Vector3.one;

    private void Awake()
    {
        if (visual == null)
            visual = transform;

        baseLocalPosition = visual.localPosition;
        baseLocalScale = visual.localScale;
    }

    public void PlayTick()
    {
        PlayTickAsync().Forget();
    }

    public void PlayBlocked()
    {
        PlayBlockedAsync().Forget();
    }

    public void PlayDefeat()
    {
        PlayDefeatAsync().Forget();
    }

    private async UniTaskVoid PlayTickAsync()
    {
        if (visual == null)
            return;

        visual.localPosition = baseLocalPosition + Vector3.back * tickPunchDistance;
        visual.localScale = new Vector3(
            baseLocalScale.x * (1f + tickSquashAmount),
            baseLocalScale.y * (1f - tickSquashAmount),
            baseLocalScale.z * (1f + tickSquashAmount)
        );

        await UniTask.Delay((int)(feedbackDuration * 1000f), cancellationToken: destroyCancellationToken);

        if (visual == null)
            return;

        visual.localPosition = baseLocalPosition;
        visual.localScale = baseLocalScale;
    }

    private async UniTaskVoid PlayBlockedAsync()
    {
        if (visual == null)
            return;

        visual.localPosition = baseLocalPosition + Vector3.back * blockedPunchDistance;
        visual.localScale = new Vector3(
            baseLocalScale.x * (1f + blockedSquashAmount),
            baseLocalScale.y * (1f - blockedSquashAmount),
            baseLocalScale.z * (1f + blockedSquashAmount)
        );

        await UniTask.Delay((int)(feedbackDuration * 1000f), cancellationToken: destroyCancellationToken);

        if (visual == null)
            return;

        visual.localPosition = baseLocalPosition;
        visual.localScale = baseLocalScale;
    }

    private async UniTaskVoid PlayDefeatAsync()
    {
        if (visual == null)
            return;

        visual.localScale = baseLocalScale * (1f + defeatScaleAmount);
        await UniTask.Delay((int)(feedbackDuration * 1000f), cancellationToken: destroyCancellationToken);

        if (visual != null)
            visual.localScale = baseLocalScale;
    }
}
