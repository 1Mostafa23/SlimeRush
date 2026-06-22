using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;


public class SlimeCrowdManager : MonoBehaviour, ISlimeCrowd, ISlimeCrowdCommands
{
    [Header("Starting Crowd")]
    [SerializeField] private int startingSlimeCount = 5;

    private readonly List<GameObject> slimes = new();

    private ICrowdFormation crowdFormation;
    private ISlimeFactory slimeFactory;
    private bool isInitialized;

    public int SlimeCount => slimes.Count;

    public event Action<int> OnSlimeCountChanged;

    [Inject]
    private void Construct(ICrowdFormation crowdFormation, ISlimeFactory slimeFactory)
    {
        this.crowdFormation = crowdFormation;
        this.slimeFactory = slimeFactory;
    }

    private void Start()
    {
        InitializeCrowdAsync().Forget();
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
            GameObject slime = slimeFactory.Create(transform);
            slimes.Add(slime);
        }

        RearrangeCrowd();
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
            Destroy(slimeToRemove);
        }

        RearrangeCrowd();
        NotifySlimeCountChanged();
    }

    public void MultiplySlimes(int multiplier)
    {
        if (multiplier <= 1)
            return;

        int currentCount = SlimeCount;
        int amountToAdd = currentCount * (multiplier - 1);

        AddSlimes(amountToAdd);
    }

    private void RearrangeCrowd()
    {
        IReadOnlyList<Vector3> positions = crowdFormation.GeneratePositions(slimes.Count);

        for (int i = 0; i < slimes.Count; i++)
        {
            Transform slimeTransform = slimes[i].transform;
            slimeTransform.localPosition = positions[i];
            slimeTransform.localRotation = Quaternion.identity;
        }
    }

    private void ClearCrowd()
    {
        for (int i = slimes.Count - 1; i >= 0; i--)
        {
            if (slimes[i] != null)
                Destroy(slimes[i]);
        }

        slimes.Clear();
        NotifySlimeCountChanged();
    }

    private void NotifySlimeCountChanged()
    {
        OnSlimeCountChanged?.Invoke(SlimeCount);
    }
}
