using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public string CurrentName { get; set; }
    public int CurrentPoints { get; set; }

    public Data[] Leaders;

    void Start()
    {
        if (Instance == null)
        {
            LoadData();

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    [Serializable]
    public class Data
    {
        public string Name;
        public int Points;
    }

    public void SaveData()
    {
        //condition cheking should be in another class

        for (int i = 0; i < Leaders.Length; i++)
        {
            if (CurrentPoints >= Leaders[i].Points)
            {
                Data[] buf = new Data[Leaders.Length];
                for (int j = 0; j < Leaders.Length; j++)
                    buf[j] = Leaders[j];

                for (int j = i; j < Leaders.Length - 1; j++)
                    Leaders[j + 1] = buf[j];

                Data newLeader = new Data();
                newLeader.Name = CurrentName;
                newLeader.Points = CurrentPoints;

                Leaders[i] = newLeader;

                File.WriteAllText(Application.persistentDataPath + "/savedata.json", JsonHelper.ToJson(Leaders));
                return;
            }
        }

    }

    public void LoadData()
    {
        string path = Application.persistentDataPath + "/savedata.json";

        if (File.Exists(path))
        {
            string jsonData = File.ReadAllText(path);
            Leaders = JsonHelper.FromJson<Data>(jsonData);
        }
        else
        {
            Leaders = new Data[5];
            for (int i = 0; i < Leaders.Length; i++)
            {
                Leaders[i] = new Data();
                Leaders[i].Name = "None";
                Leaders[i].Points = 0;
            }
            File.WriteAllText(path, JsonHelper.ToJson(Leaders));
        }
    }

}
