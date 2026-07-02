using System.Collections.Generic;
using UnityEngine;

public class CountMastersCrowdFormation : ICrowdFormation
{
    private readonly CrowdFormationSettings settings;
    private readonly ICrowdRowAllocator rowAllocator;

    public CountMastersCrowdFormation(CrowdFormationSettings settings, ICrowdRowAllocator rowAllocator)
    {
        this.settings = settings;
        this.rowAllocator = rowAllocator;
    }

    public IReadOnlyList<Vector3> GeneratePositions(int count)
    {
        List<Vector3> positions = new();

        if (count <= 0)
            return positions;

        IReadOnlyList<CrowdRowAllocation> rowAllocations = rowAllocator.AllocateRows(count);
        int placed = 0;

        for (int row = 0; row < rowAllocations.Count; row++)
        {
            CrowdRowAllocation allocation = rowAllocations[row];
            float rowZ = -row * settings.RowSpacing;

            for (int indexInRow = 0; indexInRow < allocation.Count; indexInRow++)
            {
                int slimeIndex = placed + indexInRow;
                int layer = indexInRow / allocation.BaseWidth;
                int column = indexInRow % allocation.BaseWidth;
                int layerWidth = Mathf.Min(allocation.BaseWidth, allocation.Count - layer * allocation.BaseWidth);
                float layerZOffset = (layer - (allocation.LayerCount - 1) * 0.5f) * settings.RowSpacing * 0.22f;
                float rowStaggerOffset = row % 2 == 0 ? 0f : settings.BaseSpacing * 0.35f;
                float layerStaggerOffset = layer % 2 == 0 ? 0f : settings.BaseSpacing * 0.18f;
                float organicRowOffset = GetOrganicRowOffset(row) * settings.BaseSpacing * 0.12f;
                float organicLayerOffset = GetOrganicLayerOffset(row, layer) * settings.BaseSpacing * 0.08f;
                float startX = -((layerWidth - 1) * settings.BaseSpacing) * 0.5f;
                Vector2 jitter = GetStableJitter(slimeIndex) * settings.RandomOffsetAmount;

                positions.Add(new Vector3(
                    startX + column * settings.BaseSpacing + rowStaggerOffset + layerStaggerOffset + organicRowOffset + organicLayerOffset + jitter.x,
                    0f,
                    rowZ + layerZOffset + jitter.y
                ));
            }

            placed += allocation.Count;
        }

        CenterPositions(positions);
        return positions;
    }

    private void CenterPositions(List<Vector3> positions)
    {
        if (positions.Count == 0)
            return;

        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minZ = float.MaxValue;
        float maxZ = float.MinValue;

        for (int i = 0; i < positions.Count; i++)
        {
            minX = Mathf.Min(minX, positions[i].x);
            maxX = Mathf.Max(maxX, positions[i].x);
            minZ = Mathf.Min(minZ, positions[i].z);
            maxZ = Mathf.Max(maxZ, positions[i].z);
        }

        float xOffset = -(minX + maxX) * 0.5f;
        float zOffset = -(minZ + maxZ) * 0.5f;

        for (int i = 0; i < positions.Count; i++)
        {
            Vector3 position = positions[i];
            position.x += xOffset;
            position.z += zOffset;
            positions[i] = position;
        }
    }

    private Vector2 GetStableJitter(int index)
    {
        float x = Mathf.PerlinNoise(index * 0.173f, 0.371f) * 2f - 1f;
        float z = Mathf.PerlinNoise(index * 0.521f, 0.137f) * 2f - 1f;
        return new Vector2(x, z);
    }

    private float GetOrganicRowOffset(int row)
    {
        return Mathf.PerlinNoise(row * 0.417f, 0.913f) * 2f - 1f;
    }

    private float GetOrganicLayerOffset(int row, int layer)
    {
        return Mathf.PerlinNoise(row * 0.317f, layer * 0.619f + 0.271f) * 2f - 1f;
    }

}
