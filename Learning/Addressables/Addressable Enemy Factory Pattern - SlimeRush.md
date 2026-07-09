# Addressable Enemy Factory Pattern - SlimeRush

## Зачем это нужно

Раньше враг был обычным prefab/object reference:

```text
сцена или спавнер напрямую знает конкретный prefab врага
```

Это нормально на старте проекта, когда врагов мало. Но если потом появятся разные враги, уровни, конфиги волн и боссы, прямые ссылки начинают мешать.

Более гибкий вариант:

```text
LevelConfig / EnemySpawner
        ↓
IEnemyFactory
        ↓
AddressableEnemyFactory
        ↓
Addressables.LoadAssetAsync
        ↓
Zenject InstantiatePrefab
        ↓
готовый враг с DI
```

Главная идея: спавнер не знает, где лежит prefab и как он грузится. Спавнер знает только id врага.

## Что сделать в Unity

На prefab врага:

```text
Assets/_Project/Prefabs/Enemies/Enemy_SideMover_01.prefab
```

В Inspector:

```text
Addressable: true
Address: Enemies/LaneJumper
Group: Default Local Group
```

Не стоит оставлять address как полный путь:

```text
Assets/_Project/Prefabs/Enemies/Enemy_SideMover_01.prefab
```

Путь может измениться. А `Enemies/LaneJumper` - это стабильный gameplay id.

## Интерфейс фабрики

```csharp
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IEnemyFactory
{
    UniTask<GameObject> CreateAsync(
        EnemyAddress enemyAddress,
        Vector3 position,
        Quaternion rotation,
        Transform parent,
        CancellationToken cancellationToken
    );
}
```

Спавнер будет зависеть от `IEnemyFactory`, а не от Addressables напрямую.

## Address врага

```csharp
public readonly struct EnemyAddress
{
    public static readonly EnemyAddress LaneJumper = new("Enemies/LaneJumper");

    public EnemyAddress(string value)
    {
        Value = value;
    }

    public string Value { get; }
}
```

Так лучше, чем разбрасывать строки `"Enemies/LaneJumper"` по всему коду.

## Как будет выглядеть будущий спавнер

Это пример, его не обязательно держать в runtime-коде проекта:

```csharp
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnParent;

    private IEnemyFactory enemyFactory;

    [Inject]
    private void Construct(IEnemyFactory enemyFactory)
    {
        this.enemyFactory = enemyFactory;
    }

    public async UniTask<GameObject> SpawnLaneJumperAsync()
    {
        if (enemyFactory == null)
            throw new InvalidOperationException("EnemySpawner: IEnemyFactory was not injected.");

        return await enemyFactory.CreateAsync(
            EnemyAddress.LaneJumper,
            transform.position,
            transform.rotation,
            spawnParent,
            destroyCancellationToken
        );
    }
}
```

## Почему UniTask

Addressables грузятся асинхронно:

```csharp
Addressables.LoadAssetAsync<GameObject>("Enemies/LaneJumper");
```

Prefab может быть не готов в тот же кадр. Поэтому надо дождаться загрузки:

```csharp
GameObject prefab = await Addressables.LoadAssetAsync<GameObject>("Enemies/LaneJumper");
```

В Unity удобнее использовать `UniTask`, потому он хорошо работает с Unity lifecycle и cancellation token.

## Почему Zenject InstantiatePrefab

Нельзя просто делать так:

```csharp
Object.Instantiate(prefab);
```

Если prefab содержит компоненты с `[Inject]`, обычный `Instantiate` не выполнит Zenject injection.

Правильнее:

```csharp
container.InstantiatePrefab(prefab, position, rotation, parent);
```

Так враг создается и сразу получает зависимости.

## Правило архитектуры

Плохо:

```text
EnemySpawner знает про Addressables
EnemySpawner знает путь к prefab
EnemySpawner сам делает Instantiate
```

Лучше:

```text
EnemySpawner знает только IEnemyFactory
AddressableEnemyFactory знает Addressables
Zenject отвечает за создание объекта и DI
```

Так проще заменить загрузку в будущем: Addressables, pool, remote bundles или обычные prefab references.
