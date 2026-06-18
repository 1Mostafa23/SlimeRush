# Zenject DI - SlimeRush Example

## Главная идея

`SlimeCrowdManager` не должен сам создавать зависимости через `new`, если проект уже использует Zenject.

Плохой вариант:

```csharp
private ICrowdFormation crowdFormation;

private void Awake()
{
    crowdFormation = new EllipseCrowdFormation();
}
```

Проблема здесь не в самом `new`, а в том, что класс сам решает, какую конкретную реализацию использовать.

Из-за этого:

- `SlimeCrowdManager` знает про `EllipseCrowdFormation`;
- сложнее заменить эллипс на другую формацию;
- сложнее тестировать класс отдельно;
- настройки сложнее передавать централизованно;
- DI-контейнер Zenject становится почти бесполезным для этой зависимости.

Хороший вариант:

```csharp
private ICrowdFormation crowdFormation;

[Inject]
private void Construct(ICrowdFormation crowdFormation)
{
    this.crowdFormation = crowdFormation;
}
```

Теперь `SlimeCrowdManager` знает только про интерфейс.

Он не знает:

- кто создал объект;
- какая конкретная реализация используется;
- откуда пришли настройки;
- сколько внутренних зависимостей есть у формации.

Этим занимается `GameplayInstaller`.

## Роли классов

### SlimeCrowdManager

Отвечает за состояние толпы:

- хранит список слаймов;
- добавляет слаймов;
- удаляет слаймов;
- применяет позиции к `Transform`;
- сообщает UI, что количество изменилось.

Он не должен считать математическую форму толпы.

Пример:

```csharp
public class SlimeCrowdManager : MonoBehaviour, ISlimeCrowd, ISlimeCrowdCommands
{
    private ICrowdFormation crowdFormation;

    [Inject]
    private void Construct(ICrowdFormation crowdFormation)
    {
        this.crowdFormation = crowdFormation;
    }

    private void RearrangeCrowd()
    {
        IReadOnlyList<Vector3> positions = crowdFormation.GeneratePositions(slimes.Count);

        for (int i = 0; i < slimes.Count; i++)
        {
            slimes[i].transform.localPosition = positions[i];
            slimes[i].transform.localRotation = Quaternion.identity;
        }
    }
}
```

### ICrowdFormation

Это контракт для любого алгоритма расстановки толпы.

```csharp
public interface ICrowdFormation
{
    IReadOnlyList<Vector3> GeneratePositions(int count);
}
```

Важно: интерфейс говорит, что нужно сделать, но не говорит как.

Сегодня можно использовать эллипс:

```csharp
public class EllipseCrowdFormation : ICrowdFormation
{
}
```

Позже можно заменить на линию:

```csharp
public class LineCrowdFormation : ICrowdFormation
{
}
```

И `SlimeCrowdManager` не придётся менять.

### EllipseCrowdFormation

Отвечает только за математику эллипса.

```csharp
public class EllipseCrowdFormation : ICrowdFormation
{
    private readonly CrowdFormationSettings settings;

    public EllipseCrowdFormation(CrowdFormationSettings settings)
    {
        this.settings = settings;
    }

    public IReadOnlyList<Vector3> GeneratePositions(int count)
    {
        // calculate positions
    }
}
```

У него обычный C# constructor, потому что это не `MonoBehaviour`.

Zenject умеет создавать такие классы сам и передавать им зависимости в constructor.

### CrowdFormationSettings

`ScriptableObject` хранит данные для баланса.

```csharp
[CreateAssetMenu(
    fileName = "CrowdFormationSettings",
    menuName = "SlimeRush/Slimes/Crowd Formation Settings")]
public class CrowdFormationSettings : ScriptableObject
{
    [SerializeField] private float baseSpacing = 1.0f;
    [SerializeField] private float ellipseWidth = 1.15f;
    [SerializeField] private float ellipseDepth = 0.9f;
    [SerializeField] private float frontOffsetZ = -0.35f;
    [SerializeField] private float randomOffsetAmount = 0.05f;

    public float BaseSpacing => baseSpacing;
    public float EllipseWidth => ellipseWidth;
    public float EllipseDepth => ellipseDepth;
    public float FrontOffsetZ => frontOffsetZ;
    public float RandomOffsetAmount => randomOffsetAmount;
}
```

Почему `ScriptableObject` здесь полезен:

- настройки видны в Unity Inspector;
- можно менять баланс без изменения кода;
- можно создать несколько assets с разными настройками;
- данные не привязаны к конкретному объекту сцены.

## Где появляется конкретика

