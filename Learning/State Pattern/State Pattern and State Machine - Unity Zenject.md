# State Pattern и State Machine в Unity + Zenject

## Зачем это нужно

State Pattern нужен, когда у объекта есть разные режимы поведения.

Например враг может быть:

- `Idle` - стоит и ждет.
- `Chasing` - бежит к игроку.
- `Attacking` - атакует.
- `Cooldown` - ждет после атаки.
- `Dead` - выключен.

Если писать это через один большой `Update`, код быстро превращается в набор `if`, `else`, `switch`, флагов и багов.

Плохой признак:

```csharp
if (isDead)
    return;

if (isAttacking)
{
    // attack logic
}
else if (isChasing)
{
    // chase logic
}
else if (isWaiting)
{
    // idle logic
}
```

Сначала это нормально. Но когда появляется 3-5 состояний, условия начинают мешать друг другу.

## Главная идея

Вместо одного класса, который знает все режимы, мы делаем отдельный класс на каждое состояние.

```text
Enemy
 -> EnemyStateMachine
    -> IdleState
    -> ChaseState
    -> AttackState
    -> DeadState
```

Каждое состояние отвечает только за свое поведение.

## Чем State отличается от простого if/switch

### Вариант без states

```csharp
public class EnemyView : MonoBehaviour
{
    private bool isIdle;
    private bool isAttacking;
    private bool isDead;

    private void Update()
    {
        if (isDead)
            return;

        if (isAttacking)
        {
            Debug.Log("Enemy attacks");
            return;
        }

        if (isIdle)
        {
            Debug.Log("Enemy waits");
            return;
        }
    }
}
```

Проблема:

- один класс знает все;
- много флагов;
- легко забыть выключить старый флаг;
- сложно добавить новое состояние;
- сложно тестировать отдельно.

### Вариант со states

```csharp
public interface IEnemyState
{
    void Enter();
    void Tick();
    void Exit();
}
```

```csharp
public class EnemyIdleState : IEnemyState
{
    public void Enter()
    {
        Debug.Log("Enter Idle");
    }

    public void Tick()
    {
        Debug.Log("Enemy waits");
    }

    public void Exit()
    {
        Debug.Log("Exit Idle");
    }
}
```

```csharp
public class EnemyAttackState : IEnemyState
{
    public void Enter()
    {
        Debug.Log("Enter Attack");
    }

    public void Tick()
    {
        Debug.Log("Enemy attacks");
    }

    public void Exit()
    {
        Debug.Log("Exit Attack");
    }
}
```

Теперь каждое состояние живет отдельно.

## State Machine

State Machine - это класс, который хранит текущее состояние и переключает его.

```csharp
public class EnemyStateMachine
{
    private IEnemyState currentState;

    public void ChangeState(IEnemyState nextState)
    {
        currentState?.Exit();
        currentState = nextState;
        currentState.Enter();
    }

    public void Tick()
    {
        currentState?.Tick();
    }
}
```

Логика простая:

```text
старое состояние Exit
новое состояние Enter
каждый кадр вызывается Tick
```

## Где здесь MonoBehaviour

В Unity полностью отказаться от `MonoBehaviour` нельзя.

Правильная схема:

```text
MonoBehaviour
 -> ловит Unity events: Update, OnTriggerEnter, SerializeField
 -> вызывает обычные C# states/services
```

Пример:

```csharp
public class EnemyView : MonoBehaviour
{
    private EnemyStateMachine stateMachine;

    private void Update()
    {
        stateMachine.Tick();
    }
}
```

`EnemyView` не должен содержать всю логику состояний. Он только соединяет Unity-сцену и обычный C# код.

## Где здесь Zenject

Zenject нужен, чтобы не создавать зависимости руками через `new` внутри MonoBehaviour.

Пример идеи:

```csharp
Container.Bind<EnemyStateMachine>().AsSingle();
Container.Bind<EnemyIdleState>().AsSingle();
Container.Bind<EnemyAttackState>().AsSingle();
```

