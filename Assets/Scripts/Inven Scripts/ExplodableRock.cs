using UnityEngine;

public class ExplodableRock : InventoryCostInteractable
{
    [Header("Explosion")]
    [SerializeField] private GameObject destroyedVersion;
    [SerializeField] private float destroyDelay = 0f;

    protected override void OnPaymentSucceeded(GameObject interactor)
    {
        // Disable collisions immediately
        foreach (var col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        if (destroyedVersion != null)
        {
            Instantiate(destroyedVersion, transform.position, Quaternion.identity);
        }

        Destroy(gameObject, destroyDelay);
    }
}