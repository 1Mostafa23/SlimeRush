using UnityEngine;
using Zenject;

public class BossTriggerZone : MonoBehaviour
{
    [SerializeField] private Transform bossFightPoint;

    private IBossStateMachine bossStateMachine;
    private Collider triggerCollider;

    [Inject]
    private void Construct(IBossStateMachine bossStateMachine)
    {
        this.bossStateMachine = bossStateMachine;
    }

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayerCrowd(other))
            return;

        bossStateMachine.StartClashPhase(bossFightPoint, triggerCollider);
    }

    private bool IsPlayerCrowd(Collider other)
    {
        return other.TryGetComponent(out PlayerCrowdMarker _) ||
               other.GetComponentInParent<PlayerCrowdMarker>() != null;
    }
}
