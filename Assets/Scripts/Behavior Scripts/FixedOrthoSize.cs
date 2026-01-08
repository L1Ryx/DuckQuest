using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class FixedOrthoSize : MonoBehaviour
{
    [Tooltip("Must match your sprite Pixels Per Unit.")]
    public int pixelsPerUnit = 64;

    [Tooltip("Vertical pixel height you want visible in gameplay (design height).")]
    public int targetVerticalPixels = 1080;

    private Camera cam;

    private void Awake()
    {
        EnsureCamera();
        Apply();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // OnValidate runs in the editor before Awake, so we must reacquire the Camera here.
        EnsureCamera();

        // Avoid spamming errors if inspector is mid-edit
        if (pixelsPerUnit <= 0 || targetVerticalPixels <= 0)
            return;

        Apply();
    }
#endif

    private void EnsureCamera()
    {
        if (cam == null)
            cam = GetComponent<Camera>();
    }

    private void Apply()
    {
        if (cam == null)
            return;

        cam.orthographic = true;

        float targetVerticalWorldUnits = (float)targetVerticalPixels / pixelsPerUnit;
        cam.orthographicSize = targetVerticalWorldUnits * 0.5f;
    }
}