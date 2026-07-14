using System;

public interface IRapidFireService
{
    bool IsActive { get; }
    bool IsPaused { get; }
    event Action Activated;
    event Action Expired;
    event Action Paused;
    event Action Resumed;

    void Activate(float duration);
    void Pause();
    void Resume();
}
