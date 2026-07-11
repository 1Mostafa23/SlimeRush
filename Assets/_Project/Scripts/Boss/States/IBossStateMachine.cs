using UnityEngine;

public interface IBossStateMachine
{
    bool IsDefeated { get; }
    void StartRangedPhase();
    void StopRangedPhase();
    void StartClashPhase(Transform fightPoint, Collider bossTrigger);
    void Defeat();
}
