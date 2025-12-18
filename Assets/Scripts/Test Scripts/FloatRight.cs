using UnityEngine;

public class FloatRight : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
    }
}
