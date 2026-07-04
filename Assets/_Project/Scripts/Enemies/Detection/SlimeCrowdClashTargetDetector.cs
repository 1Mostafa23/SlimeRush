using UnityEngine;

public class SlimeCrowdClashTargetDetector : IEnemyClashTargetDetector
{
    public bool IsClashTarget(Collider other)
    {
        if (other == null)
            return false;

        if (other.TryGetComponent(out SlimeHitbox _) || other.GetComponentInParent<SlimeHitbox>() != null)
            return true;

        return other.TryGetComponent(out PlayerCrowdController _)
               || other.GetComponentInParent<PlayerCrowdController>() != null;
    }
}
