using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;


public class SlimeCrowdManager : MonoBehaviour, ISlimeCrowd, ISlimeCrowdCommands, ISlimeCrowdDamageCommands, IBossCrowdFormationController
{
    private const float BossFormationMoveSpeed = 8f;
    private const float BossSemicircleSpacing = 0.85f;
    private const float BossSemicircleStartAngle = -80f;
    private const float BossSemicircleEndAngle = 80f;

    private readonly List<GameObject> slimes = new();
    private readonly List<Vector3> targetLocalPositions = new();

    private SlimeCrowdSettings settings;
    private ICrowdFormation crowdFormation;
    private ISlimeFactory slimeFactory;
    private ISlimePool slimePool;
    private ICrowdMovementStateMachine movementStateMachine;
    private CrowdFollowFormationState followFormationState;
    private IPlayerUpgradeService playerUpgradeService;
    private bool isInitialized;
    private bool isBossFormationActive;
    private int formationUpdateVersion;

    public int SlimeCount => slimes.Count;

    public event Action<int> OnSlimeCountChanged;

    [Inject]
    private void Construct(
        SlimeCrowdSettings settings,
        ICrowdFormation crowdFormation,
        ISlimeFactory slimeFactory,
        ISlimePool slimePool,
        ICrowdMovementStateMachine movementStateMachine,
        CrowdFollowFormationState followFormationState,
        IPlayerUpgradeService playerUpgradeService)
    {
        this.settings = settings;
        this.crowdFormation = crowdFormation;
        this.slimeFactory = slimeFactory;
        this.slimePool = slimePool;
        this.movementStateMachine = movementStateMachine;
        this.followFormationState = followFormationState;
        this.playerUpgradeService = playerUpgradeService;
    }

    private void Start()
    {
        movementStateMachine.ChangeState(followFormationState);
        InitializeCrowdAsync().Forget();
    }

    private void Update()
    {
        movementStateMachine.Tick(slimes, targetLocalPositions, Time.deltaTime);
    }

    private async UniTaskVoid InitializeCrowdAsync()
    {
        try
        {
            await slimeFactory.InitializeAsync(destroyCancellationToken);

            if (destroyCancellationToken.IsCancellationRequested)
                return;

            isInitialized = true;
            CreateStartingCrowd();
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }
    }

    private void CreateStartingCrowd()
    {
        ClearCrowd();
        int startingSlimeCount = playerUpgradeService.GetStartingSlimeCount(settings.StartingSlimeCount);
        Debug.Log($"SlimeCrowdManager: Starting slime count from {settings.name} = {startingSlimeCount}");
        AddSlimes(startingSlimeCount);
    }

    public void AddSlimes(int amount)
    {
        if (!isInitialized)
        {
            Debug.LogWarning("SlimeCrowdManager: Cannot add slimes before factory initialization.");
            return;
        }

        if (amount <= 0)
            return;

        for (int i = 0; i < amount; i++)
        {
            GameObject slime = slimePool.Rent(transform);
            slimes.Add(slime);
        }

        UpdateFormationTargets();
        NotifySlimeCountChanged();
    }

    public void RemoveSlimes(int amount)
    {
        if (amount <= 0)
            return;

        int removeCount = Mathf.Min(amount, slimes.Count);

        for (int i = 0; i < removeCount; i++)
        {
            int lastIndex = slimes.Count - 1;
            GameObject slimeToRemove = slimes[lastIndex];

            slimes.RemoveAt(lastIndex);
            slimePool.Return(slimeToRemove);
        }

        UpdateFormationTargets();
        NotifySlimeCountChanged();
    }

    public bool RemoveSlime(SlimeHitbox slimeHitbox)
    {
        if (slimeHitbox == null)
            return false;

        GameObject slime = ResolveSlimeObject(slimeHitbox);
        int slimeIndex = slimes.IndexOf(slime);

        if (slimeIndex < 0)
            return false;

        slimes.RemoveAt(slimeIndex);
        slimePool.Return(slime);

        ScheduleDelayedFormationTargetsUpdate();
        NotifySlimeCountChanged();

        return true;
    }

    public void MultiplySlimes(int multiplier)
    {
        if (multiplier <= 1)
            return;

        int currentCount = SlimeCount;
        int amountToAdd = currentCount * (multiplier - 1);

        AddSlimes(amountToAdd);
    }

