using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugDummy : MonoBehaviour
{
    private int callCount = 1;

    public void PrintDebugMessage()
    {
        Debug.Log("Debug Dummy: Call #" + callCount.ToString());
        callCount++;
    }

    /*
     * SHAWN REFACTOR THIS INTO ANOTHER GAMEOBJECT PLEASE IM BEGGING
     */
    public void ReloadSameScene()
    {
        if (Game.IsReady && Game.Ctx != null)
        {
            Game.Ctx.Audio?.StopGlobalAmbience(immediate: false);
            Game.Ctx.LevelState?.Reset();
            Game.Ctx.InteractionLock?.ForceClear();
            // Optional: reset other per-run state here (inventory, selections, etc.)
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
