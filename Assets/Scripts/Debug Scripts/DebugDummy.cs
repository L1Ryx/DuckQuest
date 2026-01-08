using System;
using IngameDebugConsole;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class DebugDummy : MonoBehaviour
{
    private int callCount = 1;
    [Header("Events")] [SerializeField] private UnityEvent OnToggleNoclip;

    private void Awake()
    {
        RegisterConsoleCommands();
    }

    void RegisterConsoleCommands()
    {
        DebugLogConsole.AddCommand("/noclip", "Toggles noclip for the player", ToggleNoclip);
        DebugLogConsole.AddCommand("/restart", "Reloads the current scene", ReloadSameScene);
    }

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
            Game.Ctx.Inventory.Clear();
            // Optional: reset other per-run state here (inventory, selections, etc.)
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ToggleNoclip()
    {
        OnToggleNoclip?.Invoke();
    }
}