Конкретные классы выбираются в одном месте: в `GameplayInstaller`.

```csharp
public class GameplayInstaller : MonoInstaller
{
    [SerializeField] private SlimeCrowdManager slimeCrowdManager;
    [SerializeField] private CrowdFormationSettings crowdFormationSettings;

    public override void InstallBindings()
    {
        Container.BindInstance(crowdFormationSettings).AsSingle();

        Container.Bind<ICrowdFormation>()
            .To<EllipseCrowdFormation>()
            .AsSingle();

        Container.Bind<SlimeCrowdManager>()
            .FromInstance(slimeCrowdManager)
            .AsSingle();

        Container.Bind<ISlimeCrowd>()
            .FromInstance(slimeCrowdManager)
            .AsSingle();

        Container.Bind<ISlimeCrowdCommands>()
            .FromInstance(slimeCrowdManager)
            .AsSingle();
    }
}
```

Это называется composition root.

Composition root - место, где собирается граф зависимостей приложения.

Обычные игровые классы не должны решать, какие реализации создавать. Они должны получать готовые зависимости.

## Почему не new

`new` допустим для простых внутренних объектов, которые не являются зависимостями архитектуры.

Например нормально:

```csharp
List<Vector3> positions = new();
```

Это просто временный список внутри метода.

Но нежелательно:

```csharp
crowdFormation = new EllipseCrowdFormation(settings);
```

Потому что `EllipseCrowdFormation` - это отдельная архитектурная зависимость.

Если класс сам создаёт такую зависимость, он нарушает Dependency Inversion Principle:

- high-level класс `SlimeCrowdManager` начинает зависеть от low-level класса `EllipseCrowdFormation`;
- вместо зависимости от абстракции `ICrowdFormation`.

Правильная зависимость:

```text
SlimeCrowdManager -> ICrowdFormation
EllipseCrowdFormation -> ICrowdFormation
GameplayInstaller связывает ICrowdFormation с EllipseCrowdFormation
```

## Read и Command интерфейсы

Для толпы мы разделили чтение и команды.

Read-only интерфейс:

```csharp
public interface ISlimeCrowd
{
    int SlimeCount { get; }
    event Action<int> OnSlimeCountChanged;
}
```

Командный интерфейс:

```csharp
public interface ISlimeCrowdCommands
{
    void AddSlimes(int amount);
    void RemoveSlimes(int amount);
    void MultiplySlimes(int multiplier);
}
```

Зачем разделять:

- UI должен только читать количество;
- ворота должны менять количество;
- враги могут удалять слаймов;
- пикапы могут добавлять слаймов.

Так UI случайно не получит доступ к методам изменения толпы.

## Как это пригодится воротам

Ворота не должны знать про `SlimeCrowdManager`.

Плохо:

```csharp
public class Gate : MonoBehaviour
{
    [SerializeField] private SlimeCrowdManager slimeCrowdManager;

    private void Apply()
    {
        slimeCrowdManager.AddSlimes(10);
    }
}
```

Лучше:

```csharp
public class Gate : MonoBehaviour
{
    private ISlimeCrowdCommands slimeCrowdCommands;

    [Inject]
    private void Construct(ISlimeCrowdCommands slimeCrowdCommands)
    {
        this.slimeCrowdCommands = slimeCrowdCommands;
    }

    private void Apply()
    {
        slimeCrowdCommands.AddSlimes(10);
    }
}
```

Теперь ворота знают только, что есть команды толпы.

Они не знают, кто конкретно эти команды выполняет.

## Простая схема

```text
GameplayInstaller
    binds CrowdFormationSettings asset
    binds ICrowdFormation -> EllipseCrowdFormation
    binds ISlimeCrowd -> SlimeCrowdManager
    binds ISlimeCrowdCommands -> SlimeCrowdManager

SlimeCrowdManager
    receives ICrowdFormation
    owns slime list
    applies generated positions

EllipseCrowdFormation
    receives CrowdFormationSettings
    calculates positions

CrowdFormationSettings.asset
    stores tunable numbers

CrowdCountLabel
    receives ISlimeCrowd
    reads count

Future Gate
    receives ISlimeCrowdCommands
    changes count
```

## Правило для себя

Если класс использует другой класс как важную часть поведения, лучше зависеть от интерфейса и получать зависимость через Zenject.

Если объект является просто локальной технической деталью внутри метода, `new` обычно нормален.

Коротко:

```text
new List<T>() - нормально
new EllipseCrowdFormation() внутри SlimeCrowdManager - плохо
ICrowdFormation через [Inject] - хорошо
```

