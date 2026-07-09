using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IEnemyFactory
{
    UniTask<GameObject> CreateAsync(
        EnemyAddress enemyAddress,
        Vector3 position,
        Quaternion rotation,
        Transform parent,
        CancellationToken cancellationToken
    );
}
