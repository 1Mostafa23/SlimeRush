using System;
using UnityEngine;
using Zenject;

public class BossStateMachine : IBossStateMachine, IInitializable, ITickable, IDisposable
{
    private readonly BossCombatant bossCombatant;
    private readonly BossStateContext context;
    private readonly BossIdleState idleState;
    private readonly BossRangedState rangedState;
    private readonly BossClashState clashState;
    private readonly BossDefeatedState defeatedState;

    private IBossState currentState;

    public bool IsDefeated => bossCombatant.IsDefeated;

    public BossStateMachine(
        BossCombatant bossCombatant,
        BossStateContext context,
        BossIdleState idleState,
        BossRangedState rangedState,
        BossClashState clashState,
        BossDefeatedState defeatedState)
    {
        this.bossCombatant = bossCombatant;
        this.context = context;
        this.idleState = idleState;
        this.rangedState = rangedState;
        this.clashState = clashState;
        this.defeatedState = defeatedState;
    }

    public void Initialize()
    {
        bossCombatant.Defeated += Defeat;
        ChangeState(idleState);
    }

    public void Dispose()
    {
        bossCombatant.Defeated -= Defeat;
    }

    public void Tick()
    {
        currentState?.Tick(Time.deltaTime);
    }

    public void StartRangedPhase()
    {
        if (IsDefeated || currentState == clashState)
            return;

        ChangeState(rangedState);
    }

    public void StopRangedPhase()
    {
        if (currentState == rangedState)
            ChangeState(idleState);
    }

    public void StartClashPhase(Transform fightPoint, Collider bossTrigger)
    {
        if (IsDefeated)
            return;

        context.SetClashContext(fightPoint, bossTrigger);
        ChangeState(clashState);
    }

    public void Defeat()
    {
        if (currentState == defeatedState)
            return;

        ChangeState(defeatedState);
    }

    private void ChangeState(IBossState nextState)
    {
        if (nextState == null || currentState == nextState)
            return;

        currentState?.Exit();
        currentState = nextState;
        currentState.Enter();
    }
}

public class BossDefeatedState : IBossState
{
    private readonly BossStateContext context;
    private readonly IBossFightService bossFightService;
    private readonly BossDefeatView bossDefeatView;

    public BossDefeatedState(
        BossStateContext context,
        IBossFightService bossFightService,
        BossDefeatView bossDefeatView)
    {
        this.context = context;
        this.bossFightService = bossFightService;
        this.bossDefeatView = bossDefeatView;
    }

    public void Enter()
    {
        bossFightService.StopCloseFight(true);

        if (context.BossTrigger != null)
            context.BossTrigger.enabled = false;

        bossDefeatView.HideBoss();
    }

    public void Exit()
    {
    }

    public void Tick(float deltaTime)
    {
    }
}
