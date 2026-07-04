using System.Collections.Generic;
using UnityEngine;

public interface IEnemyLaneSelector
{
    Transform SelectLane(IReadOnlyList<Transform> lanes);
}
