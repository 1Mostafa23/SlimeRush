public interface IBossRuntimeContext
{
    BossRangedAttackController RangedAttackController { get; }
    BossCameraController CameraController { get; }
    BossHitFeedback HitFeedback { get; }
    BossDefeatView DefeatView { get; }

    void Register(BossRuntimeBinder binder);
    void Unregister(BossRuntimeBinder binder);
}
