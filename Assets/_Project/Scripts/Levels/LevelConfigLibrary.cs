using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "LevelConfigLibrary",
    menuName = "SlimeRush/Levels/Level Config Library")]
public class LevelConfigLibrary : ScriptableObject
{
    [SerializeField] private List<LevelConfig> levels = new();

    public int MaxLevelNumber
    {
        get
        {
            int maxLevelNumber = 1;

            for (int i = 0; i < levels.Count; i++)
            {
                if (levels[i] != null)
                    maxLevelNumber = Mathf.Max(maxLevelNumber, levels[i].LevelNumber);
            }

            return maxLevelNumber;
        }
    }

    public int ClampLevelNumber(int levelNumber)
    {
        if (levels.Count == 0)
            return Mathf.Max(1, levelNumber);

        return Mathf.Clamp(Mathf.Max(1, levelNumber), 1, MaxLevelNumber);
    }

    public LevelConfig GetConfig(int levelNumber)
    {
        if (levels.Count == 0)
            return null;

        int clampedLevelNumber = ClampLevelNumber(levelNumber);

        for (int i = 0; i < levels.Count; i++)
        {
            if (levels[i] != null && levels[i].LevelNumber == clampedLevelNumber)
                return levels[i];
        }

        LevelConfig fallback = null;

        for (int i = 0; i < levels.Count; i++)
        {
            if (levels[i] != null && levels[i].LevelNumber <= clampedLevelNumber)
                fallback = levels[i];
        }

        return fallback;
    }
}
