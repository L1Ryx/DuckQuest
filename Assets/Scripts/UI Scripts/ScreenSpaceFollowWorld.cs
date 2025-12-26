using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ScreenSpaceFollowWorld : MonoBehaviour
{
    [SerializeField] private Camera worldCamera;
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private Transform worldAnchor;

    [SerializeField] private Vector2 screenOffsetPx = new Vector2(0f, 24f);
    [SerializeField] private bool snapToPixels = true;

    private RectTransform rt;

    private void Awake()
    {
        rt = (RectTransform)transform;
        if (worldCamera == null) worldCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (worldCamera == null || canvasRect == null || worldAnchor == null)
            return;

        Vector3 screen = worldCamera.WorldToScreenPoint(worldAnchor.position);
        if (screen.z < 0f) return;

        screen.x += screenOffsetPx.x;
        screen.y += screenOffsetPx.y;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screen, null, out Vector2 localPoint
        );

        if (snapToPixels)
        {
            localPoint.x = Mathf.Round(localPoint.x);
            localPoint.y = Mathf.Round(localPoint.y);
        }

        rt.anchoredPosition = localPoint;
    }

    public void Init(Camera cam, RectTransform canvas, Transform anchor)
    {
        worldCamera = cam;
        canvasRect = canvas;
        worldAnchor = anchor;
    }
}