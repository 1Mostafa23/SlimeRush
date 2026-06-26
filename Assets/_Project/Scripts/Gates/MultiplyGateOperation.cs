using System;

public class MultiplyGateOperation : IGateMathOperation
{
    public GateOperationType OperationType => GateOperationType.Multiply;

    public int CalculateResultCount(int currentCount, int value)
    {
        return Math.Max(0, currentCount) * Math.Max(1, value);
    }

    public string GetDisplayText(int value)
    {
        return $"x{Math.Max(1, value)}";
    }
}
