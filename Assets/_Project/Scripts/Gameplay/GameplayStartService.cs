using System;

public class GameplayStartService : IGameplayStartService
{
    public bool IsGameplayStarted { get; private set; }

    public event Action GameplayStarted;
    public event Action GameplayReset;

    public void StartGameplay()
    {
        if (IsGameplayStarted)
            return;

        IsGameplayStarted = true;
        GameplayStarted?.Invoke();
    }

    public void ResetGameplay()
    {
        if (!IsGameplayStarted)
            return;

        IsGameplayStarted = false;
        GameplayReset?.Invoke();
    }
}
