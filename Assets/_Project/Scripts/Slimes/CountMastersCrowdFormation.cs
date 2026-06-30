using System.Collections.Generic;
using UnityEngine;

public class CountMastersCrowdFormation : ICrowdFormation
{
    private readonly CrowdFormationSettings settings;

    public CountMastersCrowdFormation(CrowdFormationSettings settings)
    {
        this.settings = settings;
    }

    public IReadOnlyList<Vector3> GeneratePositions(int count)
    {
        List<Vector3> positions = new();

        if (count <= 0)
            return positions;

        positions.Add(Vector3.zero);

        if (count == 1)
            return positions;

        int placed = 1;
        int row = 1;

        while (placed < count)
        {
            int rowCapacity = GetRowCapacity(row);
            int placeThisRow = Mathf.Min(rowCapacity, count - placed);
            float rowZ = -row * settings.RowSpacing;
            float startX = -((placeThisRow - 1) * settings.BaseSpacing) * 0.5f;

            for (int column = 0; column < placeThisRow; column++)
            {
                int slimeIndex = placed + column;
                Vector2 jitter = GetStableJitter(slimeIndex) * settings.RandomOffsetAmount;

                positions.Add(new Vector3(
                    startX + column * settings.BaseSpacing + jitter.x,
                    0f,
                    rowZ + jitter.y
                ));
            }

            placed += placeThisRow;
            row++;
        }

        return positions;
    }

    private int GetRowCapacity(int row)
    {
        int rawCapacity = 1 + row * 2;
        float ovalLimit = settings.MaxRowWidth - Mathf.Max(0, row - settings.WidestRowIndex);
        int limitedCapacity = Mathf.RoundToInt(Mathf.Min(rawCapacity, ovalLimit));

        return Mathf.Max(1, limitedCapacity);
    }

    private Vector2 GetStableJitter(int index)
    {
        float x = Mathf.PerlinNoise(index * 0.173f, 0.371f) * 2f - 1f;
        float z = Mathf.PerlinNoise(index * 0.521f, 0.137f) * 2f - 1f;
        return new Vector2(x, z);
    }
}
