using System;
using UnityEngine;

public class EnemyCombatant : MonoBehaviour, IEnemyCombatant
{
    private int maxPower = 1;

    public event Action<int> PowerChanged;

    public int CurrentPower { get; private set; }
    public int MaxPower => maxPower;
    public bool IsDefeated { get; private set; }

    private void OnEnable()
    {
        ResetCombat();
    }

    public void SetMaxPower(int maxPower)
    {
        this.maxPower = Mathf.Max(1, maxPower);
        ResetCombat();
    }

    public void ResetCombat()
    {
        CurrentPower = maxPower;
        IsDefeated = false;
        PowerChanged?.Invoke(CurrentPower);
    }

    public void ReducePower(int amount)
    {
        if (IsDefeated || amount <= 0)
            return;

        int nextPower = Mathf.Max(0, CurrentPower - amount);

        if (nextPower == CurrentPower)
            return;

        CurrentPower = nextPower;

        if (CurrentPower <= 0)
            IsDefeated = true;

        PowerChanged?.Invoke(CurrentPower);
    }
}
