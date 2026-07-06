using UnityEngine;

public interface IAILaneJumperMover
{
    Transform SelectedLane { get; }

    void Configure(
        Transform body,
        Transform leftLane,
        Transform centerLane,
        Transform rightLane,
        AILaneJumperEnemySettings settings);

    void TickPatrol(float deltaTime);
    bool SelectClosestPlayerLane();
    void BeginDash();
    void TickDash(float deltaTime);
    bool HasReachedDashTarget();
}
