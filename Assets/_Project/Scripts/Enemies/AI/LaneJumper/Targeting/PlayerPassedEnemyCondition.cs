using UnityEngine;

public class PlayerPassedEnemyCondition : IPlayerPassedEnemyCondition
{
    public bool HasPassed(IPlayerCrowdPositionProvider playerPositionProvider, Transform enemyRoot, float zOffset)
    {
        if (playerPositionProvider == null || enemyRoot == null)
            return false;

        return playerPositionProvider.PositionZ > enemyRoot.position.z + zOffset;
    }
}
