using UnityEngine;

public interface IPlayerPassedEnemyCondition
{
    bool HasPassed(IPlayerCrowdPositionProvider playerPositionProvider, Transform enemyRoot, float zOffset);
}
