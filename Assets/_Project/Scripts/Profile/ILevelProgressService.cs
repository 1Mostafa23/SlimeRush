public interface ILevelProgressService
{
    int CurrentLevel { get; }

    void AdvanceToNextLevel();
    void AdvanceToNextLevel(int maxAvailableLevel);
}
