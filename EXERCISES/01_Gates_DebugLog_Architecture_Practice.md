# Упражнение 01: Ворота через Debug.Log

## Зачем это упражнение

Ты уже сделал ворота в SlimeRush, но тема кажется сложной, потому что там сразу много всего:

- Unity-сцена
- префабы
- коллайдеры
- Zenject
- SOLID
- фабрики
- группы ворот
- UI-лейблы
- толпа слаймов

Это упражнение убирает визуальную часть. Остается только логика. Все можно проверять через `Debug.Log`.

## Игровая задача

Есть игрок с количеством слаймов.

Есть пара ворот:

```text
GatePair_01
  LeftGate: +10
  RightGate: x2
```

Правило:

```text
Игрок может выбрать только одни ворота из пары.
Выбранные ворота применяют эффект.
Вторые ворота остаются видимыми, но больше не активируются.
```

## Что нужно реализовать

Сделай отдельные классы для логики. Можно создать их в тестовой папке или просто написать в одном временном учебном месте.

Главное: проверяй работу через `Debug.Log`.

## Архитектурная схема

```text
PlayerCrowd
  хранит количество слаймов

ISlimeCrowdCommands
  команды: Add, Remove, Multiply

GateOperationType
  Add / Multiply / Subtract

IGateOperation
  общий контракт операции

AddGateOperation
MultiplyGateOperation
SubtractGateOperation
  конкретные операции

GateOperationResolver
  выбирает нужную операцию по типу

CrowdCountChangeApplier
  применяет изменение к толпе через ISlimeCrowdCommands

GateEffectApplier
  главный сервис применения эффекта ворот

GateGroup
  помнит, использована ли пара ворот

Gate
  конкретные ворота: тип операции, значение, ссылка на группу
```

## Главное правило SOLID

Не делай один большой класс `Gate`, который:

- хранит толпу
- считает математику
- меняет количество слаймов
- проверяет группу
- рисует текст
- создает префабы

Такой класс быстро станет грязным.

Лучше много маленьких классов, где каждый делает одну вещь.

## Практическое ТЗ

### 1. Создай `ISlimeCrowdCommands`

```csharp
public interface ISlimeCrowdCommands
{
    void AddSlimes(int amount);
    void RemoveSlimes(int amount);
    void MultiplySlimes(int multiplier);
}
```

### 2. Создай `DebugSlimeCrowd`

Он хранит число слаймов и пишет в консоль результат.

Пример поведения:

```text
Start count: 5
Add 10 -> count: 15
Multiply 2 -> count: 30
Remove 7 -> count: 23
```

### 3. Создай `GateOperationType`

```csharp
public enum GateOperationType
{
    Add,
    Multiply,
    Subtract
}
```

### 4. Создай `IGateOperation`

Контракт должен отвечать на два вопроса:

```text
Какой это тип операции?
Какой текст показать игроку?
```

Например:

```csharp
public interface IGateOperation
{
    GateOperationType Type { get; }
    string GetLabel(int value);
}
```

### 5. Создай операции

Нужны классы:

```text
AddGateOperation
MultiplyGateOperation
SubtractGateOperation
```

Примеры label:

```text
Add 10 -> +10
Multiply 2 -> x2
Subtract 5 -> -5
```

### 6. Создай `GateOperationResolver`

Он получает `GateOperationType` и возвращает нужную операцию.

Пример:

```text
Add -> AddGateOperation
Multiply -> MultiplyGateOperation
Subtract -> SubtractGateOperation
```

Это нужно, чтобы `Gate` не создавал операции сам через `new`.

### 7. Создай `CrowdCountChangeApplier`

Он получает `ISlimeCrowdCommands` и применяет эффект.

Пример логики:

```text
Add -> crowd.AddSlimes(value)
Multiply -> crowd.MultiplySlimes(value)
Subtract -> crowd.RemoveSlimes(value)
```

Важно: этот класс не должен знать про конкретный `DebugSlimeCrowd`.

Он знает только интерфейс `ISlimeCrowdCommands`.

### 8. Создай `GateEffectApplier`

Это главный класс для применения ворот.

Он должен:

```text
получить operation type и value
попросить resolver найти операцию
попросить CrowdCountChangeApplier применить изменение
написать Debug.Log с label операции
```

### 9. Создай `GateGroup`

