# Addressables with Factories - SlimeRush

## Main Idea

Addressables are a Unity system for loading assets by an address instead of keeping a direct reference to the asset everywhere.

Without Addressables:

```csharp
[SerializeField] private GameObject slimePrefab;
```

With Addressables:

```text
"SlimePrefab" -> Unity loads the prefab by address
```

The class does not need a direct prefab reference in the scene.

## Why This Matters

In a small scene, a direct prefab reference is fine.

But as the project grows, direct references can become limiting:

- every scene may need to reference the same prefabs;
- heavy assets may load too early;
- replacing assets can become messy;
- remote content is harder to support;
- dynamic spawning becomes mixed with loading logic.

Addressables solve the loading side.

Factories solve the creation side.

Together:

```text
SlimeCrowdManager -> ISlimeFactory -> AddressableSlimeFactory -> Addressables
```

`SlimeCrowdManager` should not know that Addressables exist.

## Current Project Status

Current project state:

```text
com.unity.addressables - installed
com.cysharp.unitask - installed
slime prefab address - SlimePrefab
ISlimeFactory - exists
AddressableSlimeFactory - exists
```

The slime prefab is marked as Addressable and lives in the default local Addressables group.

The current runtime chain is:

```text
GameplayInstaller
    binds SlimePrefabAddress("SlimePrefab")
    binds ISlimeFactory -> AddressableSlimeFactory

SlimeCrowdManager
    receives ISlimeFactory
    waits for factory initialization
    asks factory to create slimes

AddressableSlimeFactory
    loads "SlimePrefab" once
    stores the loaded prefab
    creates slime instances from the loaded prefab
    releases Addressables handle on dispose
```

## What Addressables Should Be Used For

Use Addressables for assets that:

- are loaded dynamically;
- are spawned during gameplay;
- may be shared between levels;
- may become heavy;
- may later come from remote content;
- do not need to be directly referenced by the scene from the first frame.

Good candidates in SlimeRush:

- slime prefab;
- enemy prefabs;
- pickup prefabs;
- gate prefabs;
- VFX prefabs;
- UI popup prefabs;
- level chunks;
- audio clips for dynamic content.

Bad candidates:

- `GameplayInstaller`;
- small interfaces;
- normal C# classes;
- tiny settings that are always needed immediately;
- objects already placed in the scene and never spawned dynamically.

## Why Slime Prefab Is a Good First Candidate

Slimes are gameplay objects.

They are created during gameplay:

```csharp
GameObject slime = Instantiate(slimePrefab, transform);
```

That means slime creation is already dynamic.

If we later move slime creation into a factory, Addressables can live inside the factory:

```text
SlimeCrowdManager
    asks ISlimeFactory for a slime

AddressableSlimeFactory
    loads or stores the slime prefab
    creates slime instances
```

The manager stays clean.

## What Not to Do

Do not put Addressables directly inside `SlimeCrowdManager`.

Bad direction:

```csharp
public class SlimeCrowdManager : MonoBehaviour
{
    private void AddSlimes(int amount)
    {
        Addressables.InstantiateAsync("SlimePrefab");
    }
}
```

This mixes too many responsibilities:

- crowd state;
- slime count;
- formation;
- UI notification;
- asset loading;
- prefab instantiation.

Better direction:

```csharp
public class SlimeCrowdManager : MonoBehaviour
{
    private ISlimeFactory slimeFactory;

    private void AddOneSlime()
    {
        GameObject slime = slimeFactory.Create(transform);
    }
}
```

The Addressables details stay behind `ISlimeFactory`.

## Important Detail: Addressables Are Async

Addressables usually load assets asynchronously.

This means the result is not available immediately in the same simple way as `Instantiate`.

Simple direct instantiate:

```csharp
GameObject slime = Instantiate(slimePrefab, transform);
```

Addressables style:

```csharp
AsyncOperationHandle<GameObject> handle =
    Addressables.LoadAssetAsync<GameObject>("SlimePrefab");
```

The prefab arrives later.

Because of this, the architecture needs a decision:

```text
Should we load every slime when it is needed?
Or should we preload the prefab once, then spawn quickly?
```

For SlimeRush, preload is easier to understand.

The project uses UniTask for this async preload step:

```csharp
await slimeFactory.InitializeAsync(destroyCancellationToken);
```

This keeps `SlimeCrowdManager` simple:

```text
Initialize factory once.
Then create slimes normally.
```

## Recommended Approach for SlimeRush

The learning order:

1. Create normal `ISlimeFactory`.
2. Create normal `SlimeFactory` using `Object.Instantiate`.
3. Move `SlimeCrowdManager` to `ISlimeFactory`.
4. Install Addressables package.
5. Mark slime prefab as Addressable.
6. Create `AddressableSlimeFactory`.
7. Preload the slime prefab once.
8. Spawn slimes from the loaded prefab.
9. Later add pooling if spawning becomes expensive.

Steps 1-8 are already done.

Step 9 is the next performance-oriented architecture topic.

This order keeps learning clear:

```text
Factory first.
Addressables second.
Pooling later.
```

## Simple Factory Before Addressables

Before using Addressables:

