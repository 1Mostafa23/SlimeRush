public interface ILevelConfigProvider
{
    int CurrentLevel { get; }
    int MaxAvailableLevel { get; }

    LevelConfig GetCurrentConfig();
}
