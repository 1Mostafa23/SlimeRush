using UnityEngine;

public class LevelGenerationRuntimeSettings
{
    public Transform GeneratedLevelRoot { get; }
    public bool GenerateOnStart { get; }
    public bool ClearRootBeforeGeneration { get; }

    public LevelGenerationRuntimeSettings(
        Transform generatedLevelRoot,
        bool generateOnStart,
        bool clearRootBeforeGeneration)
    {
        GeneratedLevelRoot = generatedLevelRoot;
        GenerateOnStart = generateOnStart;
        ClearRootBeforeGeneration = clearRootBeforeGeneration;
    }
}
