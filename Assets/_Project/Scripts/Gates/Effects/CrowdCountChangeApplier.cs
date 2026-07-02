using System;

public class CrowdCountChangeApplier
{
    private readonly ISlimeCrowdCommands slimeCrowdCommands;

    public CrowdCountChangeApplier(ISlimeCrowdCommands slimeCrowdCommands)
    {
        this.slimeCrowdCommands = slimeCrowdCommands;
    }

    public void Apply(IGateMathOperation operation, int value)
    {
        switch (operation.OperationType)
        {
            case GateOperationType.Add:
                slimeCrowdCommands.AddSlimes(value);
                break;
            case GateOperationType.Multiply:
                slimeCrowdCommands.MultiplySlimes(value);
                break;
            case GateOperationType.Subtract:
                slimeCrowdCommands.RemoveSlimes(value);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
