using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

[ExecuteAlways]
[RequireComponent(typeof(EnemyCombatant))]
public class AILaneJumperEnemyView : MonoBehaviour, IAILaneJumperEnemy, IAILaneJumperStateContext, IEnemyClashTarget
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

    private IEnemyCombatant enemyCombatant;
    private IEnemyPowerView enemyPowerDisplay;
    private IEnemyClashFeedback enemyClashFeedback;
    private IEnemyClashZone enemyClashZone;
    private AILaneJumperEnemySystem laneJumperEnemySystem;
    private IAILaneJumperMover mover;
    private IAILaneJumperVisualAnimator visualAnimator;
    private IEnemyClashService enemyClashService;
    private IPlayerCrowdMovementController playerMovementController;
    private IPlayerCrowdPositionProvider playerPositionProvider;
    private IEnemyDefeatHandler defeatHandler;
    private IPlayerPassedEnemyCondition playerPassedEnemyCondition;
    private EnemyClashTickContext clashTickContext;
    private AILaneJumperStateMachine stateMachine;
    private AILaneJumperPatrolState patrolState;
    private AILaneJumperObserveState observeState;
    private AILaneJumperWarningState warningState;
    private AILaneJumperDashState dashState;
    private AILaneJumperClashState clashState;
    private AILaneJumperDefeatedState defeatedState;
    private bool isRegistered;
    private bool isInitialized;
    private bool isClashing;

    public AILaneJumperEnemySettings Settings => settings;
    public int EnemyPower => enemyCombatant != null ? enemyCombatant.CurrentPower : 0;
    public bool IsClashing => isClashing;

    [Inject]
    private void Construct(
        AILaneJumperEnemySystem laneJumperEnemySystem,
        IAILaneJumperMover mover,
        IAILaneJumperVisualAnimator visualAnimator,
        IEnemyClashService enemyClashService,
        IPlayerCrowdMovementController playerMovementController,
        IPlayerCrowdPositionProvider playerPositionProvider,
        IEnemyDefeatHandler defeatHandler,
        IPlayerPassedEnemyCondition playerPassedEnemyCondition)
    {
        this.laneJumperEnemySystem = laneJumperEnemySystem;
        this.mover = mover;
        this.visualAnimator = visualAnimator;
        this.enemyClashService = enemyClashService;
        this.playerMovementController = playerMovementController;
        this.playerPositionProvider = playerPositionProvider;
        this.defeatHandler = defeatHandler;
        this.playerPassedEnemyCondition = playerPassedEnemyCondition;

        ResolveSceneReferences();
        CreateStates();
        TryRegister();
        TryStartStateMachine();
    }

    private void Awake()
    {
        ResolveSceneReferences();
        HideWarning();
    }

    private void OnValidate()
    {
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

        enemyCombatant = combatant;
        enemyPowerDisplay = enemyPowerView;
        enemyClashFeedback = clashFeedback;
        enemyClashZone = clashZone;

        mover?.Configure(body, leftLane, centerLane, rightLane, settings);
        visualAnimator?.Configure(visual, settings);
    }

    private void OnEnable()
    {
        ResolveSceneReferences();
        AILaneJumperEnemySettings.SettingsChanged += HandleSettingsChanged;
        ApplyCombatSettings();
        isClashing = false;
        UpdatePowerLabel();

        if (!Application.isPlaying)
            return;

        TryRegister();
        TryStartStateMachine();
    }

    private void OnDisable()
    {
        AILaneJumperEnemySettings.SettingsChanged -= HandleSettingsChanged;

        if (!Application.isPlaying)
            return;

        if (laneJumperEnemySystem == null || !isRegistered)
            return;

        laneJumperEnemySystem.Unregister(this);
        isRegistered = false;
    }

    public void Tick(float deltaTime)
    {
        if (body == null || visual == null || settings == null || stateMachine == null)
            return;

        stateMachine.Tick(deltaTime);
        visualAnimator?.Tick(deltaTime);
    }

    public void ChangeToPatrol()
    {
        stateMachine.ChangeState(patrolState);
    }

    public void ChangeToObserve()
    {
        stateMachine.ChangeState(observeState);
    }

    public void ChangeToWarning()
    {
        stateMachine.ChangeState(warningState);
    }

    public void ChangeToDash()
    {
        stateMachine.ChangeState(dashState);
    }

    public void ChangeToClash()
    {
        stateMachine.ChangeState(clashState);
    }

    public void ChangeToDefeated()
    {
        stateMachine.ChangeState(defeatedState);
    }

    public void TickPatrol(float deltaTime)
    {
        mover?.TickPatrol(deltaTime);
    }

    public bool SelectClosestPlayerLane()
    {
        return mover != null && mover.SelectClosestPlayerLane();
    }

    public void ShowWarning()
    {
        if (warningView == null || mover?.SelectedLane == null)
            return;

        Vector3 warningPosition = warningView.position;
        warningView.position = new Vector3(mover.SelectedLane.position.x, warningPosition.y, warningPosition.z);
        warningView.gameObject.SetActive(true);
    }

    public void HideWarning()
    {
        if (warningView != null)
            warningView.gameObject.SetActive(false);
    }

    public void BeginDash()
    {
        mover?.BeginDash();
    }

    public void TickDash(float deltaTime)
    {
        mover?.TickDash(deltaTime);
    }

    public bool HasReachedDashTarget()
    {
        return mover != null && mover.HasReachedDashTarget();
    }

    public void BeginClash()
    {
        if (isClashing || enemyCombatant == null || enemyCombatant.IsDefeated)
            return;

        isClashing = true;
        playerMovementController?.StopMovement();

        if (stateMachine != null)
            ChangeToClash();
    }

    public void EndClash()
    {
        isClashing = false;
    }

    public void ResetClashDamageTimer()
    {
        clashTickContext = new EnemyClashTickContext
        {
            Combatant = enemyCombatant,
            Feedback = enemyClashFeedback,
            TickInterval = settings.ClashTickInterval,
            ElapsedTime = 0f
        };
    }

    public EnemyClashTickResult TickClash(float deltaTime)
    {
        if (clashTickContext == null)
            ResetClashDamageTimer();

        return enemyClashService.Tick(clashTickContext, deltaTime);
    }

    public bool HasPlayerPassed()
    {
        if (playerPositionProvider == null)
            return false;

        return playerPassedEnemyCondition.HasPassed(
            playerPositionProvider,
            transform,
            settings.PlayerPassedZOffset
        );
    }

    public void Defeat()
    {
        HideWarning();
        isClashing = false;
        enemyClashZone?.Disable();
        enemyClashFeedback?.PlayDefeat();
        DefeatAsync().Forget();
    }

    private void CreateStates()
    {
        stateMachine = new AILaneJumperStateMachine();
        patrolState = new AILaneJumperPatrolState(this);
        observeState = new AILaneJumperObserveState(this);
        warningState = new AILaneJumperWarningState(this);
        dashState = new AILaneJumperDashState(this);
        clashState = new AILaneJumperClashState(this);
        defeatedState = new AILaneJumperDefeatedState(this);
    }

    private void UpdatePowerLabel()
    {
        enemyPowerDisplay?.SetPower(EnemyPower);
    }

    private void UpdatePowerLabelFromSettings()
    {
        if (enemyPowerDisplay == null)
            return;

        int displayedPower = settings != null ? settings.EnemyPower : EnemyPower;
        enemyPowerDisplay.SetPower(displayedPower);
    }

    private void HandleSettingsChanged(AILaneJumperEnemySettings changedSettings)
    {
        if (changedSettings != settings)
            return;

        ApplyCombatSettings();
        UpdatePowerLabelFromSettings();
    }

    private void ApplyCombatSettings()
    {
        if (enemyCombatant == null)
            return;

        if (settings != null)
        {
            enemyCombatant.SetMaxPower(settings.EnemyPower);
            return;
        }

        enemyCombatant.ResetCombat();
    }

    private async UniTaskVoid DefeatAsync()
    {
        await UniTask.Delay((int)(settings.DefeatDeactivateDelay * 1000f), cancellationToken: destroyCancellationToken);
        playerMovementController?.StartMovement();
        defeatHandler.Defeat(gameObject);
    }

    private void TryRegister()
    {
        if (!isActiveAndEnabled || laneJumperEnemySystem == null || isRegistered)
            return;

        laneJumperEnemySystem.Register(this);
        isRegistered = true;
    }

    private void TryStartStateMachine()
    {
        if (!isActiveAndEnabled || isInitialized || stateMachine == null)
            return;

        isInitialized = true;
        ChangeToPatrol();
    }
}
