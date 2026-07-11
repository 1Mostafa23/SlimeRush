using System;
using UnityEngine;

public class BossCombatant
{
    public int MaxHp { get; private set; }
    public int CurrentHp { get; private set; }
    public bool IsDefeated => CurrentHp <= 0;

    public event Action<int, int> HpChanged;
    public event Action Defeated;

    public BossCombatant(int maxHp = 50)
    {
        MaxHp = Mathf.Max(1, maxHp);
        CurrentHp = MaxHp;
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || IsDefeated)
            return;

        CurrentHp = Mathf.Max(0, CurrentHp - amount);
        HpChanged?.Invoke(CurrentHp, MaxHp);

        if (IsDefeated)
            Defeated?.Invoke();
    }
}
