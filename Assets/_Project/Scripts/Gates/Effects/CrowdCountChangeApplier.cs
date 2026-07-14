using System;

public class CrowdCountChangeApplier
{
    private readonly ISlimeCrowdCommands slimeCrowdCommands;
    private readonly ISfxService sfxService;

    public CrowdCountChangeApplier(ISlimeCrowdCommands slimeCrowdCommands, ISfxService sfxService)
    {
        this.slimeCrowdCommands = slimeCrowdCommands;
        this.sfxService = sfxService;
    }

    public void Apply(IGateMathOperation operation, int value)
    {
        switch (operation.OperationType)
        {
            case GateOperationType.Add:
                slimeCrowdCommands.AddSlimes(value);
                if (value > 0)
                    sfxService.PlaySlimeIncrease();
                break;
            case GateOperationType.Multiply:
                slimeCrowdCommands.MultiplySlimes(value);
                if (value > 1)
                    sfxService.PlaySlimeIncrease();
                break;
            case GateOperationType.Subtract:
                slimeCrowdCommands.RemoveSlimes(value);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
