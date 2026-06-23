# Gate Gameplay Logic

## Goal

Implement gate gameplay logic for the existing SlimeRush Unity project.

Gates must affect the whole crowd through a single `PlayerCrowd` collider.

Do not add individual slime collision logic in this task.

## Architecture Rules

- `SlimeCrowdManager` implements `ISlimeCrowd` and `ISlimeCrowdCommands`.
- Gate logic must not depend directly on `SlimeCrowdManager`.
- Gate logic should use `ISlimeCrowdCommands`.
- Use Zenject bindings from `GameplayInstaller`.
- Do not implement traps in this task.
- Do not implement per-slime collision in this task.

## Open Tasks

1. Final physics setup check

   Verify:

   ```text
   PlayerCrowd has a shared BoxCollider
   PlayerCrowd has a kinematic Rigidbody
   Gate_Add_10 root collider is trigger
   Gate_Multiply_2 root collider is trigger
   Individual slime colliders are not used for gate logic
   ```

2. Create `GateOperationType`

   Values:

   ```csharp
   Add
   Multiply
   Subtract
   ```

3. Create `IGateMathOperation`

   Contract for pure gate math.

   This should not know about Unity, Zenject, colliders, or `SlimeCrowdManager`.

4. Create operation implementations

   Files:

   ```text
   AddGateOperation.cs
   MultiplyGateOperation.cs
   SubtractGateOperation.cs
   ```

5. Create `GateOperationResolver`

   It should resolve:

   ```text
   Add -> AddGateOperation
   Multiply -> MultiplyGateOperation
   Subtract -> SubtractGateOperation
   ```

6. Create `CrowdCountChangeApplier`

   Responsibilities:

   ```text
   receive ISlimeCrowdCommands through Zenject
   apply AddSlimes / MultiplySlimes / RemoveSlimes
   never depend directly on SlimeCrowdManager
   ```

7. Create `GateEffectApplier`

   Responsibilities:

   ```text
   receive operation type and value
   use GateOperationResolver
   use CrowdCountChangeApplier
   apply the selected gate effect to the crowd
   ```

8. Create `GateVisualView`

   MonoBehaviour for gate visuals.

   Responsibilities:

   ```text
   display +10
   display x2
   display -5
   no gameplay logic
   ```

9. Create `GateView`

   MonoBehaviour for the root gate object.

   Responsibilities:

   ```text
   expose GateOperationType in Inspector
   expose int value in Inspector
   receive GateEffectApplier through Zenject
   update visual label
   react only to PlayerCrowd collider
   apply effect once
   disable gate after use
   ```

10. Update `GameplayInstaller`

    Add bindings for:

    ```text
    AddGateOperation
    MultiplyGateOperation
    SubtractGateOperation
    GateOperationResolver
    CrowdCountChangeApplier
    GateEffectApplier
    ```

11. Configure gates in Unity

    `Gate_Add_10`:

    ```text
    add GateView
    operation = Add
    value = 10
    assign visual view
    ```

    `Gate_Multiply_2`:

    ```text
    add GateView
    operation = Multiply
    value = 2
    assign visual view
    ```

12. Test in Play Mode

    Verify:

    ```text
    +10 adds slimes
    x2 multiplies slimes
    gates trigger once
    gates disable after use
    individual slime colliders do not trigger gate effect
    Console has no Zenject errors
    ```

13. Later separate tasks

    ```text
    enemy that removes slimes
    subtract gate prefab if needed
    prefab workflow for future levels
    ```

