using UnityEngine;

public interface IEnemyClashTargetDetector
{
    bool IsClashTarget(Collider other);
}
