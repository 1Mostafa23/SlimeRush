# Factories - When and How to Use

## Main Idea

A factory is an object whose job is to create other objects.

In Unity and Zenject projects, factories are useful when object creation becomes an important part of the architecture, not just a small local detail.

Simple version:

```csharp
public interface ISlimeFactory
{
    UniTask InitializeAsync(CancellationToken cancellationToken);
    GameObject Create(Transform parent);
}
```

The class that needs a slime does not need to know how the slime is created.

It only asks:

```csharp
await slimeFactory.InitializeAsync(cancellationToken);
GameObject slime = slimeFactory.Create(transform);
```

The factory decides how to instantiate the prefab.

## The Problem Without a Factory

Right now `SlimeCrowdManager` does several jobs:

- stores the list of slimes;
- adds slimes;
- removes slimes;
- rearranges the crowd;
- notifies UI about count changes;
- creates slime GameObjects with `Instantiate`;
- destroys slime GameObjects with `Destroy`.

Example:

```csharp
GameObject slime = Instantiate(slimePrefab, transform);
slimes.Add(slime);
```

This works. For a small project, this is acceptable.

But architecturally, `SlimeCrowdManager` now knows too much:

- it knows there is a prefab;
- it knows how to instantiate the prefab;
- it knows who the parent transform is;
- later it may need to know about pooling;
- later it may need to know about Zenject prefab injection.

The more creation logic grows, the less `SlimeCrowdManager` remains only a crowd manager.

## What a Factory Solves

A factory moves object creation out of the class that uses the object.

Instead of this:

```csharp
public class SlimeCrowdManager : MonoBehaviour
{
    [SerializeField] private GameObject slimePrefab;

    private void AddOneSlime()
    {
        GameObject slime = Instantiate(slimePrefab, transform);
    }
}
```

We move creation into another class:

```csharp
public interface ISlimeFactory
{
    UniTask InitializeAsync(CancellationToken cancellationToken);
    GameObject Create(Transform parent);
}
```

```csharp
public class SlimeFactory : ISlimeFactory
{
    private readonly GameObject slimePrefab;

    public SlimeFactory(GameObject slimePrefab)
    {
        this.slimePrefab = slimePrefab;
    }

    public UniTask InitializeAsync(CancellationToken cancellationToken)
    {
        return UniTask.CompletedTask;
    }

    public GameObject Create(Transform parent)
    {
        return Object.Instantiate(slimePrefab, parent);
    }
}
```

Then `SlimeCrowdManager` depends on the interface:

```csharp
public class SlimeCrowdManager : MonoBehaviour
{
    private ISlimeFactory slimeFactory;

    [Inject]
    private void Construct(ISlimeFactory slimeFactory)
    {
        this.slimeFactory = slimeFactory;
    }

    private void AddOneSlime()
    {
        GameObject slime = slimeFactory.Create(transform);
    }
}
```

Now `SlimeCrowdManager` does not create slimes directly.

It only requests a slime.

## When to Use a Factory

Use a factory when creation is not just a small technical detail.

Good reasons to use a factory:

- the object is created many times during gameplay;
- the object has dependencies from Zenject;
- the object is created from a prefab;
- creation has rules or parameters;
- creation may later change from `Instantiate` to pooling;
- many classes need to create the same kind of object;
- you want the main class to depend on an interface, not on prefab creation details.

Example cases:

- spawning slimes;
- spawning enemies;
- spawning bullets;
- spawning pickups;
- creating UI popups;
- creating level chunks;
- creating effects from prefabs.

## When Not to Use a Factory

Do not create a factory for every `new`.

This is normal:

```csharp
List<Vector3> positions = new();
```

This is also normal:

```csharp
Vector3 position = new Vector3(0f, 0f, 1f);
```

These are local technical objects. They are not architectural dependencies.

A factory would be unnecessary noise here.

Bad example:

```csharp
public interface IVector3Factory
{
    Vector3 Create(float x, float y, float z);
}
```

This does not help the architecture.

It only makes simple code harder to read.

## Factory vs `new`

`new` is fine when the object is local and simple.

