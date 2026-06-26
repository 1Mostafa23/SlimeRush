using System;

public class SubtractGateOperation : IGateMathOperation
{
    public GateOperationType OperationType => GateOperationType.Subtract;

    public int CalculateResultCount(int currentCount, int value)
    {
        return Math.Max(0, currentCount - Math.Max(0, value));
    }

    public string GetDisplayText(int value)
    {
        return $"-{Math.Max(0, value)}";
    }
}
