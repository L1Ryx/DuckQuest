using UnityEngine;

public class GameContext : MonoBehaviour
{
    [Header("State References")]
    public PlayerData PlayerData { get; private set; }

    public ItemDatabase ItemDb => itemDatabase;
    public InventoryModel Inventory { get; private set; }
    public InventorySelectionModel InventorySelection { get; private set; }

    [Header("Databases")] [SerializeField] private ItemDatabase itemDatabase;

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
        Inventory = new InventoryModel(new InventoryData());
        InventorySelection = new InventorySelectionModel(Inventory);
    }
}