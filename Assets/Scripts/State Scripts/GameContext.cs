using UnityEngine;

public class GameContext : MonoBehaviour
{
    [Header("State References")]
    public PlayerData PlayerData { get; private set; }

    private void Awake()
    {
        if (Game.IsReady && Game.Ctx != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        
        InitializeRuntimeState();

        Game.SetContext(this);
    }

    private void InitializeRuntimeState()
    {
        PlayerData = new PlayerData();
    }
}