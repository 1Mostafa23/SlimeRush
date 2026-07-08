using System.Threading;
using UnityEngine;
using Zenject;

[ExecuteAlways]
[RequireComponent(typeof(EnemyCombatant))]
public class AILaneJumperEnemyView : MonoBehaviour, IAILaneJumperEnemyView, IEnemyClashTarget, IDamageBlockedReaction
{
    [SerializeField] private Transform body;
    [SerializeField] private Transform visual;
    [SerializeField] private Transform leftLane;
    [SerializeField] private Transform centerLane;
    [SerializeField] private Transform rightLane;
    [SerializeField] private Transform warningView;
    [SerializeField] private EnemyCombatant combatant;
    [SerializeField] private EnemyPowerView enemyPowerView;
    [SerializeField] private EnemyClashFeedback clashFeedback;
    [SerializeField] private EnemyClashZone clashZone;
    [SerializeField] private AILaneJumperEnemySettings settings;

    [Header("Instance Overrides")]
    [SerializeField] private bool overrideEnemyPower;
    [SerializeField] private int enemyPowerOverride = 5;
    [SerializeField] private float shieldBlockRecoveryDuration = 0.45f;

    private AILaneJumperEnemyController controller;

    public AILaneJumperEnemySettings Settings => settings;
    public int EnemyPower => overrideEnemyPower
        ? Mathf.Max(1, enemyPowerOverride)
        : settings != null ? settings.EnemyPower : 1;
    public float ShieldBlockRecoveryDuration => Mathf.Max(0f, shieldBlockRecoveryDuration);
    public IEnemyCombatant Combatant => combatant;
    public IEnemyClashFeedback ClashFeedback => clashFeedback;
    public Transform Body => body;
    public Transform Visual => visual;
    public Transform LeftLane => leftLane;
    public Transform CenterLane => centerLane;
    public Transform RightLane => rightLane;
    public Transform RootTransform => transform;
    public GameObject GameObject => gameObject;
    public CancellationToken DestroyCancellationToken => destroyCancellationToken;
    public bool IsActiveAndEnabled => isActiveAndEnabled;

    [Inject]
    private void Construct(AILaneJumperEnemyController.Factory controllerFactory)
    {
        ResolveSceneReferences();
        controller = controllerFactory.Create(this);

        if (Application.isPlaying && isActiveAndEnabled)
            controller.Enable();
    }

    private void Awake()
    {
        ResolveSceneReferences();
        HideWarning();
    }

    private void OnValidate()
    {
        enemyPowerOverride = Mathf.Max(1, enemyPowerOverride);
        shieldBlockRecoveryDuration = Mathf.Max(0f, shieldBlockRecoveryDuration);
        ResolveSceneReferences();
        UpdatePowerLabelFromSettings();
    }

    private void ResolveSceneReferences()
    {
        if (combatant == null)
            combatant = GetComponent<EnemyCombatant>();

        if (enemyPowerView == null)
            enemyPowerView = GetComponentInChildren<EnemyPowerView>(true);

        if (clashFeedback == null)
            clashFeedback = GetComponent<EnemyClashFeedback>();

        if (clashZone == null)
            clashZone = GetComponentInChildren<EnemyClashZone>(true);

        controller?.RefreshViewReferences();
    }

    private void OnEnable()
    {
        ResolveSceneReferences();
        AILaneJumperEnemySettings.SettingsChanged += HandleSettingsChanged;
        UpdatePowerLabelFromSettings();

        if (!Application.isPlaying)
            return;

        controller?.Enable();
    }

    private void OnDisable()
    {
        AILaneJumperEnemySettings.SettingsChanged -= HandleSettingsChanged;

        if (!Application.isPlaying)
            return;

        controller?.Disable();
    }

    public void HideWarning()
    {
        if (warningView != null)
            warningView.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        controller?.Dispose();
        controller = null;
    }

    public void SetPowerLabel(int power)
    {
        if (enemyPowerView != null)
            enemyPowerView.SetPower(power);
    }

    public void ShowWarningAt(float xPosition)
    {
        if (warningView == null)
            return;

        Vector3 warningPosition = warningView.position;
        warningView.position = new Vector3(xPosition, warningPosition.y, warningPosition.z);
        warningView.gameObject.SetActive(true);
    }

    public void BeginClash()
    {
        controller?.BeginClash();
    }

    private void UpdatePowerLabelFromSettings()
    {
        SetPowerLabel(EnemyPower);
    }

    private void HandleSettingsChanged(AILaneJumperEnemySettings changedSettings)
    {
        if (changedSettings != settings)
            return;

        UpdatePowerLabelFromSettings();
        controller?.RefreshSettings();
    }

    public void DisableClashZone()
    {
        clashZone?.Disable();
    }

    public void PlayDefeatFeedback()
    {
        clashFeedback?.PlayDefeat();
    }

    public void OnDamageBlocked()
    {
        clashFeedback?.PlayBlocked();
    }
}
