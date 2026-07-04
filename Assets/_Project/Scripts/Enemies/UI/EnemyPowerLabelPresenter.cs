using UnityEngine;

public class EnemyPowerLabelPresenter : MonoBehaviour, IEnemyPowerLabelPresenter
{
    [SerializeField] private EnemyCombatant combatant;
    [SerializeField] private EnemyPowerView powerView;

    private void Awake()
    {
        ResolveReferences();
    }

    private void OnEnable()
    {
        ResolveReferences();

        if (combatant != null)
            combatant.PowerChanged += HandlePowerChanged;

        Refresh();
    }

    private void OnDisable()
    {
        if (combatant != null)
            combatant.PowerChanged -= HandlePowerChanged;
    }

    public void Refresh()
    {
        if (combatant != null)
            HandlePowerChanged(combatant.CurrentPower);
    }

    private void ResolveReferences()
    {
        if (combatant == null)
            combatant = GetComponentInParent<EnemyCombatant>();

        if (powerView == null)
            powerView = GetComponent<EnemyPowerView>();
    }

    private void HandlePowerChanged(int currentPower)
    {
        if (powerView != null)
            powerView.SetPower(currentPower);
    }
}
