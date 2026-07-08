public abstract class ShieldStateBase : IShieldState
{
    protected ShieldStateMachine StateMachine { get; private set; }

    public void Initialize(ShieldStateMachine stateMachine)
    {
        StateMachine = stateMachine;
    }

    public virtual void Enter()
    {
    }

    public virtual void Exit()
    {
    }

    public virtual void Activate()
    {
    }

    public virtual bool TryConsume()
    {
        return false;
    }
}
