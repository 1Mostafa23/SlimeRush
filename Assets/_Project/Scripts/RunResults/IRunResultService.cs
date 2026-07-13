using System;

public interface IRunResultService
{
    bool IsCompleted { get; }

    event Action<RunResultData> RunCompleted;

    void CompleteRun();
}
