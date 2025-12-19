using UnityEngine;

public class WorldPrompt : MonoBehaviour, IProximityHighlightable
{
    [SerializeField] private GameObject promptRoot;

    private void Reset()
    {
        promptRoot = gameObject;
    }

    private void Awake()
    {
        if (promptRoot != null)
            promptRoot.SetActive(false);
    }

    public void SetIsClosest(bool isClosest)
    {
        if (promptRoot != null)
            promptRoot.SetActive(isClosest);
    }
}