using System;

public interface IEnemyCombatant
{
    event Action<int> PowerChanged;

    int CurrentPower { get; }
    int MaxPower { get; }
    bool IsDefeated { get; }

    void SetMaxPower(int maxPower);
    void ResetCombat();
    void ReducePower(int amount);
}
