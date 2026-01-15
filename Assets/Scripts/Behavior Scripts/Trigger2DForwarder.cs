using UnityEngine;

public class Trigger2DForwarder : MonoBehaviour
{
    [SerializeField] private FragileBridge bridge;

    private void Awake()
    {
        if (bridge == null)
            bridge = GetComponentInParent<FragileBridge>();
    }

    private void OnTriggerEnter2D(Collider2D other) => bridge?.OnDetectorEnter(other);
    private void OnTriggerExit2D(Collider2D other) => bridge?.OnDetectorExit(other);
}