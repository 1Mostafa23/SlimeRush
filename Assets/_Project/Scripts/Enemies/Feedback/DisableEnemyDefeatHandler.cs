using UnityEngine;

public class DisableEnemyDefeatHandler : IEnemyDefeatHandler
{
    public void Defeat(GameObject enemyRoot)
    {
        if (enemyRoot != null)
            enemyRoot.SetActive(false);
    }
}
