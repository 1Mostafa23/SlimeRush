using UnityEngine;

public class LevelProgressService : ILevelProgressService
{
    private readonly IPlayerProfileService profileService;

    public int CurrentLevel => Mathf.Max(1, profileService.Profile.currentLevel);

    public LevelProgressService(IPlayerProfileService profileService)
    {
        this.profileService = profileService;
    }

    public void AdvanceToNextLevel()
    {
        profileService.Profile.currentLevel = CurrentLevel + 1;
        profileService.Save();
    }

    public void AdvanceToNextLevel(int maxAvailableLevel)
    {
        int clampedMaxLevel = Mathf.Max(1, maxAvailableLevel);
        profileService.Profile.currentLevel = Mathf.Min(CurrentLevel + 1, clampedMaxLevel);
        profileService.Save();
    }
}
