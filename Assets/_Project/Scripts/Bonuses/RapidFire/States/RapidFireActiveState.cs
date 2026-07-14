public class RapidFireActiveState : RapidFireStateBase
{
    private RapidFireExpiredState expiredState;
    private float remainingTime;

    public void SetExpiredState(RapidFireExpiredState expiredState)
    {
        this.expiredState = expiredState;
    }

    public void SetDuration(float duration)
    {
        remainingTime = duration;
    }

    public override void Enter()
    {
        StateMachine.NotifyActivated();
    }

    public override void Activate(float duration)
    {
        remainingTime = duration;
        StateMachine.NotifyActivated();
    }

    public override void Tick(float deltaTime)
    {
        remainingTime -= deltaTime;

        if (remainingTime <= 0f)
            StateMachine.ChangeState(expiredState);
    }
}
