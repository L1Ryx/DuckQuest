using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Camera))]
public class CameraEaseInOnLevelStart : MonoBehaviour
{
    [Header("Position Ease")]
    [SerializeField] private bool easePosition = true;
    [SerializeField] private Vector3 startOffset = new Vector3(0f, 0.25f, 0f);

    [Header("Ortho Zoom Ease")]
    [SerializeField] private bool easeOrthoSize = true;
    [SerializeField] private float orthoSizeOffset = 0.35f;

    [Header("Timing")]
    [SerializeField] private float duration = 0.9f;
    [SerializeField] private float startDelay = 0.0f;
    [SerializeField] private Ease ease = Ease.OutCubic;
    [SerializeField] private bool useUnscaledTime = true;

    private Camera cam;
    private Tween posTween;
    private Tween sizeTween;

    private Vector3 targetPos;
    private float targetOrthoSize;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        targetPos = transform.position;
        targetOrthoSize = cam.orthographicSize;
    }

    private void OnDisable()
    {
        posTween?.Kill();
        sizeTween?.Kill();
    }

    // Hook this to LevelStartedEvent via GameEventListener.Response
    public void Play()
    {
        posTween?.Kill();
        sizeTween?.Kill();

        // Reset targets in case something moved camera pre-start (e.g., spawn system).
        targetPos = transform.position;
        targetOrthoSize = cam.orthographicSize;

        if (easePosition)
        {
            transform.position = targetPos + startOffset;

            posTween = transform.DOMove(targetPos, duration)
                .SetDelay(startDelay)
                .SetEase(ease)
                .SetUpdate(useUnscaledTime);
        }

        if (easeOrthoSize && cam.orthographic)
        {
            cam.orthographicSize = targetOrthoSize + orthoSizeOffset;

            sizeTween = cam.DOOrthoSize(targetOrthoSize, duration)
                .SetDelay(startDelay)
                .SetEase(ease)
                .SetUpdate(useUnscaledTime);
        }
    }
}
