[System.Serializable]
public class SaveData
{
    public int version = 1;
    
    public int highestUnlockedLevelIndex = 0;
    
    // maybe useful
    public string lastKnownScene = "";

    // maybe useful
    public string lastCompletedLevelId = "";
}