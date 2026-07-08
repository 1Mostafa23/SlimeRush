using System;

public class ShieldService : IShieldService
{
    private readonly IShieldStateMachine stateMachine;

    public ShieldService(IShieldStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public bool IsActive => stateMachine.IsActive;

    public event Action Activated
    {
        add => stateMachine.Activated += value;
        remove => stateMachine.Activated -= value;
    }

    public event Action Consumed
    {
        add => stateMachine.Consumed += value;
        remove => stateMachine.Consumed -= value;
    }

    public void Activate()
    {
        stateMachine.Activate();
    }

    public bool TryConsumeShield()
    {
        return stateMachine.TryConsume();
    }
}