```csharp
List<Vector3> positions = new();
```

`new` is suspicious when the object is a meaningful dependency.

```csharp
crowdFormation = new EllipseCrowdFormation(settings);
```

This is why `ICrowdFormation` is injected through Zenject.

`Instantiate` is also suspicious when the object is an important gameplay object.

```csharp
GameObject slime = Instantiate(slimePrefab, transform);
```

This is not always wrong, but it is a sign that a factory may become useful.

## Applying This to SlimeRush

Current situation:

```text
SlimeCrowdManager
    owns slime list
    changes slime count
    asks ISlimeFactory to create slimes
    destroys slime objects
    rearranges formation
```

Current structure:

```text
GameplayInstaller
    binds SlimePrefabAddress("SlimePrefab")
    binds ISlimeFactory -> AddressableSlimeFactory
    binds ISlimeCrowd -> SlimeCrowdManager
    binds ISlimeCrowdCommands -> SlimeCrowdManager

SlimeCrowdManager
    owns slime list
    changes slime count
    waits for slime factory initialization
    asks ISlimeFactory to create slimes
    rearranges formation

AddressableSlimeFactory
    loads the slime prefab through Addressables
    stores the loaded prefab
    creates slime GameObjects from the loaded prefab

EllipseCrowdFormation
    calculates positions
```

The important part:

`SlimeCrowdManager` should not care whether slimes are created with:

- `Object.Instantiate`;
- Zenject prefab instantiation;
- object pooling;
- another prefab;
- a fake test object.

It should only care that it gets a slime object back.

## Simple Non-Zenject Factory

This is the simplest factory:

```csharp
using UnityEngine;

public interface ISlimeFactory
{
    UniTask InitializeAsync(CancellationToken cancellationToken);
    GameObject Create(Transform parent);
}
```

```csharp
using UnityEngine;

public class SlimeFactory : ISlimeFactory
{
    private readonly GameObject slimePrefab;

    public SlimeFactory(GameObject slimePrefab)
    {
        this.slimePrefab = slimePrefab;
    }

    public UniTask InitializeAsync(CancellationToken cancellationToken)
    {
        return UniTask.CompletedTask;
    }

    public GameObject Create(Transform parent)
    {
        return Object.Instantiate(slimePrefab, parent);
    }
}
```

Installer:

```csharp
public class GameplayInstaller : MonoInstaller
{
    [SerializeField] private GameObject slimePrefab;

    public override void InstallBindings()
    {
        Container.BindInstance(slimePrefab).WhenInjectedInto<SlimeFactory>();

        Container.Bind<ISlimeFactory>()
            .To<SlimeFactory>()
            .AsSingle();
    }
}
```

This version is easy to understand.

The factory still uses normal Unity `Object.Instantiate`.

The value is that `SlimeCrowdManager` no longer knows about the prefab.

In the current project, this simple factory was used as an intermediate learning step.

The project now uses `AddressableSlimeFactory` instead.

## Current Addressable Factory

The current project has this chain:

```text
SlimeCrowdManager
    -> ISlimeFactory
    -> AddressableSlimeFactory
    -> Addressables.LoadAssetAsync("SlimePrefab")
    -> Object.Instantiate(loadedSlimePrefab)
```

The important design decision:

```text
Do async loading once.
Keep Create synchronous after preload.
```

That is why `ISlimeFactory` has two responsibilities:

```csharp
UniTask InitializeAsync(CancellationToken cancellationToken);
GameObject Create(Transform parent);
```

`InitializeAsync` prepares the factory.

`Create` creates one slime after the factory is ready.

## Zenject Prefab Factory

Zenject also has its own factory system.

This is useful when spawned prefab components need `[Inject]`.

Example:

```csharp
public class SlimeView : MonoBehaviour
{
    public class Factory : PlaceholderFactory<Transform, SlimeView>
    {
    }
}
```

Installer:

```csharp
Container.BindFactory<Transform, SlimeView, SlimeView.Factory>()
    .FromComponentInNewPrefab(slimePrefab)
    .AsSingle();
```

Usage:

