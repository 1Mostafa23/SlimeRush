public interface IAILaneJumperStateContext
{
    AILaneJumperEnemySettings Settings { get; }
    int EnemyPower { get; }
    bool IsClashing { get; }

    bool CanAttackPlayer();
    void ChangeToPatrol();
    void ChangeToObserve();
    void ChangeToWarning();
    void ChangeToDash();
    void ChangeToClash();
    void ChangeToDefeated();
    void TickPatrol(float deltaTime);
    bool SelectClosestPlayerLane();
    void ShowWarning();
    void HideWarning();
    void BeginDash();
    void TickDash(float deltaTime);
    bool HasReachedDashTarget();
    void ResetClashDamageTimer();
    EnemyClashTickResult TickClash(float deltaTime);
    bool HasPlayerPassed();
    void Defeat();
}
