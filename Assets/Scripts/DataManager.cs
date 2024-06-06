using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public string filePath;
    
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        filePath = Path.Combine(Application.persistentDataPath, "whaleBlood.json");
        Debug.LogWarning(filePath);
    }

    public void SaveData(Data data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Data saved to " + filePath);
    }

    public Data LoadData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            Data data = JsonUtility.FromJson<Data>(json);
            Debug.Log("Data loaded from " + filePath);
            return data;
        }
        else
        {
            Debug.LogWarning("No data file found at " + filePath);
            return new Data();
        }
    }
}