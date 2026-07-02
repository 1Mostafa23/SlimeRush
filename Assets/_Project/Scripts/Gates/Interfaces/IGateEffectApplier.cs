public interface IGateEffectApplier
{
    void Apply(GateOperationType operationType, int value);
    string GetDisplayText(GateOperationType operationType, int value);
}
