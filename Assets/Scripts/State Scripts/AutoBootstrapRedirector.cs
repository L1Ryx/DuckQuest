using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoBootstrapRedirector : MonoBehaviour
{
    [SerializeField]
    private string bootstrapSceneName = "Bootstrap";

    private void Awake()
    {
        if (Game.IsReady) return;

        var active = SceneManager.GetActiveScene().name;
        if (active == bootstrapSceneName) return;

        // Remember where we wanted to go
        BootstrapHandoff.PendingSceneName = active;

        // Load bootstrap
        SceneManager.LoadScene(bootstrapSceneName);
    }
    public static class BootstrapHandoff
    {
        public static string PendingSceneName;
    }

}