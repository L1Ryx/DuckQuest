using Unity.VisualScripting;
using UnityEngine;

public class DebugDummy : MonoBehaviour
{
    private int callCount = 1;

    public void PrintDebugMessage()
    {
        Debug.Log("Debug Dummy: Call #" + callCount.ToString());
        callCount++;
    }
}
