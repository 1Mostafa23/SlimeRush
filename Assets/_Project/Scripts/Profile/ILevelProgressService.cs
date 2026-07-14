using System;

public interface ILevelProgressService
{
    int CurrentLevel { get; }

    event Action Changed;

    void SetCurrentLevel(int level);
    void AdvanceToNextLevel();
    void AdvanceToNextLevel(int maxAvailableLevel);
}
