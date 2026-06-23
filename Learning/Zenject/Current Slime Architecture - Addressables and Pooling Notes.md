# Current Slime Architecture - Addressables and Pooling Notes

## Current State

The project now has this runtime chain:

```text
GameplayInstaller
    binds SlimePrefabAddress("SlimePrefab")
    binds ISlimeFactory -> AddressableSlimeFactory
    binds CrowdFormationSettings
    binds ICrowdFormation -> EllipseCrowdFormation
    binds ISlimeCrowd -> SlimeCrowdManager
    binds ISlimeCrowdCommands -> SlimeCrowdManager

SlimeCrowdManager
    receives ICrowdFormation
    receives ISlimeFactory
    waits for ISlimeFactory.InitializeAsync
    owns slime list
    applies generated positions

AddressableSlimeFactory
    loads addressable prefab "SlimePrefab"
    stores loaded prefab
    creates slime GameObjects
    releases Addressables handle on dispose
```

## What Each Part Solves

`SlimeCrowdManager` solves gameplay state:

- how many slimes exist;
- adding slimes;
- removing slimes;
- multiplying slimes;
- rearranging positions;
- notifying UI about count changes.

`EllipseCrowdFormation` solves formation math:

- how to calculate local positions;
- how to make the crowd look organic;
- how to keep the formation in front of the player.

`AddressableSlimeFactory` solves asset loading and object creation:

- where the slime prefab comes from;
- how to load it through Addressables;
- how to create slime instances after preload.

## Why `SlimeCrowdManager` Does Not Use Addressables Directly

Bad direction:

```csharp
Addressables.LoadAssetAsync<GameObject>("SlimePrefab");
```

inside `SlimeCrowdManager`.

That would mix:

- crowd state;
- formation;
- asset loading;
- async lifecycle;
- prefab creation.

Current direction:

```text
SlimeCrowdManager -> ISlimeFactory
```

The manager only knows that a factory can create a slime.

It does not know if the factory uses:

- direct prefab reference;
- Addressables;
- Zenject factory;
- pooling;
- fake test object.

## Why Factory Initialization Is Async

Addressables loading is asynchronous.

The prefab is not available immediately.

So the factory has an initialization step:

```csharp
UniTask InitializeAsync(CancellationToken cancellationToken);
```

Then object creation stays synchronous:

```csharp
GameObject Create(Transform parent);
```

This means:

```text
Load once.
Create many times.
```

## Current Important Rule

```text
Addressables load the prefab.
Factory creates objects from the loaded prefab.
SlimeCrowdManager manages gameplay.
```

These responsibilities should stay separate.

## Why Pooling Is Next

Addressables solve loading.

They do not solve the cost of constantly creating and destroying GameObjects.

Current behavior:

```text
AddSlimes
    Object.Instantiate

RemoveSlimes
    Destroy
```

For a small number of slimes, this is acceptable.

For many slimes, this can become expensive:

- `Instantiate` allocates memory;
- `Destroy` delays cleanup;
- garbage collector may run more often;
- spikes may appear on mobile;
- Android performance can suffer.

Pooling changes this.

Future behavior:

```text
AddSlimes
    take inactive slime from pool
    activate it
    put it into active slime list

RemoveSlimes
    remove slime from active slime list
    deactivate it
    return it to pool
```

## Pooling Is Not the Same as Addressables

Addressables:

```text
How do we load the prefab asset?
```

Pooling:

```text
How do we reuse already created GameObjects?
```

They work well together:

```text
Addressables load SlimePrefab once.
Pool creates and stores slime instances.
SlimeCrowdManager asks the factory for slimes.
Factory/pool returns reusable slime objects.
```

## Is Pooling Difficult?

The idea is simple.

The difficulty is in ownership rules.

You need clear answers:

- who owns inactive slimes;
- who owns active slimes;
- when a slime is returned to the pool;
- whether returned slimes reset position, rotation, scale, effects, animation, and state;
- how many slimes are prewarmed;
- what happens if the pool is empty.

For this project, the first pool can be simple:

```text
Preload addressable prefab.
Prewarm a small number of slime instances.
On AddSlimes, get from pool or create if empty.
On RemoveSlimes, deactivate and return to pool.
```

## Recommended Next Step

Do not add a complex pooling system immediately.

Start with a small slime-specific pool:

```text
ISlimeFactory
    InitializeAsync
    Create
    Release
```

Then `SlimeCrowdManager` can stop calling `Destroy` directly.

Instead of:

```csharp
Destroy(slimeToRemove);
```

it will call:

```csharp
slimeFactory.Release(slimeToRemove);
```

That is the key architecture change.

