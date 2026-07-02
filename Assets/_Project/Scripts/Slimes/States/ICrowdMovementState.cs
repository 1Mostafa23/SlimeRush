using System.Collections.Generic;
using UnityEngine;

public interface ICrowdMovementState
{
    void Enter();
    void Tick(IReadOnlyList<GameObject> slimes, IReadOnlyList<Vector3> targetLocalPositions, float deltaTime);
    void Exit();
}
