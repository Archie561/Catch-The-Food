using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public string CurrentName { get; set; }
    public string BestName { get; set; }
    public int CurrentPoints { get; set; }
    public int BestPoints { get; set; }

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    [Serializable]
    class Data
    {
        public string Name;
        public int Points;
    }

    public void SaveData()
    {
        Data data = new Data();
        data.Name = CurrentName;
        data.Points = CurrentPoints;

        string jsonData = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savedata.json", jsonData);
    }

    public void LoadData()
    {
        string path = Application.persistentDataPath + "/savedata.json";
        if (File.Exists(path))
        {
            string jsonData = File.ReadAllText(path);
            Data data = JsonUtility.FromJson<Data>(jsonData);

            BestName = data.Name;
            BestPoints = data.Points;
        }
    }

}
