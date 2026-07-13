using UnityEngine;

public class LevelChunkView : MonoBehaviour
{
    [SerializeField] private float length = 20f;
    [SerializeField] private Transform exitPoint;

    public float Length
    {
        get
        {
            if (exitPoint != null)
                return Mathf.Max(0.1f, exitPoint.localPosition.z);

            return Mathf.Max(0.1f, length);
        }
    }
}