Он отвечает только за правило:

```text
из этой группы можно использовать только одни ворота
```

Пример:

```csharp
public class GateGroup
{
    private bool isUsed;

    public bool CanUseGate()
    {
        return !isUsed;
    }

    public void MarkUsed()
    {
        isUsed = true;
    }
}
```

### 10. Создай `Gate`

Это учебная версия без Unity-collider.

Поля:

```text
GateOperationType operationType
int value
GateGroup group
GateEffectApplier effectApplier
string name
```

Метод:

```text
Enter()
```

Логика:

```text
если группа уже использована -> Debug.Log("Gate ignored")
иначе применить эффект
пометить группу использованной
Debug.Log("Gate used")
```

## Проверочный сценарий

Напиши метод, который руками вызывает:

```text
Create crowd with 5 slimes
Create one GateGroup
Create left gate: Add 10
Create right gate: Multiply 2

leftGate.Enter()
rightGate.Enter()
```

Ожидаемый результат:

```text
Start count: 5
Gate +10 used
Add 10 -> count: 15
Gate x2 ignored because group already used
Final count: 15
```

Потом проверь обратный порядок:

```text
rightGate.Enter()
leftGate.Enter()
```

Ожидаемый результат:

```text
Start count: 5
Gate x2 used
Multiply 2 -> count: 10
Gate +10 ignored because group already used
Final count: 10
```

## Где тут Zenject

В реальном проекте Zenject нужен для таких зависимостей:

```text
GateEffectApplier
GateOperationResolver
CrowdCountChangeApplier
ISlimeCrowdCommands
```

Почему:

```text
это логика/сервисы
они не зависят от конкретного объекта сцены
их удобно переиспользовать
их удобно тестировать
```

А вот `GateGroup` в реальной сцене лучше не делать глобальным Zenject-сервисом.

Почему:

```text
GateGroup относится к конкретной паре ворот
на уровне может быть много пар ворот
каждая пара должна иметь свой isUsed
```

В Unity это обычно:

```text
GatePair_01 has GateGroupView
  Gate_Add_10 has GateView
  Gate_Multiply_2 has GateView
```

`GateView` может найти группу через:

```csharp
GetComponentInParent<GateGroupView>()
```

## Где тут фабрика

Фабрика нужна, когда объект создается кодом и создание становится неочевидным.

Например:

```text
создать слайма
загрузить prefab через Addressables
положить слайма в нужного parent
сбросить transform
вернуть объект из pool
```

Это не должно жить в `SlimeCrowdManager`.

Поэтому у тебя есть идея:

```text
SlimeCrowdManager просит ISlimeFactory.Create()
```

А конкретная фабрика сама решает:

```text
Instantiate
Addressables
Pool
```

## Где тут префабы

Префаб нужен не для логики, а для готовой сборки объекта.

Например `PF_GatePair` хранит:

```text
родителя с GateGroupView
левые ворота с GateView
правые ворота с GateView
коллайдеры
текст
визуал
```

Код говорит, как оно работает.

Префаб говорит, из каких Unity-компонентов это собрано.

## Самопроверка

Ответь себе:

```text
1. Почему Gate не должен напрямую менять SlimeCrowdManager?
2. Почему GateGroup не нужно делать AsSingle в Zenject?
3. Почему фабрика лучше, чем Instantiate внутри SlimeCrowdManager?
4. Почему выбранные ворота исчезают, а соседние только блокируются?
5. Что изменится, если на уровне будет 10 пар ворот?
6. Где в этой схеме Single Responsibility Principle?
7. Где Dependency Inversion Principle?
```

## Усложнение

Когда базовая версия заработает через `Debug.Log`, добавь:

```text
GatePairFactory
```

Она должна создавать учебные пары ворот:

```text
CreateAddMultiplyPair(addValue, multiplyValue)
CreateSubtractAddPair(subtractValue, addValue)
```

Пока без Unity-префабов. Просто через обычные C# объекты.

Цель: понять идею фабрики без сцены.

## Главное, что нужно запомнить

```text
Zenject связывает зависимости.
SOLID помогает не превращать класс в кашу.
Фабрика прячет сложное создание объектов.
Префаб хранит готовую Unity-сборку объекта.
GateGroup хранит состояние выбора внутри одной пары ворот.
GateView не должен знать, как устроена толпа слаймов.
```
