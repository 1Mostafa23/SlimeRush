using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface ISlimeFactory
{
    UniTask InitializeAsync(CancellationToken cancellationToken); // cancellationToken is used to cancel the initialization process if needed
    GameObject Create(Transform parent);
    void Release(GameObject slime);
}
