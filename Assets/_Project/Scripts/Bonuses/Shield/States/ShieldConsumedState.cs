public class ShieldConsumedState : ShieldStateBase
{
    private ShieldInactiveState inactiveState;

    public void SetInactiveState(ShieldInactiveState inactiveState)
    {
        this.inactiveState = inactiveState;
    }

    public override void Enter()
    {
        StateMachine.NotifyConsumed();
        StateMachine.ChangeState(inactiveState);
    }
}
