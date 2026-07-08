public class ShieldInactiveState : ShieldStateBase
{
    private ShieldActiveState activeState;

    public void SetActiveState(ShieldActiveState activeState)
    {
        this.activeState = activeState;
    }

    public override void Activate()
    {
        StateMachine.ChangeState(activeState);
    }

    public override bool TryConsume()
    {
        return false;
    }
}