    public void EnterBossFormation(Transform fightPoint)
    {
        if (fightPoint == null)
            return;

        isBossFormationActive = true;
        UpdateFormationTargets();
        MoveToBossFightPointAsync(fightPoint).Forget();
    }

    public void ExitBossFormation()
    {
        if (!isBossFormationActive)
            return;

        isBossFormationActive = false;
        UpdateFormationTargets();
    }

    private void UpdateFormationTargets()
    {
        formationUpdateVersion++;

        if (isBossFormationActive)
            RebuildBossSemicircleTargets();
        else
            RebuildFormationTargets();
    }

    private void ScheduleDelayedFormationTargetsUpdate()
    {
        if (settings.DamageFormationRebuildDelay <= 0f)
        {
            UpdateFormationTargets();
            return;
        }

        formationUpdateVersion++;
        DelayedFormationTargetsUpdateAsync(formationUpdateVersion).Forget();
    }

    private async UniTaskVoid DelayedFormationTargetsUpdateAsync(int expectedVersion)
    {
        try
        {
            await UniTask.Delay(
                TimeSpan.FromSeconds(settings.DamageFormationRebuildDelay),
                cancellationToken: destroyCancellationToken
            );

            if (expectedVersion != formationUpdateVersion)
                return;

            if (isBossFormationActive)
                RebuildBossSemicircleTargets();
            else
                RebuildFormationTargets();
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void RebuildFormationTargets()
    {
        IReadOnlyList<Vector3> positions = crowdFormation.GeneratePositions(slimes.Count);
        targetLocalPositions.Clear();

        for (int i = 0; i < slimes.Count; i++)
        {
            targetLocalPositions.Add(positions[i]);
            slimes[i].transform.localRotation = Quaternion.identity;
        }
    }

    private void RebuildBossSemicircleTargets()
    {
        targetLocalPositions.Clear();

        IReadOnlyList<Vector3> positions = GenerateBossSemicirclePositions(slimes.Count);

        for (int i = 0; i < slimes.Count; i++)
        {
            targetLocalPositions.Add(positions[i]);
            slimes[i].transform.localRotation = Quaternion.identity;
        }
    }

    private IReadOnlyList<Vector3> GenerateBossSemicirclePositions(int count)
    {
        List<Vector3> positions = new();
        int remaining = count;
        int row = 0;

        while (remaining > 0)
        {
            int capacity = Mathf.Min(remaining, 6 + row * 4);
            float radius = BossSemicircleSpacing * (row + 1);

            for (int i = 0; i < capacity; i++)
            {
                float t = capacity == 1 ? 0.5f : (float)i / (capacity - 1);
                float angle = Mathf.Lerp(BossSemicircleStartAngle, BossSemicircleEndAngle, t) * Mathf.Deg2Rad;

                positions.Add(new Vector3(
                    Mathf.Sin(angle) * radius,
                    0f,
                    Mathf.Cos(angle) * radius));
            }

            remaining -= capacity;
            row++;
        }

        return positions;
    }

    private async UniTaskVoid MoveToBossFightPointAsync(Transform fightPoint)
    {
        try
        {
            Vector3 targetPosition = fightPoint.position;
            targetPosition.y = transform.position.y;

            while (!destroyCancellationToken.IsCancellationRequested &&
                   Vector3.SqrMagnitude(transform.position - targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    BossFormationMoveSpeed * Time.deltaTime);

                await UniTask.Yield(destroyCancellationToken);
            }

            transform.position = targetPosition;
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void ClearCrowd()
    {
        for (int i = slimes.Count - 1; i >= 0; i--)
        {
            if (slimes[i] != null)
                slimePool.Return(slimes[i]);
        }

        slimes.Clear();
        targetLocalPositions.Clear();
        NotifySlimeCountChanged();
    }

    private void NotifySlimeCountChanged()
    {
        OnSlimeCountChanged?.Invoke(SlimeCount);
    }

    private GameObject ResolveSlimeObject(SlimeHitbox slimeHitbox)
    {
        Transform current = slimeHitbox.transform;

        while (current != null && current.parent != transform)
            current = current.parent;

        if (current != null)
            return current.gameObject;

        return slimeHitbox.SlimeObject;
    }
}
