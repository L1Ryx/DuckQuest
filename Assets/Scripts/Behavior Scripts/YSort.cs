using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class YSort : MonoBehaviour
{
    [Header("Use a child transform placed at the object's 'feet'")]
    public Transform sortPoint;

    [Header("Higher value = stronger separation (avoid ties)")]
    public int orderPrecision = 100;

    [Header("Optional constant offset (e.g. ensure shadows render behind)")]
    public int orderOffset = 0;

    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sortPoint == null) sortPoint = transform;
    }

    void LateUpdate()
    {
        // lower Y => larger sortingOrder => drawn in front
        int order = Mathf.RoundToInt(-sortPoint.position.y * orderPrecision) + orderOffset;
        sr.sortingOrder = order;
    }
}