А потом:

```csharp
[Inject]
private void Construct(
    EnemyStateMachine stateMachine,
    EnemyIdleState idleState)
{
    this.stateMachine = stateMachine;
    stateMachine.ChangeState(idleState);
}
```

Так `EnemyView` зависит от готовых классов, а Zenject собирает их.

## Когда states реально нужны

States стоит использовать, если:

- у объекта 3 или больше режима поведения;
- каждый режим имеет свой `Enter/Exit`;
- есть переходы между режимами;
- появляются флаги типа `isAttacking`, `isDead`, `isWaiting`, `isMoving`;
- логика в `Update` становится длинной;
- нужно легко добавлять новые режимы.

States не нужны, если:

- объект просто один раз срабатывает;
- логика состоит из одного `OnTriggerEnter`;
- состояние только `isUsed`;
- нет сложных переходов.

Например наши ворота пока не требуют state machine:

```text
GateView
 -> не использовано
 -> использовано
```

Там достаточно `isUsed`.

## Где можно применить в SlimeRush

### Хороший кандидат: второй тип врага

Если враг будет иметь поведение:

```text
Idle
PrepareAttack
Attack
Cooldown
Disabled
```

тогда state machine будет полезна.

### Хороший кандидат: flow уровня

```text
WaitingToStart
Playing
Win
Lose
```

Это тоже удобно делать через states.

### Возможный кандидат: Player/Crowd

```text
Running
Stopped
Finished
Dead
```

Но это лучше делать позже, когда появится finish/lose/win.

## Практический пример через Debug.Log

Задача: сделать абстрактного врага без Unity-физики.

Поведение:

```text
Enemy starts in Idle
After 2 seconds -> PrepareAttack
After 1 second -> Attack
After attack -> Cooldown
After cooldown -> Idle
```

Интерфейс:

```csharp
public interface IEnemyState
{
    void Enter();
    void Tick(float deltaTime);
    void Exit();
}
```

State machine:

```csharp
public class EnemyStateMachine
{
    private IEnemyState currentState;

    public void ChangeState(IEnemyState nextState)
    {
        currentState?.Exit();
        currentState = nextState;
        currentState.Enter();
    }

    public void Tick(float deltaTime)
    {
        currentState?.Tick(deltaTime);
    }
}
```

Idle:

```csharp
public class EnemyIdleState : IEnemyState
{
    private readonly EnemyStateMachine stateMachine;
    private readonly EnemyPrepareAttackState prepareAttackState;
    private float timer;

    public EnemyIdleState(
        EnemyStateMachine stateMachine,
        EnemyPrepareAttackState prepareAttackState)
    {
        this.stateMachine = stateMachine;
        this.prepareAttackState = prepareAttackState;
    }

    public void Enter()
    {
        timer = 0f;
        Debug.Log("Enemy: Idle");
    }

    public void Tick(float deltaTime)
    {
        timer += deltaTime;

        if (timer >= 2f)
            stateMachine.ChangeState(prepareAttackState);
    }

    public void Exit()
    {
        Debug.Log("Enemy: Exit Idle");
    }
}
```

Идея: каждое состояние само решает, когда перейти дальше.

## Почему это лучше

State pattern дает:

- меньше флагов;
- меньше огромных `if/switch`;
- каждый режим поведения отдельно;
- проще добавлять новые состояния;
- проще объяснять архитектуру;
- лучше сочетается с Zenject;
- легче тестировать обычные C# классы.

## Главное правило для проекта

Не нужно делать states ради states.

Используем так:

```text
простая одноразовая механика -> MonoBehaviour + service
сложное поведение с режимами -> State Machine
математика/правила -> обычные C# классы
Unity-события/сцена -> MonoBehaviour
связи между классами -> Zenject
```

Для второго врага сначала нужно описать его поведение. Если у него будет несколько фаз, лучше сразу делать через state machine.
