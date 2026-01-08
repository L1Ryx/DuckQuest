using UnityEngine;

public class SaveService
{
    private const string SaveFileName = "ducksafar_save.es3";
    private const string RootKey = "SAVE_ROOT";

    public SaveData Data { get; private set; } = new SaveData();

    private ES3Settings Settings => new ES3Settings(SaveFileName);


    public bool HasSave()
    {
        return ES3.KeyExists(RootKey, Settings);
    }

    public void LoadOrCreate()
    {
        if (HasSave())
        {
            Data = ES3.Load<SaveData>(RootKey, Settings);
        }
        else
        {
            Data = new SaveData();
            Save(); // create file immediately (optional)
        }
    }

    public void Save()
    {
        ES3.Save(RootKey, Data, Settings);
    }

    public void Delete()
    {
        ES3.DeleteKey(RootKey, Settings);
        Data = new SaveData();
    }
}