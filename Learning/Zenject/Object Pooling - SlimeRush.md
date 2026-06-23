# Object Pooling - SlimeRush

## Main Idea

Object pooling means reusing objects instead of constantly creating and destroying them.

Without pooling:

```text
Need slime -> Instantiate new slime
Remove slime -> Destroy slime
Need slime again -> Instantiate another slime
```

With pooling:

```text
Need slime -> take inactive slime from pool
Remove slime -> deactivate slime and return it to pool
Need slime again -> reuse old slime
```

The object is not destroyed.

It is hidden, reset, and reused later.

## Why Pooling Matters

`Instantiate` and `Destroy` are not free.

They can cause:

- memory allocations;
- CPU spikes;
- delayed cleanup;
- extra garbage collector work;
- frame drops on mobile.

For a few objects, this is usually fine.

For many objects created and removed during gameplay, pooling is often better.

In SlimeRush, slimes are a good pooling candidate because:

- there can be many slimes;
- slimes are added and removed during gameplay;
- removed slimes may be needed again later;
- Android performance matters;
- the slime prefab is already created through a factory.

## Pooling Is Not Addressables

Addressables and pooling solve different problems.

Addressables:

```text
How do we load the prefab asset?
```

Pooling:

```text
How do we reuse GameObject instances?
```

Current project:

```text
Addressables load SlimePrefab once.
Factory uses Object.Instantiate to create every slime.
Destroy removes every slime.
```

Future project:

```text
Addressables load SlimePrefab once.
Pool keeps reusable slime instances.
Factory returns slimes from the pool.
Removed slimes go back into the pool.
```

## Current Architecture Before Pooling

Current chain:

```text
SlimeCrowdManager
    -> ISlimeFactory
    -> AddressableSlimeFactory
    -> Addressables.LoadAssetAsync("SlimePrefab")
    -> Object.Instantiate(loadedSlimePrefab)
```

Current remove logic:

```csharp
slimes.RemoveAt(lastIndex);
Destroy(slimeToRemove);
```

This works, but it destroys the object.

Pooling changes that last line.

Instead of:

```csharp
Destroy(slimeToRemove);
```

we want:

```csharp
slimeFactory.Release(slimeToRemove);
```

## How Pooling Changes the Factory Interface

Current `ISlimeFactory`:

```csharp
public interface ISlimeFactory
{
    UniTask InitializeAsync(CancellationToken cancellationToken);
    GameObject Create(Transform parent);
}
```

With pooling:

```csharp
public interface ISlimeFactory
{
    UniTask InitializeAsync(CancellationToken cancellationToken);
    GameObject Create(Transform parent);
    void Release(GameObject slime);
}
```

`Create` means:

```text
Give me an active slime.
```

`Release` means:

```text
I no longer need this slime. Store it for reuse.
```

## Simple Pool Behavior

A slime pool can store inactive objects in a stack:

```csharp
private readonly Stack<GameObject> inactiveSlimes = new();
```

Create logic:

```text
If inactiveSlimes has objects:
    take one
    set parent
    activate
    return it

Otherwise:
    instantiate a new slime
    return it
```

Release logic:

```text
deactivate slime
detach or move it under pool root
reset transform/state
push it into inactiveSlimes
```

## Prewarming

Prewarming means creating some objects before gameplay needs them.

Example:

```text
At startup:
    load prefab
    create 20 inactive slimes
    put them into pool

During gameplay:
    first 20 AddSlimes calls reuse existing objects
```

Prewarming reduces spikes during gameplay.

But prewarming too many objects wastes memory.

For SlimeRush, a reasonable first prewarm could be:

```text
startingSlimeCount
```

or a small fixed number:

```text
20
```

## What Must Be Reset

When an object returns to a pool, it may still have old state.

For a slime, reset at least:

- active state;
- parent;
- local position;
- local rotation;
- local scale if gameplay can change it.

Later, if slimes get more behavior, also reset:

- animation state;
- VFX;
- trail renderers;
- health;
- temporary buffs;
- collision state;
- custom scripts.

This is one of the main risks of pooling:

```text
Objects can accidentally remember old state.
```

## Who Owns What

Ownership must be clear.

When a slime is active:

```text
SlimeCrowdManager owns it in the active slimes list.
```

When a slime is inactive:

```text
The pool owns it.
```

A slime should not be both:

```text
active list item and inactive pool item
```

That would create bugs.

## Pooling and SOLID

### Single Responsibility Principle

Without pooling/factory separation:

```text
SlimeCrowdManager manages crowd and creates/destroys objects.
```

With pooling:

```text
SlimeCrowdManager manages crowd.
Factory/pool manages object lifecycle.
```

### Dependency Inversion Principle

`SlimeCrowdManager` still depends on:

```text
ISlimeFactory
```

It does not depend on:

```text
Stack<GameObject>
Addressables
Object.Instantiate
Destroy
```

### Open/Closed Principle

The factory implementation can change:

```text
SlimeFactory
AddressableSlimeFactory
PooledAddressableSlimeFactory
```

`SlimeCrowdManager` only changes if the interface changes.

For pooling, adding `Release` to `ISlimeFactory` is a reasonable interface change because the manager must tell the factory when a slime is no longer needed.

## Minimal Implementation Plan

Do this in small steps:

1. Add `Release(GameObject slime)` to `ISlimeFactory`.
2. Add `Release` to simple `SlimeFactory` so it still works.
3. Add `Release` to `AddressableSlimeFactory`.
4. Add an inactive slime stack inside `AddressableSlimeFactory`.
5. Change `Create`:
   - use inactive slime if available;
   - otherwise instantiate a new slime.
6. Change `RemoveSlimes`:
   - remove slime from active list;
   - call `slimeFactory.Release(slimeToRemove)`;
   - do not call `Destroy`.
7. Optionally prewarm the pool during `InitializeAsync`.
8. Test add/remove/multiply.

## Minimal First Version

The first version does not need to be perfect.

It can be:

```text
No max pool size.
No complex reset interface.
No separate generic pool class.
No Zenject MemoryPool yet.
```

A simple slime-specific pool is enough for learning.

Later, if many systems need pooling, then it may be worth creating:

```text
IObjectPool<T>
SlimePool
EnemyPool
ProjectilePool
```

or using Zenject's built-in memory pools.

## Common Mistakes

### Mistake 1: Returning active objects twice

Problem:

```text
Release is called twice on the same slime.
```

Result:

```text
The same slime can exist twice in the inactive pool.
```

Avoid by keeping ownership clear.

### Mistake 2: Forgetting to deactivate

Problem:

```text
Released slime stays active in the scene.
```

Result:

```text
It may still render, collide, or run scripts.
```

### Mistake 3: Forgetting to reset transform

Problem:

```text
Old position/rotation remains.
```

Result:

```text
Reused slime appears in the wrong place for one frame or more.
```

### Mistake 4: Pooling too early everywhere

Do not pool every object.

Pool objects that are:

- created often;
- destroyed often;
- expensive;
- numerous;
- performance-sensitive.

Slimes qualify.

Tiny temporary data objects usually do not.

## Short Version

```text
Addressables load the prefab.
Factory creates or reuses slime objects.
Pool stores inactive slime objects.
SlimeCrowdManager only manages active crowd state.
```

Current:

```text
RemoveSlimes -> Destroy
```

Next:

```text
RemoveSlimes -> slimeFactory.Release
```

This is the key step toward reducing runtime allocations and garbage collector pressure.

