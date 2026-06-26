public class GateEffectApplier
{
    private readonly GateOperationResolver gateOperationResolver;
    private readonly CrowdCountChangeApplier crowdCountChangeApplier;

    public GateEffectApplier(
        GateOperationResolver gateOperationResolver,
        CrowdCountChangeApplier crowdCountChangeApplier)
    {
        this.gateOperationResolver = gateOperationResolver;
        this.crowdCountChangeApplier = crowdCountChangeApplier;
    }

    public void Apply(GateOperationType operationType, int value)
    {
        IGateMathOperation operation = gateOperationResolver.Resolve(operationType);
        crowdCountChangeApplier.Apply(operation, value);
    }

    public string GetDisplayText(GateOperationType operationType, int value)
    {
        IGateMathOperation operation = gateOperationResolver.Resolve(operationType);
        return operation.GetDisplayText(value);
    }
}
