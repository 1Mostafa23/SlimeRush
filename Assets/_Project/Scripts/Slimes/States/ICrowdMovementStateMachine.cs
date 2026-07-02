using System.Collections.Generic;
using UnityEngine;

public interface ICrowdMovementStateMachine
{
    void ChangeState(ICrowdMovementState nextState);
    void Tick(IReadOnlyList<GameObject> slimes, IReadOnlyList<Vector3> targetLocalPositions, float deltaTime);
}
