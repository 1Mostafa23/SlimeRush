using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SideMoverEnemySystem : ITickable
{
    private readonly List<ISideMoverEnemy> enemies = new();

    public void Register(ISideMoverEnemy enemy)
    {
        if (enemy == null || enemies.Contains(enemy))
            return;

        enemies.Add(enemy);
    }

    public void Unregister(ISideMoverEnemy enemy)
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
