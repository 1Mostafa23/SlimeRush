using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface ISlimeFactory
{
    UniTask InitializeAsync(CancellationToken cancellationToken);
    GameObject Create(Transform parent);
}
