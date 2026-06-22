using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SlimeFactory : ISlimeFactory
{
    private readonly GameObject slimePrefab;

    public SlimeFactory(GameObject slimePrefab)
    {
        this.slimePrefab = slimePrefab;
    }

    public UniTask InitializeAsync(CancellationToken cancellationToken)
    {
        return UniTask.CompletedTask;
    }

    public GameObject Create(Transform parent)
    {
        return Object.Instantiate(slimePrefab, parent);
    }
}
