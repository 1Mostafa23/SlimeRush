public abstract class RapidFireStateBase : IRapidFireState
{
    protected RapidFireStateMachine StateMachine { get; private set; }

    public void Initialize(RapidFireStateMachine stateMachine)
    {
        StateMachine = stateMachine;
    }

    public virtual void Enter()
    {
    }

    public virtual void Exit()
    {
    }

    public virtual void Activate(float duration)
    {
    }

    public virtual void Tick(float deltaTime)
    {
    }
}
