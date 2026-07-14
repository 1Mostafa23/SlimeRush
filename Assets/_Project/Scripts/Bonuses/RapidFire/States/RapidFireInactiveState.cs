public class RapidFireInactiveState : RapidFireStateBase
{
    private RapidFireActiveState activeState;

    public void SetActiveState(RapidFireActiveState activeState)
    {
        this.activeState = activeState;
    }

    public override void Activate(float duration)
    {
        activeState.SetDuration(duration);
        StateMachine.ChangeState(activeState);
    }
}
