using System.Collections.Generic;
using UnityEngine;

public class EllipseCrowdFormation : ICrowdFormation
{
    private readonly CrowdFormationSettings settings;

    public EllipseCrowdFormation(CrowdFormationSettings settings)
    {
        this.settings = settings;
    }

    public IReadOnlyList<Vector3> GeneratePositions(int count)
    {
        List<Vector3> positions = new();

        if (count <= 0)
            return positions;

        positions.Add(new Vector3(0f, 0f, settings.FrontOffsetZ));

        if (count == 1)
            return positions;

        int placed = 1;
        int ring = 1;

        while (placed < count)
        {
            float radiusX = ring * settings.BaseSpacing * settings.EllipseWidth;
            float radiusZ = ring * settings.BaseSpacing * settings.EllipseDepth;

            float circumferenceEstimate =
                2f * Mathf.PI * Mathf.Sqrt((radiusX * radiusX + radiusZ * radiusZ) * 0.5f);

            int ringCapacity = Mathf.Max(6, Mathf.RoundToInt(circumferenceEstimate / settings.BaseSpacing));
            int placeThisRing = Mathf.Min(ringCapacity, count - placed);

            float angleOffset = ring % 2 == 0 ? 0f : Mathf.PI / placeThisRing;

            for (int i = 0; i < placeThisRing; i++)
            {
                float angle = ((float)i / placeThisRing) * Mathf.PI * 2f + angleOffset;

                float x = Mathf.Cos(angle) * radiusX;
                float z = Mathf.Sin(angle) * radiusZ + settings.FrontOffsetZ;

                Vector2 jitter = GetStableJitter(placed + i) * settings.RandomOffsetAmount;

                positions.Add(new Vector3(
                    x + jitter.x,
                    0f,
                    z + jitter.y
                ));
            }

            placed += placeThisRing;
            ring++;
        }

        ShiftFormationForward(positions);
        return positions;
    }

    private Vector2 GetStableJitter(int index)
    {
        float x = Mathf.PerlinNoise(index * 0.173f, 0.371f) * 2f - 1f;
        float y = Mathf.PerlinNoise(index * 0.521f, 0.137f) * 2f - 1f;
        return new Vector2(x, y);
    }

    private void ShiftFormationForward(List<Vector3> positions)
    {
        if (positions.Count == 0)
            return;

        float maxZ = float.MinValue;

        for (int i = 0; i < positions.Count; i++)
        {
            if (positions[i].z > maxZ)
                maxZ = positions[i].z;
        }

        float targetFrontZ = 0.2f;
        float shift = targetFrontZ - maxZ;

        for (int i = 0; i < positions.Count; i++)
        {
            Vector3 pos = positions[i];
            pos.z += shift;
            positions[i] = pos;
        }
    }
}
