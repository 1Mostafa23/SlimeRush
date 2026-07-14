public class RapidFireExpiredState : RapidFireStateBase
{
    private RapidFireInactiveState inactiveState;

    public void SetInactiveState(RapidFireInactiveState inactiveState)
    {
        this.inactiveState = inactiveState;
    }

    public override void Enter()
    {
        StateMachine.NotifyExpired();
        StateMachine.ChangeState(inactiveState);
    }
}
