using UnityEngine;
using Zenject;

public class EnemyClashZone : MonoBehaviour, IEnemyClashZone
{
    [SerializeField] private MonoBehaviour clashTargetBehaviour;
    [SerializeField] private Collider clashCollider;

    private IEnemyClashTarget clashTarget;
    private IEnemyClashTargetDetector targetDetector;
    private bool hasStartedClash;

    [Inject]
    private void Construct(IEnemyClashTargetDetector targetDetector)
    {
        this.targetDetector = targetDetector;
    }

    private void Awake()
    {
        if (clashTargetBehaviour == null)
            clashTargetBehaviour = GetComponentInParent<AILaneJumperEnemyView>();

        clashTarget = clashTargetBehaviour as IEnemyClashTarget;

        if (clashCollider == null)
            clashCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasStartedClash)
            return;

        if (targetDetector == null || !targetDetector.IsClashTarget(other))
            return;

        hasStartedClash = true;
        clashTarget?.BeginClash();
    }

    public void Disable()
    {
        if (clashCollider != null)
            clashCollider.enabled = false;
    }
}
