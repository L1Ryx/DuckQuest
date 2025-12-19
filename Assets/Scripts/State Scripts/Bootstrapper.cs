using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private string firstSceneName = "Stage";

    private void Start()
    {
        SceneManager.LoadScene(firstSceneName);
    }
}
