using UnityEngine;

public class GateGroupView : MonoBehaviour
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
