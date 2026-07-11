using UnityEngine;

public class BossStateContext
{
    public Transform FightPoint { get; private set; }
    public Collider BossTrigger { get; private set; }

    public void SetClashContext(Transform fightPoint, Collider bossTrigger)
    {
        FightPoint = fightPoint;
        BossTrigger = bossTrigger;
    }
}
