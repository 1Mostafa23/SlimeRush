public class BossClashState : IBossState
{
    private readonly BossStateContext context;
    private readonly IBossFightService bossFightService;

    public BossClashState(BossStateContext context, IBossFightService bossFightService)
    {
        this.context = context;
        this.bossFightService = bossFightService;
    }

    public void Enter()
    {
        bossFightService.StartCloseFight(context.FightPoint, context.BossTrigger);
    }

    public void Exit()
    {
    }

    public void Tick(float deltaTime)
    {
    }
}
