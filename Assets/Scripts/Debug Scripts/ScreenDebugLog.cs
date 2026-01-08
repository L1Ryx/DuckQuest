using UnityEngine;

public class ScreenDebugLog : MonoBehaviour
{
    private int lastW, lastH;
    void Start()
    {
        lastW = Screen.width;
        lastH = Screen.height;
        Debug.Log($"START: {Screen.width}x{Screen.height} fullscreen={Screen.fullScreen} mode={Screen.fullScreenMode}");
    }

    void Update()
    {
        if (Screen.width != lastW || Screen.height != lastH)
        {
            lastW = Screen.width;
            lastH = Screen.height;
            Debug.Log($"RES CHANGE: {Screen.width}x{Screen.height} fullscreen={Screen.fullScreen} mode={Screen.fullScreenMode}");
        }
    }
}