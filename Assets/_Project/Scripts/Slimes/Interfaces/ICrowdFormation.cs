using System.Collections.Generic;
using UnityEngine;

public interface ICrowdFormation
{
    IReadOnlyList<Vector3> GeneratePositions(int count);
}
