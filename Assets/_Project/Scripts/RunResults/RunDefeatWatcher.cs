using Zenject;

public class RunDefeatWatcher : ITickable
{
    private readonly ISlimeCrowd slimeCrowd;
    private readonly IRunResultService runResultService;

    private bool hasHadSlimes;

    public RunDefeatWatcher(ISlimeCrowd slimeCrowd, IRunResultService runResultService)
    {
        this.slimeCrowd = slimeCrowd;
        this.runResultService = runResultService;
    }

    public void Tick()
    {
        if (runResultService.IsCompleted)
            return;

        if (slimeCrowd.SlimeCount > 0)
        {
            hasHadSlimes = true;
            return;
        }

        if (!hasHadSlimes)
            return;

        runResultService.CompleteRun();
    }
}
