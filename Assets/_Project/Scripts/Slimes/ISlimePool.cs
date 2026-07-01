using UnityEngine;

public interface ISlimePool
{
    int ActiveCount { get; }
    int InactiveCount { get; }
    int TotalCount { get; }

    void Prewarm(int count, Transform parent);
    GameObject Rent(Transform parent);
    void Return(GameObject slime);
    void Clear();
}
