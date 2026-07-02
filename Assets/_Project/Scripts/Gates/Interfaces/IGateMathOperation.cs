public interface IGateMathOperation
{
    GateOperationType OperationType { get; }
    int CalculateResultCount(int currentCount, int value);
    string GetDisplayText(int value);
}
