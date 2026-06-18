# SlimeRush Architecture TODO

## Current Status

- Zenject is added to the project.
- `Assets/GameplayPrototype.unity` has a `SceneContext`.
- `SceneContext` uses `GameplayInstaller`.
- `GameplayInstaller` has a scene reference to `SlimeCrowdManager`.
- `ISlimeCrowd` is extracted into a separate file.
- `CrowdCountLabel` depends on `ISlimeCrowd` through Zenject.
- `SlimeCrowdManager` implements `ISlimeCrowd`.

## Current Assessment

The first architecture step is correct:

- UI reads crowd state through an interface.
- The concrete `SlimeCrowdManager` is hidden behind `ISlimeCrowd` for read-only consumers.
- Zenject is used to bind the scene instance.

Main issue:

- `SlimeCrowdManager` currently has too many responsibilities:
  - stores slime instances;
  - creates and destroys slime GameObjects;
  - calculates formation positions;
  - applies positions to transforms;
  - exposes slime count state;
  - raises UI update events.

## Next Tasks

### 1. Split Read and Write Crowd Interfaces

Keep `ISlimeCrowd` as a read-only interface:

```csharp
public interface ISlimeCrowd
{
    int SlimeCount { get; }
    event Action<int> OnSlimeCountChanged;
}
```

Add a separate command interface:

```csharp
public interface ISlimeCrowdCommands
{
    void AddSlimes(int amount);
    void RemoveSlimes(int amount);
    void MultiplySlimes(int multiplier);
}
```

`SlimeCrowdManager` should implement both:

```csharp
public class SlimeCrowdManager : MonoBehaviour, ISlimeCrowd, ISlimeCrowdCommands
```

Zenject bindings:

```csharp
Container.Bind<SlimeCrowdManager>().FromInstance(slimeCrowdManager).AsSingle();
Container.Bind<ISlimeCrowd>().FromInstance(slimeCrowdManager).AsSingle();
Container.Bind<ISlimeCrowdCommands>().FromInstance(slimeCrowdManager).AsSingle();
```

Reason:

- UI only needs `ISlimeCrowd`.
- Gates, pickups, enemies, or level logic should use `ISlimeCrowdCommands`.
- This follows Interface Segregation Principle.

### 2. Extract Crowd Formation Calculation

Create:

- `ICrowdFormation`
- `EllipseCrowdFormation`

Target interface:

```csharp
using System.Collections.Generic;
using UnityEngine;

public interface ICrowdFormation
{
    IReadOnlyList<Vector3> GeneratePositions(int count);
}
```

Move these responsibilities out of `SlimeCrowdManager`:

- `GenerateCrowdPositions`
- `GetStableJitter`
- `ShiftFormationForward`
- formation settings if possible

Reason:

- `SlimeCrowdManager` should manage crowd state, not own formation math.
- Formation can later be changed without touching crowd logic.

### 3. Extract Slime Creation

Create:

- `ISlimeFactory`
- `SlimeFactory`

Possible interface:

```csharp
using UnityEngine;

public interface ISlimeFactory
{
    GameObject Create(Transform parent);
}
```

Move direct prefab instantiation out of `SlimeCrowdManager`:

```csharp
GameObject slime = Instantiate(slimePrefab, transform);
```

Reason:

- `SlimeCrowdManager` should not know how slimes are created.
- Later it will be easier to add pooling, Zenject prefab injection, or different slime types.

### 4. Improve CrowdCountLabel Injection

Current code uses field injection:

```csharp
[Inject] private ISlimeCrowd slimeCrowd;
```

Prefer method injection:

```csharp
[Inject]
private void Construct(ISlimeCrowd slimeCrowd)
{
    this.slimeCrowd = slimeCrowd;
}
```

Also review lifecycle:

- avoid relying on `OnEnable` if injection order becomes unclear;
- subscribe after dependency is injected;
- unsubscribe safely in `OnDestroy` or `OnDisable`.

Reason:

- Dependencies become explicit.
- Initialization order is easier to reason about.

### 5. Remove Slime Count Duplication

`PlayerCrowdController` currently has:

```csharp
public int GetSlimeCount()
{
    return transform.childCount;
}
```

This duplicates `SlimeCrowdManager.SlimeCount`.

Task:

- Remove this method if unused.
- If another system needs slime count, inject `ISlimeCrowd`.

Reason:

- There should be one source of truth for slime count.

### 6. Split Player Movement Later

`PlayerCrowdController` currently:

- reads touch/mouse input;
- calculates target X;
- moves forward;
- moves horizontally;
- controls start/stop movement.

Later split into:

- `IPlayerInput`
- `TouchPlayerInput`
- `PlayerMovement`
- optional `PlayerMovementController`

Reason:

- Input and movement are separate responsibilities.
- Movement becomes easier to test and tune.

### 7. Add Installer Validation

`GameplayInstaller` should guard against missing scene references.

Example:

```csharp
public override void InstallBindings()
{
    if (slimeCrowdManager == null)
    {
        Debug.LogError("GameplayInstaller: SlimeCrowdManager is not assigned.");
        return;
    }

    Container.Bind<SlimeCrowdManager>().FromInstance(slimeCrowdManager).AsSingle();
    Container.Bind<ISlimeCrowd>().FromInstance(slimeCrowdManager).AsSingle();
}
```

Reason:

- Missing inspector references should fail clearly.
- Zenject errors become easier to diagnose.

## Suggested Order For Tomorrow

1. Add `ISlimeCrowdCommands`.
2. Update `SlimeCrowdManager` to implement it.
3. Update `GameplayInstaller` bindings.
4. Update `CrowdCountLabel` injection style.
5. Extract `ICrowdFormation` and `EllipseCrowdFormation`.
6. Refactor `SlimeCrowdManager` to use `ICrowdFormation`.
7. Extract slime factory after formation is stable.

## Continue Prompt

Use this prompt tomorrow:

```text
продолжаем по ARCHITECTURE_TODO.md
```
