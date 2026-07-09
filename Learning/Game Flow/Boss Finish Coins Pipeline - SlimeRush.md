# Boss, Finish, Coins Pipeline - SlimeRush

## Общая идея

Пайплайн уровня:

```text
Player starts level
        ↓
Run gameplay
        ↓
Collect slimes / pass gates / avoid traps / fight enemies
        ↓
Reach boss
        ↓
Kill boss
        ↓
Level complete screen
        ↓
Coins reward
        ↓
Main menu / next flow
```

Если толпа умерла:

```text
Slime count = 0
        ↓
Game over screen
        ↓
Restart
        ↓
or rewarded ad continue
```

## Почему boss должен завершать уровень

Boss - это финальная проверка силы толпы.

Если игрок собрал мало слаймов или плохо прошел уровень, он может не убить босса. Если собрал достаточно, убивает босса и получает завершение уровня.

Так core loop становится понятным:

```text
собираю толпу → усиливаю толпу → дохожу до босса → трачу силу толпы → побеждаю
```

## Состояния игры

Минимально нужны такие состояния:

```text
Playing
BossFight
LevelComplete
GameOver
```

### Playing

Обычный бег:

```text
игрок движется
работают gates
работают traps
работают enemies
можно подбирать bonuses
```

Переходы:

```text
если дошли до boss zone → BossFight
если SlimeCount <= 0 → GameOver
```

### BossFight

Финальный бой:

```text
player movement останавливается или ограничивается
boss получает урон от толпы
boss атакует толпу
shield может заблокировать один boss hit
```

Переходы:

```text
если boss HP <= 0 → LevelComplete
если SlimeCount <= 0 → GameOver
```

### LevelComplete

Уровень пройден:

```text
движение игрока остановлено
опасности больше не наносят урон
показывается плашка победы
начисляются монетки
```

Кнопки:

```text
Main Menu
Next Level
```

На первом этапе можно оставить только:

```text
Main Menu
```

### GameOver

Толпа умерла:

```text
движение игрока остановлено
уровень считается проваленным
показывается плашка поражения
```

Кнопки:

```text
Restart
Continue with Ad
```

`Continue with Ad` лучше сделать как future hook. Сначала можно оставить кнопку disabled или заглушку.

## Монетки

Монетки лучше начислять не в UI, а в отдельном сервисе.

Плохо:

```text
LevelCompleteView сама считает и сохраняет coins
```

Лучше:

```text
LevelRewardService считает reward
CurrencyService добавляет coins
LevelCompleteView только показывает результат
```

Пример flow:

```text
Boss defeated
        ↓
LevelCompleteService.CompleteLevel()
        ↓
LevelRewardService.CalculateReward()
        ↓
CurrencyService.AddCoins(reward)
        ↓
LevelCompleteView.Show(reward, totalCoins)
```

## Откуда брать reward

На старте можно просто:

```text
baseLevelReward = 100
```

Позже можно расширить:

```text
base reward за уровень
bonus за оставшихся слаймов
bonus за shield unused
bonus multiplier за perfect run
```

Пример:

```text
reward = baseReward + remainingSlimes * 2
```

Но для первой версии лучше не усложнять:

```text
reward = 100
```

## Будущий meta content

Монетки нужны не просто как число на экране. Они пригодятся для meta progression:

```text
upgrade starting slime count
upgrade shield duration
upgrade income
unlock skins
unlock new bonuses
unlock boss rewards
```

Поэтому coins должны жить в отдельном сервисе/сохранении, а не внутри UI.

## Реклама continue

Когда толпа умерла:

```text
GameOver
        ↓
Show restart button
Show continue with ad button
```

Если игрок посмотрел рекламу:

```text
Ad completed
        ↓
restore small slime count
        ↓
return to Playing or BossFight
```

Пример:

```text
continueSlimeCount = 10
```

Важно: рекламный сервис должен быть отдельным:

```text
IRewardedAdService
```

GameOver UI не должен напрямую знать SDK рекламы.

## Архитектурные блоки на будущее

Можно прийти к таким интерфейсам:

```csharp
public interface IGameStateMachine
{
    void ChangeState(GameStateId stateId);
}
```

```csharp
public interface ILevelCompleteService
{
    void CompleteLevel();
}
```

```csharp
public interface ILevelRewardService
{
    int CalculateReward();
}
```

```csharp
public interface ICurrencyService
{
    int Coins { get; }
    void AddCoins(int amount);
}
```

```csharp
public interface IRewardedAdService
{
    UniTask<bool> ShowContinueAdAsync();
}
```

## Что делать по порядку

Рекомендуемый порядок:

```text
1. Boss health/damage system
2. Boss fight trigger
3. Level complete state
4. Game over state
5. Reward calculation
6. Currency save
7. Win/lose UI
8. Ad continue hook
```

UI лучше делать после gameplay flow, потому UI должен показывать реальные состояния, а не временные заглушки.

## Минимальная первая версия

Чтобы не распыляться:

```text
Boss has HP
Crowd damages boss
Boss damages crowd
Shield blocks one boss hit
Boss death triggers LevelComplete
LevelComplete adds 100 coins
GameOver shows Restart
```

А позже:

```text
reward scaling
main menu economy
ads continue
next level
upgrades
skins
```
