using UnityEngine;
using Zenject;

public class BossRuntimeBinder : MonoBehaviour
{
    [SerializeField] private BossRangedAttackController rangedAttackController;
    [SerializeField] private BossCameraController cameraController;
    [SerializeField] private BossHitFeedback hitFeedback;
    [SerializeField] private BossDefeatView defeatView;

    private IBossRuntimeContext runtimeContext;

    public BossRangedAttackController RangedAttackController => rangedAttackController;
    public BossCameraController CameraController => cameraController;
    public BossHitFeedback HitFeedback => hitFeedback;
    public BossDefeatView DefeatView => defeatView;

    [Inject]
    private void Construct(IBossRuntimeContext runtimeContext)
    {
        this.runtimeContext = runtimeContext;
    }

    private void Awake()
    {
        if (rangedAttackController == null)
            rangedAttackController = GetComponentInChildren<BossRangedAttackController>(true);

        if (cameraController == null)
            cameraController = GetComponentInChildren<BossCameraController>(true);

        if (hitFeedback == null)
            hitFeedback = GetComponentInChildren<BossHitFeedback>(true);

        if (defeatView == null)
            defeatView = GetComponentInChildren<BossDefeatView>(true);
    }

    private void OnEnable()
    {
        runtimeContext?.Register(this);
    }

    private void OnDisable()
    {
        runtimeContext?.Unregister(this);
    }
}
