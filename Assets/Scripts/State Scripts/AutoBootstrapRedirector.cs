using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoBootstrapRedirector : MonoBehaviour
{
    [SerializeField]
    private string bootstrapSceneName = "Bootstrap";

    private void Awake()
    {
        // If the GameContext already exists, do nothing
        if (Game.IsReady)
            return;

        // Prevent multiple redirectors from firing
        if (SceneManager.GetActiveScene().name == bootstrapSceneName)
            return;

        // Redirect to Bootstrap
        SceneManager.LoadScene(bootstrapSceneName);
    }
}