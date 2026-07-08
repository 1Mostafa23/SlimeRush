using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class AILaneJumperEnemyController : IAILaneJumperEnemy, IAILaneJumperStateContext, IDisposable
{
    private readonly IAILaneJumperEnemyView view;
    private readonly AILaneJumperEnemySystem laneJumperEnemySystem;
    private readonly IAILaneJumperMover mover;
    private readonly IAILaneJumperVisualAnimator visualAnimator;
    private readonly IEnemyClashService enemyClashService;
    private readonly IPlayerCrowdMovementController playerMovementController;
    private readonly IPlayerCrowdPositionProvider playerPositionProvider;
    private readonly IEnemyDefeatHandler defeatHandler;
    private readonly IPlayerPassedEnemyCondition playerPassedEnemyCondition;

    private readonly AILaneJumperStateMachine stateMachine = new();
    private readonly AILaneJumperPatrolState patrolState;
    private readonly AILaneJumperObserveState observeState;
    private readonly AILaneJumperWarningState warningState;
    private readonly AILaneJumperDashState dashState;
    private readonly AILaneJumperClashState clashState;
    private readonly AILaneJumperDefeatedState defeatedState;

    private EnemyClashTickContext clashTickContext;
    private bool isRegistered;
    private bool isInitialized;
    private bool isClashing;

    public AILaneJumperEnemyController(
        IAILaneJumperEnemyView view,
        AILaneJumperEnemySystem laneJumperEnemySystem,
        IAILaneJumperMover mover,
        IAILaneJumperVisualAnimator visualAnimator,
        IEnemyClashService enemyClashService,
        IPlayerCrowdMovementController playerMovementController,
        IPlayerCrowdPositionProvider playerPositionProvider,
        IEnemyDefeatHandler defeatHandler,
        IPlayerPassedEnemyCondition playerPassedEnemyCondition)
    {
        this.view = view;
        this.laneJumperEnemySystem = laneJumperEnemySystem;
        this.mover = mover;
        this.visualAnimator = visualAnimator;
        this.enemyClashService = enemyClashService;
        this.playerMovementController = playerMovementController;
        this.playerPositionProvider = playerPositionProvider;
        this.defeatHandler = defeatHandler;
        this.playerPassedEnemyCondition = playerPassedEnemyCondition;

        patrolState = new AILaneJumperPatrolState(this);
        observeState = new AILaneJumperObserveState(this);
        warningState = new AILaneJumperWarningState(this);
        dashState = new AILaneJumperDashState(this);
        clashState = new AILaneJumperClashState(this);
        defeatedState = new AILaneJumperDefeatedState(this);

        RefreshViewReferences();
    }

    public AILaneJumperEnemySettings Settings => view.Settings;
    public int EnemyPower => view.Combatant != null ? view.Combatant.CurrentPower : 0;
    public bool IsClashing => isClashing;

    public void Enable()
    {
        RefreshViewReferences();
        ApplyCombatSettings();
        isClashing = false;
        UpdatePowerLabel();
        TryRegister();
        TryStartStateMachine();
    }

    public void Disable()
    {
        if (!isRegistered)
            return;

        laneJumperEnemySystem.Unregister(this);
        isRegistered = false;
    }

    public void Dispose()
    {
        Disable();
    }

    public void RefreshViewReferences()
    {
        mover?.Configure(view.Body, view.LeftLane, view.CenterLane, view.RightLane, view.Settings);
        visualAnimator?.Configure(view.Visual, view.Settings);
    }

    public void RefreshSettings()
    {
        RefreshViewReferences();
        ApplyCombatSettings();
        UpdatePowerLabelFromSettings();
    }

    public void Tick(float deltaTime)
    {
        if (view.Body == null || view.Visual == null || view.Settings == null)
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
        if (mover?.SelectedLane == null)
            return;

        view.ShowWarningAt(mover.SelectedLane.position.x);
    }

    public void HideWarning()
    {
        view.HideWarning();
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
        if (isClashing || view.Combatant == null || view.Combatant.IsDefeated)
            return;

        isClashing = true;
        playerMovementController?.StopMovement();
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
            Combatant = view.Combatant,
            Feedback = view.ClashFeedback,
            BlockedReaction = view as IDamageBlockedReaction,
            TickInterval = view.Settings.ClashTickInterval,
            BlockedRecoveryDuration = view.ShieldBlockRecoveryDuration,
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
        if (playerPositionProvider == null || view.Settings == null || view.RootTransform == null)
            return false;

        return playerPassedEnemyCondition.HasPassed(
            playerPositionProvider,
            view.RootTransform,
            view.Settings.PlayerPassedZOffset
        );
    }

    public void Defeat()
    {
        view.HideWarning();
        isClashing = false;
        view.DisableClashZone();
        view.PlayDefeatFeedback();
        DefeatAsync().Forget();
    }

    private void ApplyCombatSettings()
    {
        if (view.Combatant == null)
            return;

        view.Combatant.SetMaxPower(view.EnemyPower);
    }

    private void UpdatePowerLabel()
    {
        view.SetPowerLabel(EnemyPower);
    }

    private void UpdatePowerLabelFromSettings()
    {
        view.SetPowerLabel(view.EnemyPower);
    }

    private async UniTaskVoid DefeatAsync()
    {
        try
        {
            if (view.Settings != null)
            {
                await UniTask.Delay(
                    (int)(view.Settings.DefeatDeactivateDelay * 1000f),
                    cancellationToken: view.DestroyCancellationToken
                );
            }

            playerMovementController?.StartMovement();
            defeatHandler.Defeat(view.GameObject);
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void TryRegister()
    {
        if (!view.IsActiveAndEnabled || laneJumperEnemySystem == null || isRegistered)
            return;

        laneJumperEnemySystem.Register(this);
        isRegistered = true;
    }

    private void TryStartStateMachine()
    {
        if (!view.IsActiveAndEnabled || isInitialized)
            return;

        isInitialized = true;
        ChangeToPatrol();
    }

    public class Factory : PlaceholderFactory<IAILaneJumperEnemyView, AILaneJumperEnemyController>
    {
    }
}
