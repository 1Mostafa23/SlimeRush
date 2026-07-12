using System;

public interface IRunResultService
{
    event Action<RunResultData> RunCompleted;

    void CompleteRun();
}
