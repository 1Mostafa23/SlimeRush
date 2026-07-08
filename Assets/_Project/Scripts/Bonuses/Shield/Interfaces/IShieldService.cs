using System;

public interface IShieldService
{
    bool IsActive { get; }
    event Action Activated;
    event Action Consumed;
    void Activate();
    bool TryConsumeShield();
}
