using System;

public class RapidFireService : IRapidFireService
{
    private readonly IRapidFireStateMachine stateMachine;

    public RapidFireService(IRapidFireStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public bool IsActive => stateMachine.IsActive;
    public bool IsPaused => stateMachine.IsPaused;

    public event Action Activated
    {
        add => stateMachine.Activated += value;
        remove => stateMachine.Activated -= value;
    }

    public event Action Expired
    {
        add => stateMachine.Expired += value;
        remove => stateMachine.Expired -= value;
    }

    public event Action Paused
    {
        add => stateMachine.Paused += value;
        remove => stateMachine.Paused -= value;
    }

    public event Action Resumed
    {
        add => stateMachine.Resumed += value;
        remove => stateMachine.Resumed -= value;
    }

    public void Activate(float duration)
    {
        stateMachine.Activate(duration);
    }

    public void Pause()
    {
        stateMachine.Pause();
    }

    public void Resume()
    {
        stateMachine.Resume();
    }
}
