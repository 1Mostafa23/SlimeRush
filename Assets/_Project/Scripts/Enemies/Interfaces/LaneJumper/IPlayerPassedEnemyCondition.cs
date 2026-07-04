using UnityEngine;

public interface IPlayerPassedEnemyCondition
{
    bool HasPassed(PlayerCrowdController playerCrowdController, Transform enemyRoot, float zOffset);
}
