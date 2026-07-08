using System;

public class ShieldStateMachine : IShieldStateMachine
{
    private readonly ShieldInactiveState inactiveState;
    private readonly ShieldActiveState activeState;
    private readonly ShieldConsumedState consumedState;

    private ShieldStateBase currentState;

    public ShieldStateMachine(
        ShieldInactiveState inactiveState,
        ShieldActiveState activeState,
        ShieldConsumedState consumedState)
    {
        this.inactiveState = inactiveState;
        this.activeState = activeState;
        this.consumedState = consumedState;

        this.inactiveState.Initialize(this);
        this.activeState.Initialize(this);
        this.consumedState.Initialize(this);

        this.inactiveState.SetActiveState(this.activeState);
        this.activeState.SetConsumedState(this.consumedState);
        this.consumedState.SetInactiveState(this.inactiveState);

        ChangeState(this.inactiveState);
    }

    public bool IsActive => currentState == activeState;

    public event Action Activated;
    public event Action Consumed;

    public void Activate()
    {
        currentState.Activate();
    }

    public bool TryConsume()
    {
        return currentState.TryConsume();
    }

    public void ChangeState(ShieldStateBase nextState)
    {
        if (nextState == null || currentState == nextState)
            return;

        currentState?.Exit();
        currentState = nextState;
        currentState.Enter();
    }

    public void NotifyActivated()
    {
        Activated?.Invoke();
    }

    public void NotifyConsumed()
    {
        Consumed?.Invoke();
    }
}
