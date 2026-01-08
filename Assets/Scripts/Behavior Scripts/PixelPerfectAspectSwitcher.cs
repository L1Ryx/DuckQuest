using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(PixelPerfectCamera))]
public class PixelPerfectAspectSwitcher : MonoBehaviour
{
    [Header("Reference resolutions (pick the same 'height scale')")]
    public Vector2Int ref16x9 = new Vector2Int(1920, 1080);
    public Vector2Int ref16x10 = new Vector2Int(1920, 1200);

    [Tooltip("Treat aspect ratios within this tolerance as 16:10.")]
    public float aspectTolerance = 0.02f;

    private PixelPerfectCamera ppc;
    private int lastW, lastH;

    private void Awake()
    {
        ppc = GetComponent<PixelPerfectCamera>();
        Apply();
    }

    private void Update()
    {
        // Handle window/fullscreen changes
        if (Screen.width != lastW || Screen.height != lastH)
            Apply();
    }

    private void Apply()
    {
        lastW = Screen.width;
        lastH = Screen.height;

        float aspect = (float)Screen.width / Screen.height;

        float a16x9 = 16f / 9f;
        float a16x10 = 16f / 10f;

        bool is16x10 = Mathf.Abs(aspect - a16x10) <= aspectTolerance;

        var chosen = is16x10 ? ref16x10 : ref16x9;

        ppc.refResolutionX = chosen.x;
        ppc.refResolutionY = chosen.y;
    }
}