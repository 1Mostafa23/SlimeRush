using System;
using Zenject;

public class RapidFireStateMachine : IRapidFireStateMachine, ITickable
{
    private readonly RapidFireInactiveState inactiveState;
    private readonly RapidFireActiveState activeState;
    private readonly RapidFireExpiredState expiredState;

    private RapidFireStateBase currentState;
    private int pauseRequests;

    public bool IsActive => currentState == activeState;
    public bool IsPaused => pauseRequests > 0;

    public event Action Activated;
    public event Action Expired;
    public event Action Paused;
    public event Action Resumed;

    public RapidFireStateMachine(
        RapidFireInactiveState inactiveState,
        RapidFireActiveState activeState,
        RapidFireExpiredState expiredState)
    {
        this.inactiveState = inactiveState;
        this.activeState = activeState;
        this.expiredState = expiredState;

        this.inactiveState.Initialize(this);
        this.activeState.Initialize(this);
        this.expiredState.Initialize(this);

        this.inactiveState.SetActiveState(this.activeState);
        this.activeState.SetExpiredState(this.expiredState);
        this.expiredState.SetInactiveState(this.inactiveState);

        ChangeState(this.inactiveState);
    }

    public void Activate(float duration)
    {
        if (duration <= 0f)
            return;

        currentState.Activate(duration);
    }

    public void Tick()
    {
        if (IsPaused)
            return;

        currentState?.Tick(UnityEngine.Time.deltaTime);
    }

    public void Pause()
    {
        pauseRequests++;

        if (pauseRequests == 1)
            Paused?.Invoke();
    }

    public void Resume()
    {
        if (pauseRequests <= 0)
            return;

        pauseRequests--;

        if (pauseRequests == 0)
            Resumed?.Invoke();
    }

    public void ChangeState(RapidFireStateBase nextState)
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

    public void NotifyExpired()
    {
        Expired?.Invoke();
    }
}