```csharp
public interface ISlimeFactory
{
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

    public GameObject Create(Transform parent)
    {
        return Object.Instantiate(slimePrefab, parent);
    }
}
```

This teaches the factory pattern without the async complexity of Addressables.

## Current Addressable Factory

The current factory interface has initialization and creation:

```csharp
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface ISlimeFactory
{
    UniTask InitializeAsync(CancellationToken cancellationToken);
    GameObject Create(Transform parent);
}
```

`InitializeAsync` exists because Addressables loading is async.

`Create` stays synchronous because the prefab is already loaded before gameplay creates slimes.

```csharp
public async UniTask InitializeAsync(CancellationToken cancellationToken)
{
    loadHandle = Addressables.LoadAssetAsync<GameObject>(slimePrefabAddress.Value);
    await loadHandle.Task;

    cancellationToken.ThrowIfCancellationRequested();
    loadedSlimePrefab = loadHandle.Result;
}
```

Then creation is simple:

```csharp
public GameObject Create(Transform parent)
{
    return Object.Instantiate(loadedSlimePrefab, parent);
}
```

## Preloading Concept

Preloading means:

```text
Load the prefab once before gameplay needs it.
Store it in memory.
Instantiate it later when needed.
```

This avoids making every `AddSlimes` call async.

Example flow:

```text
Game starts
AddressableSlimeFactory loads "SlimePrefab"
Loading completes
SlimeCrowdManager creates starting crowd
SlimeCrowdManager asks factory to create slimes
Factory instantiates the already loaded prefab
```

This is easier than making `AddSlimes` return `Task` or use coroutines.

This also avoids calling Addressables for every single slime.

Addressables loads the prefab once.

Instantiation still happens many times.

## Address Key

The Addressable key is a string or asset reference used to load the asset.

For this project, use a simple clear key:

```text
SlimePrefab
```

Avoid vague keys:

```text
New Prefab
Slime
test
prefab1
```

Good keys are stable and descriptive.

## Factory Binding

Old normal factory binding:

```csharp
Container.Bind<ISlimeFactory>()
    .To<SlimeFactory>()
    .AsSingle();
```

Current Addressables factory binding:

```csharp
Container.BindInstance(new SlimePrefabAddress(slimePrefabAddress)).AsSingle();

Container.Bind<ISlimeFactory>()
    .To<AddressableSlimeFactory>()
    .AsSingle();
```

The important part:

`SlimeCrowdManager` should not change when this binding changes.

Only the installer changes.

## How This Supports SOLID

### Single Responsibility Principle

Without factory and Addressables separation:

```text
SlimeCrowdManager manages crowd and loads/creates assets.
```

With factory:

```text
SlimeCrowdManager manages crowd.
AddressableSlimeFactory loads and creates slime objects.
```

### Dependency Inversion Principle

The manager depends on an interface:

```text
SlimeCrowdManager -> ISlimeFactory
```

It does not depend on:

```text
Addressables
AssetReference
AsyncOperationHandle
slimePrefab field
```

### Open/Closed Principle

You can replace the implementation:

```text
SlimeFactory
AddressableSlimeFactory
PooledSlimeFactory
AddressablePooledSlimeFactory
```

The manager can stay closed for modification.

## When Addressables Become Worth It

Addressables are worth it when the project starts having dynamic content.

For example:

```text
Different slime skins
Different enemy types
Different level chunks
Different VFX packs
Content loaded per level
Content downloaded later
```

If the game only has one scene and one small prefab, Addressables are not technically necessary.

But for homework, the slime prefab is still a good learning target because it demonstrates the correct architecture.

## Short Version

```text
Factory decides how to create objects.
Addressables decide how to load assets.
SlimeCrowdManager should know neither detail.
```

For this project:

```text
SlimeCrowdManager
    -> ISlimeFactory
    -> AddressableSlimeFactory
    -> Addressables.LoadAssetAsync("SlimePrefab")
    -> Object.Instantiate(loadedSlimePrefab)
```

The normal factory step is complete.

The Addressables preload step is complete.

The next performance step is pooling.

## Practical Rule

Ask:

```text
Is this asset spawned dynamically and could its loading strategy change later?
```

If yes, Addressables can be useful.

For SlimeRush:

```text
Slime prefab - yes
Enemy prefab - yes
Pickup prefab - yes
GameplayInstaller - no
ICrowdFormation - no
ISlimeCrowd - no
```

## Addressables vs Pooling

Addressables and pooling solve different problems.

Addressables answer:

```text
Where does the prefab asset come from?
How is it loaded?
When do we release the loaded asset?
```

Pooling answers:

```text
Do we really need to Instantiate and Destroy every time?
Can we reuse old slime objects instead?
```

Current project:

```text
Addressables load the slime prefab once.
Object.Instantiate still creates every slime object.
Destroy still removes slime objects.
```

Future project with pooling:

```text
Addressables load the slime prefab once.
Pool creates an initial set of slime objects.
AddSlimes takes inactive slimes from the pool.
RemoveSlimes returns slimes to the pool instead of destroying them.
```

This is why pooling is the next good topic for many slimes.
