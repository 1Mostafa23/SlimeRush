using System.Collections.Generic;

public interface ICrowdRowAllocator
{
    IReadOnlyList<CrowdRowAllocation> AllocateRows(int slimeCount);
}
