using UnityEngine;

public class DebugCanvas : MonoBehaviour
{
    [Header("Refs")] [SerializeField] private GameObject[] children;

    [Header("Settings")] [SerializeField] private bool showOnStart = false;
    private bool isActive;

    private void Awake()
    {
        foreach (GameObject child in children)
        {
            if (showOnStart)
            {
                isActive = true;
                child.SetActive(true);
            }
            else
            {
                isActive = false;
                child.SetActive(false);
            }
            
        } 
    }

    public void SetCanvasActive(bool active)
    {
        foreach (GameObject child in children)
        {
            child.SetActive(active);
            isActive = active;
        }
    }

    public void ToggleCanvasActive()
    {
        SetCanvasActive(!isActive);
    }

}
