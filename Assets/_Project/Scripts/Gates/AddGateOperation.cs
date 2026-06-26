using System;

public class AddGateOperation : IGateMathOperation
{
    public GateOperationType OperationType => GateOperationType.Add;

    public int CalculateResultCount(int currentCount, int value)
    {
        return Math.Max(0, currentCount) + Math.Max(0, value);
    }

    public string GetDisplayText(int value)
    {
        return $"+{Math.Max(0, value)}";
    }
}
