using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AILaneJumperEnemySystem : ITickable
{
    private readonly List<IAILaneJumperEnemy> enemies = new();

    public void Register(IAILaneJumperEnemy enemy)
    {
        if (enemy == null || enemies.Contains(enemy))
            return;

        enemies.Add(enemy);
    }

    public void Unregister(IAILaneJumperEnemy enemy)
    {
        if (enemy == null)
            return;

        enemies.Remove(enemy);
    }

    public void Tick()
    {
        float deltaTime = Time.deltaTime;

        for (int i = enemies.Count - 1; i >= 0; i--)
            enemies[i].Tick(deltaTime);
    }
}
