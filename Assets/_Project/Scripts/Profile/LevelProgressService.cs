using System;
using UnityEngine;

public class LevelProgressService : ILevelProgressService
{
    private readonly IPlayerProfileService profileService;

    public int CurrentLevel => Mathf.Max(1, profileService.Profile.currentLevel);

    public event Action Changed;

    public LevelProgressService(IPlayerProfileService profileService)
    {
        this.profileService = profileService;
    }

    public void AdvanceToNextLevel()
    {
        SetCurrentLevel(CurrentLevel + 1);
    }

    public void AdvanceToNextLevel(int maxAvailableLevel)
    {
        int clampedMaxLevel = Mathf.Max(1, maxAvailableLevel);
        SetCurrentLevel(Mathf.Min(CurrentLevel + 1, clampedMaxLevel));
    }

    public void SetCurrentLevel(int level)
    {
        profileService.Profile.currentLevel = Mathf.Max(1, level);
        profileService.Save();
        Changed?.Invoke();
    }
}
