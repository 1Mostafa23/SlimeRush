using UnityEngine;

public class PlayerPassedEnemyCondition : IPlayerPassedEnemyCondition
{
    public bool HasPassed(PlayerCrowdController playerCrowdController, Transform enemyRoot, float zOffset)
    {
        if (playerCrowdController == null || enemyRoot == null)
            return false;

        return playerCrowdController.transform.position.z > enemyRoot.position.z + zOffset;
    }
}
