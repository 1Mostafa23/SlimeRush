using System;

public interface IShieldStateMachine
{
    bool IsActive { get; }
    event Action Activated;
    event Action Consumed;
    void Activate();
    bool TryConsume();
}
