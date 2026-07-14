using System;

public interface IGameplayStartService
{
    bool IsGameplayStarted { get; }
    event Action GameplayStarted;
    event Action GameplayReset;
    void StartGameplay();
    void ResetGameplay();
}