```csharp
private SlimeView.Factory slimeFactory;

[Inject]
private void Construct(SlimeView.Factory slimeFactory)
{
    this.slimeFactory = slimeFactory;
}

private void AddOneSlime()
{
    SlimeView slime = slimeFactory.Create(transform);
}
```

This approach is more Zenject-specific.

Use it when spawned objects are real Zenject participants and need dependencies injected into them.

## Factory and Object Pooling

A factory creates objects.

A pool reuses objects.

At first, a slime factory may do this:

```csharp
return Object.Instantiate(slimePrefab, parent);
```

Later, if creating and destroying objects becomes expensive, the implementation can change:

```csharp
return pool.Spawn(parent);
```

If `SlimeCrowdManager` depends only on `ISlimeFactory`, it does not care.

The manager code can stay almost the same.

This is one of the main benefits of the factory pattern.

For SlimeRush, pooling is the next logical improvement because the game may have many slimes.

Addressables already solved loading the prefab.

Pooling would solve repeated creation and destruction of slime GameObjects.

Current behavior:

```text
AddSlimes -> Instantiate
RemoveSlimes -> Destroy
```

Future pooled behavior:

```text
AddSlimes -> get inactive slime from pool
RemoveSlimes -> return slime to pool
```

This reduces runtime allocations and work for the garbage collector.

## SOLID Connection

### Single Responsibility Principle

Without factory:

```text
SlimeCrowdManager manages crowd and creates prefabs.
```

With factory:

```text
SlimeCrowdManager manages crowd.
SlimeFactory creates slimes.
```

Each class has a clearer responsibility.

### Dependency Inversion Principle

Without factory:

```text
SlimeCrowdManager -> slimePrefab / Instantiate
```

With factory:

```text
SlimeCrowdManager -> ISlimeFactory
SlimeFactory -> slimePrefab / Instantiate
```

The high-level class depends on an abstraction.

### Open/Closed Principle

If creation changes, you can create another factory implementation:

```csharp
public class PooledSlimeFactory : ISlimeFactory
{
    public GameObject Create(Transform parent)
    {
        // get slime from pool
    }
}
```

Then change the binding:

```csharp
Container.Bind<ISlimeFactory>()
    .To<PooledSlimeFactory>()
    .AsSingle();
```

`SlimeCrowdManager` does not need to change.

## A Practical Rule

Ask this question:

```text
If I change how this object is created, should this class care?
```

If the answer is no, consider a factory.

For `SlimeCrowdManager`:

```text
If I change slime creation from Instantiate to pooling, should SlimeCrowdManager care?
```

No.

So a factory is a good next architectural step.

## Recommended Learning Path for This Project

Do not add all factory patterns at once.

Use this order:

1. Create `ISlimeFactory`.
2. Create simple `SlimeFactory` that uses `Object.Instantiate`.
3. Bind `ISlimeFactory` in `GameplayInstaller`.
4. Inject `ISlimeFactory` into `SlimeCrowdManager`.
5. Remove `slimePrefab` from `SlimeCrowdManager`.
6. Install Addressables.
7. Mark the slime prefab as Addressable with key `SlimePrefab`.
8. Install UniTask.
9. Create `AddressableSlimeFactory`.
10. Preload the slime prefab once.
11. Later, add pooling.

Steps 1-10 are done in this project.

Step 11 is next.

The goal is not to use patterns everywhere.

The goal is to move creation logic out of classes that should focus on gameplay rules.

## Short Version

```text
new List<T>() inside a method - normal
new Vector3(...) - normal
Object.Instantiate(slimePrefab) inside SlimeCrowdManager - acceptable now, but factory is better later
new EllipseCrowdFormation(...) inside SlimeCrowdManager - bad, use DI
ISlimeFactory injected into SlimeCrowdManager - good
AddressableSlimeFactory with preload - good
Zenject factory - good when spawned prefabs need injection
pool factory - good when many objects are spawned and destroyed often
```

## Key Takeaway

A factory is useful when object creation is a decision you may want to change later.

For SlimeRush, slimes are a good candidate for a factory because they are gameplay objects created many times, and their creation may later involve Zenject injection or pooling.
