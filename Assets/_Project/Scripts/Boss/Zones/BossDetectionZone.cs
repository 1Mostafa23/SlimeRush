using UnityEngine;
using Zenject;

public class BossDetectionZone : MonoBehaviour
{
    private IBossStateMachine bossStateMachine;

    [Inject]
    private void Construct(IBossStateMachine bossStateMachine)
    {
        this.bossStateMachine = bossStateMachine;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayerCrowd(other))
            bossStateMachine.StartRangedPhase();
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsPlayerCrowd(other))
            bossStateMachine.StopRangedPhase();
    }

    private bool IsPlayerCrowd(Collider other)
    {
        return other.TryGetComponent(out PlayerCrowdMarker _) ||
               other.GetComponentInParent<PlayerCrowdMarker>() != null;
    }
}
