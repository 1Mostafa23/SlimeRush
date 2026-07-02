using System.Collections.Generic;
using UnityEngine;

public class CrowdMovementStateMachine : ICrowdMovementStateMachine
{
    private ICrowdMovementState currentState;

    public void ChangeState(ICrowdMovementState nextState)
    {
        if (nextState == null || currentState == nextState)
            return;

        currentState?.Exit();
        currentState = nextState;
        currentState.Enter();
    }

    public void Tick(IReadOnlyList<GameObject> slimes, IReadOnlyList<Vector3> targetLocalPositions, float deltaTime)
    {
        currentState?.Tick(slimes, targetLocalPositions, deltaTime);
    }
}
