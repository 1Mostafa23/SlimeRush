using UnityEngine;

public interface IAILaneJumperVisualAnimator
{
    void Configure(Transform visual, AILaneJumperEnemySettings settings);
    void Tick(float deltaTime);
}
