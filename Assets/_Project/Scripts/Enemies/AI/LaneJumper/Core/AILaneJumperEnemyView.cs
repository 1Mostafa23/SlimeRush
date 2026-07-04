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

    private readonly Transform[] lanes = new Transform[3];
    private AILaneJumperEnemySystem laneJumperEnemySystem;
    private IEnemyLaneSelector laneSelector;
    private IEnemyDashSpeedProvider dashSpeedProvider;
    private IEnemyClashService enemyClashService;
    private PlayerCrowdController playerCrowdController;
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
    private Transform selectedLane;
    private Vector3 dashStartPosition;
    private Vector3 dashTargetPosition;
    private Vector3 baseVisualLocalPosition;
    private Vector3 baseVisualLocalScale = Vector3.one;
    private float animationTime;
    private int patrolLaneIndex = 1;
    private int patrolDirection = 1;
    private bool isRegistered;
    private bool isInitialized;
    private bool isClashing;

    public AILaneJumperEnemySettings Settings => settings;
    public int EnemyPower => combatant != null ? combatant.CurrentPower : 0;
    public bool IsClashing => isClashing;

    [Inject]
    private void Construct(
        AILaneJumperEnemySystem laneJumperEnemySystem,
        IEnemyLaneSelector laneSelector,
        IEnemyDashSpeedProvider dashSpeedProvider,
        IEnemyClashService enemyClashService,
        PlayerCrowdController playerCrowdController,
        IEnemyDefeatHandler defeatHandler,
        IPlayerPassedEnemyCondition playerPassedEnemyCondition)
    {
        this.laneJumperEnemySystem = laneJumperEnemySystem;
        this.laneSelector = laneSelector;
        this.dashSpeedProvider = dashSpeedProvider;
        this.enemyClashService = enemyClashService;
        this.playerCrowdController = playerCrowdController;
        this.defeatHandler = defeatHandler;
        this.playerPassedEnemyCondition = playerPassedEnemyCondition;

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
        lanes[0] = leftLane;
        lanes[1] = centerLane;
        lanes[2] = rightLane;

        if (visual != null)
        {
            baseVisualLocalPosition = visual.localPosition;
            baseVisualLocalScale = visual.localScale;
        }

        if (combatant == null)
            combatant = GetComponent<EnemyCombatant>();

        if (enemyPowerView == null)
            enemyPowerView = GetComponentInChildren<EnemyPowerView>(true);

        if (clashFeedback == null)
            clashFeedback = GetComponent<EnemyClashFeedback>();

        if (clashZone == null)
            clashZone = GetComponentInChildren<EnemyClashZone>(true);
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
        AnimateVisual(deltaTime);
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
        Transform patrolLane = lanes[patrolLaneIndex];

        if (patrolLane == null)
            return;

        Vector3 targetPosition = new(patrolLane.position.x, body.position.y, body.position.z);

        body.position = Vector3.MoveTowards(
            body.position,
            targetPosition,
            settings.PatrolSpeed * deltaTime
        );

        if (Mathf.Abs(body.position.x - patrolLane.position.x) <= settings.LaneReachDistance)
            SelectNextPatrolLane();
    }

    public bool SelectClosestPlayerLane()
    {
        selectedLane = laneSelector.SelectLane(lanes);
        return selectedLane != null;
    }

    public void ShowWarning()
    {
        if (warningView == null || selectedLane == null)
            return;

        Vector3 warningPosition = warningView.position;
        warningView.position = new Vector3(selectedLane.position.x, warningPosition.y, warningPosition.z);
        warningView.gameObject.SetActive(true);
    }

    public void HideWarning()
    {
        if (warningView != null)
            warningView.gameObject.SetActive(false);
    }

    public void BeginDash()
    {
        if (selectedLane == null)
            return;

        dashStartPosition = body.position;
        dashTargetPosition = new Vector3(
            selectedLane.position.x,
            body.position.y,
            body.position.z
        );
    }

    public void TickDash(float deltaTime)
    {
        float dashSpeed = dashSpeedProvider != null
            ? dashSpeedProvider.GetDashSpeed(settings)
            : settings.DashSpeed;

        body.position = Vector3.MoveTowards(
            body.position,
            dashTargetPosition,
            dashSpeed * deltaTime
        );

        float distance = Mathf.Max(0.001f, Vector3.Distance(dashStartPosition, dashTargetPosition));
        float traveled = Vector3.Distance(dashStartPosition, body.position);
        float normalizedTravel = Mathf.Clamp01(traveled / distance);
        float heightOffset = Mathf.Sin(normalizedTravel * Mathf.PI) * settings.JumpHeight;
        body.position = new Vector3(body.position.x, dashTargetPosition.y + heightOffset, body.position.z);
    }

    public bool HasReachedDashTarget()
    {
        if (Vector3.SqrMagnitude(body.position - dashTargetPosition) > 0.0001f)
            return false;

        body.position = dashTargetPosition;
        return true;
    }

    public void BeginClash()
    {
        if (isClashing || combatant == null || combatant.IsDefeated)
            return;

        isClashing = true;
        playerCrowdController?.StopMovement();

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
            Combatant = combatant,
            Feedback = clashFeedback,
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
        if (playerCrowdController == null)
            return false;

        return playerPassedEnemyCondition.HasPassed(
            playerCrowdController,
            transform,
            settings.PlayerPassedZOffset
        );
    }

    public void Defeat()
    {
        HideWarning();
        isClashing = false;
        clashZone?.Disable();
        clashFeedback?.PlayDefeat();
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

    private void SelectNextPatrolLane()
    {
        patrolLaneIndex += patrolDirection;

        if (patrolLaneIndex >= lanes.Length)
        {
            patrolLaneIndex = lanes.Length - 2;
            patrolDirection = -1;
            return;
        }

        if (patrolLaneIndex < 0)
        {
            patrolLaneIndex = 1;
            patrolDirection = 1;
        }
    }

    private void AnimateVisual(float deltaTime)
    {
        animationTime += deltaTime;

        float rawWave = Mathf.Sin(animationTime * settings.BounceFrequency * Mathf.PI * 2f);
        float smoothWave = Mathf.SmoothStep(0f, 1f, (rawWave + 1f) * 0.5f);
        float bounce = smoothWave * settings.BounceHeight;
        float squash = smoothWave * settings.SquashAmount;

        Vector3 targetPosition = baseVisualLocalPosition + Vector3.up * bounce;
        Vector3 targetScale = new(
            baseVisualLocalScale.x * (1f + squash),
            baseVisualLocalScale.y * (1f - squash),
            baseVisualLocalScale.z * (1f + squash)
        );

        float smoothAmount = 1f - Mathf.Exp(-settings.VisualSmoothSpeed * deltaTime);
        visual.localPosition = Vector3.Lerp(visual.localPosition, targetPosition, smoothAmount);
        visual.localScale = Vector3.Lerp(visual.localScale, targetScale, smoothAmount);
    }

    private void UpdatePowerLabel()
    {
        if (enemyPowerView != null)
            enemyPowerView.SetPower(EnemyPower);
    }

    private void UpdatePowerLabelFromSettings()
    {
        if (enemyPowerView == null)
            return;

        int displayedPower = settings != null ? settings.EnemyPower : EnemyPower;
        enemyPowerView.SetPower(displayedPower);
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
        if (combatant == null)
            return;

        if (settings != null)
        {
            combatant.SetMaxPower(settings.EnemyPower);
            return;
        }

        combatant.ResetCombat();
    }

    private async UniTaskVoid DefeatAsync()
    {
        await UniTask.Delay((int)(settings.DefeatDeactivateDelay * 1000f), cancellationToken: destroyCancellationToken);
        playerCrowdController?.StartMovement();
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
