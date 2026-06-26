using System;

public class GateOperationResolver
{
    private readonly AddGateOperation addGateOperation;
    private readonly MultiplyGateOperation multiplyGateOperation;
    private readonly SubtractGateOperation subtractGateOperation;

    public GateOperationResolver(
        AddGateOperation addGateOperation,
        MultiplyGateOperation multiplyGateOperation,
        SubtractGateOperation subtractGateOperation)
    {
        this.addGateOperation = addGateOperation;
        this.multiplyGateOperation = multiplyGateOperation;
        this.subtractGateOperation = subtractGateOperation;
    }

    public IGateMathOperation Resolve(GateOperationType operationType)
    {
        return operationType switch
        {
            GateOperationType.Add => addGateOperation,
            GateOperationType.Multiply => multiplyGateOperation,
            GateOperationType.Subtract => subtractGateOperation,
            _ => throw new ArgumentOutOfRangeException(nameof(operationType), operationType, null)
        };
    }
}
