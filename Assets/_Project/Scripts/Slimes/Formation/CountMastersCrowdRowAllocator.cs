using System.Collections.Generic;
using UnityEngine;

public class CountMastersCrowdRowAllocator : ICrowdRowAllocator
{
    private readonly CrowdFormationSettings settings;
    private readonly List<MutableRowAllocation> rowAllocations = new();
    private readonly List<CrowdRowAllocation> result = new();

    public CountMastersCrowdRowAllocator(CrowdFormationSettings settings)
    {
        this.settings = settings;
    }

    public IReadOnlyList<CrowdRowAllocation> AllocateRows(int slimeCount)
    {
        rowAllocations.Clear();
        result.Clear();

        if (slimeCount <= 0)
            return result;

        int rowCount = GetActiveRowCount(slimeCount);
        float[] rowWeights = new float[rowCount];
        float totalWeight = 0f;

        for (int row = 0; row < rowCount; row++)
        {
            float normalizedRow = rowCount == 1 ? 0f : (float)row / (rowCount - 1);
            float weight = GetOvalWeight(normalizedRow);

            rowWeights[row] = weight;
            totalWeight += weight;
        }

        int allocated = 0;

        for (int row = 0; row < rowCount; row++)
        {
            float exactCount = (slimeCount - rowCount) * rowWeights[row] / totalWeight;
            int rowCountFloor = Mathf.FloorToInt(exactCount);

            rowAllocations.Add(new MutableRowAllocation(rowCountFloor + 1, exactCount - rowCountFloor));
            allocated += rowCountFloor + 1;
        }

        while (allocated < slimeCount)
        {
            int row = GetBestRemainderRow();
            rowAllocations[row] = rowAllocations[row].WithAddedSlime();
            allocated++;
        }

        int maxWidth = Mathf.Max(1, settings.MaxRowWidth);

        for (int row = 0; row < rowAllocations.Count; row++)
        {
            MutableRowAllocation allocation = rowAllocations[row];
            int baseWidth = Mathf.Min(maxWidth, Mathf.Max(1, allocation.Count));
            int layerCount = Mathf.CeilToInt((float)allocation.Count / baseWidth);

            result.Add(new CrowdRowAllocation(
                allocation.Count,
                baseWidth,
                Mathf.Max(1, layerCount)
            ));
        }

        return result;
    }

    private int GetActiveRowCount(int slimeCount)
    {
        if (slimeCount <= 1)
            return 1;

        if (slimeCount <= 4)
            return 2;

        int compactRows = Mathf.CeilToInt(Mathf.Sqrt(slimeCount));
        return Mathf.Clamp(compactRows, 3, settings.MaxRows);
    }

    private int GetBestRemainderRow()
    {
        int bestRow = 0;
        float bestScore = float.MinValue;
        float centerRow = (rowAllocations.Count - 1) * 0.5f;

        for (int row = 0; row < rowAllocations.Count; row++)
        {
            float centerPriority = 1f / (1f + Mathf.Abs(row - centerRow));
            float score = rowAllocations[row].Remainder + centerPriority * 0.001f;

            if (score <= bestScore)
                continue;

            bestScore = score;
            bestRow = row;
        }

        return bestRow;
    }

    private float GetOvalWeight(float normalizedRow)
    {
        float oval = Mathf.Sin(normalizedRow * Mathf.PI);
        float frontMinimum = Mathf.Lerp(0.35f, 1f, Mathf.SmoothStep(0f, 0.28f, normalizedRow));
        float backTaper = Mathf.Lerp(1f, 0.68f, Mathf.SmoothStep(0.72f, 1f, normalizedRow));

        return Mathf.Max(0.22f, (0.35f + oval * 0.9f) * frontMinimum * backTaper);
    }

    private readonly struct MutableRowAllocation
    {
        public MutableRowAllocation(int count, float remainder)
        {
            Count = count;
            Remainder = remainder;
        }

        public int Count { get; }
        public float Remainder { get; }

        public MutableRowAllocation WithAddedSlime()
        {
            return new MutableRowAllocation(Count + 1, 0f);
        }
    }
}
