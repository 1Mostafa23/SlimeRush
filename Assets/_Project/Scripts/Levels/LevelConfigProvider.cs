public class LevelConfigProvider : ILevelConfigProvider
{
    private readonly ILevelProgressService levelProgressService;
    private readonly LevelConfigLibrary levelConfigLibrary;

    public int CurrentLevel => levelConfigLibrary.ClampLevelNumber(levelProgressService.CurrentLevel);
    public int MaxAvailableLevel => levelConfigLibrary.MaxLevelNumber;

    public LevelConfigProvider(
        ILevelProgressService levelProgressService,
        LevelConfigLibrary levelConfigLibrary)
    {
        this.levelProgressService = levelProgressService;
        this.levelConfigLibrary = levelConfigLibrary;
    }

    public LevelConfig GetCurrentConfig()
    {
        return levelConfigLibrary.GetConfig(CurrentLevel);
    }
}
