public class BossRuntimeContext : IBossRuntimeContext
{
    private BossRuntimeBinder currentBinder;

    public BossRangedAttackController RangedAttackController => currentBinder != null ? currentBinder.RangedAttackController : null;
    public BossCameraController CameraController => currentBinder != null ? currentBinder.CameraController : null;
    public BossHitFeedback HitFeedback => currentBinder != null ? currentBinder.HitFeedback : null;
    public BossDefeatView DefeatView => currentBinder != null ? currentBinder.DefeatView : null;

    public void Register(BossRuntimeBinder binder)
    {
        currentBinder = binder;
    }

    public void Unregister(BossRuntimeBinder binder)
    {
        if (currentBinder == binder)
            currentBinder = null;
    }
}
