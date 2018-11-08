using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Stats : MonoBehaviour {

    public float Happiness;
    public float Hunger;
    public float Cleanliness;
    public long CurrentTime;
    public long CloseTime;
    public long TimeAway;

    public float HappinessAway;
    public long HungerAway;
    public float CleanlinessAway;

    public PlayerData loadedData;

    public bool FileExists;

    string path;
    string jsonString;

    // Use this for initialization
    void Awake () {
        CurrentTime = System.DateTime.Now.Ticks;
        Load();
        if(FileExists)
        {
            TimeAway = CurrentTime - CloseTime;
            TimeAway = TimeAway / 10000000;
            Debug.Log(TimeAway);

            HungerAway = TimeAway / 15000;
            HappinessAway = TimeAway / 10000;
            CleanlinessAway = TimeAway / 25000;

            Hunger = Hunger - HungerAway;
            Happiness = Happiness - HappinessAway;
            Cleanliness = Cleanliness - CleanlinessAway;
        }
        else
        {

        }
    }
	
	// Update is called once per frame
	void Update () {

        Happiness -= 0.01f;

        if (Happiness >= 100)
        {
            Happiness = 100;
        }
        else if (Happiness <= 0)
        {
            Happiness = 0;
        }

        Hunger -= 0.01f;

        if (Hunger >= 100)
        {
            Hunger = 100;
        }
        else if (Hunger <= 0)
        {
            Hunger = 0;
        }

        Cleanliness -= 0.01f;

        if (Cleanliness >= 100)
        {
            Cleanliness = 100;
        }
        else if (Cleanliness <= 0)
        {
            Cleanliness = 0;
        }
    }

    private void OnApplicationPause(bool pause)
    {
        Save();
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    public void Save()
    {
        //path = Application.streamingAssetsPath + "/playerdata.json";
        //loadedData.Happiness = Happiness;
        //loadedData.Hunger = Hunger;
        //loadedData.Cleanliness = Cleanliness;
        //string newData = JsonUtility.ToJson(loadedData);
        //File.WriteAllText(path, newData);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerinfo.dat");

        PlayerData data = new PlayerData();
        data.Happiness = Happiness;
        Debug.Log("Happiness" + data.Happiness);
        data.Hunger = Hunger;
        data.Cleanliness = Cleanliness;
        data.CloseTime = System.DateTime.Now.Ticks;

        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Save Complete");
    }

    public void Load()
    {
        //path = Application.streamingAssetsPath + "/playerdata.json";
        //jsonString = File.ReadAllText(path);
        //loadedData = JsonUtility.FromJson<PlayerData>(jsonString);

        //Happiness = loadedData.Happiness;
        //Hunger = loadedData.Hunger;
        //Cleanliness = loadedData.Cleanliness;

        if (File.Exists(Application.persistentDataPath + "/playerinfo.dat"))
        {
            FileExists = true;
            Debug.Log("File Found");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerinfo.dat", FileMode.Open, FileAccess.Read);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            Debug.Log("Happiness = " + data.Happiness);

            Happiness = data.Happiness;
            Hunger = data.Hunger;
            Cleanliness = data.Cleanliness;
            CloseTime = data.CloseTime;

            file.Close();
            Debug.Log("File Loaded");
        }
        else
        {
            FileExists = false;
            Debug.Log("File not found");
            Happiness = 100;
            Hunger = 100;
            Cleanliness = 100;
            CurrentTime = 0;
            TimeAway = 0;
            CloseTime = 0;
            HappinessAway = 0;
            HungerAway = 0;
            CleanlinessAway = 0;
        }
    }
    [Serializable]
    public class PlayerData
    {
        public long CloseTime;
        public float Happiness;
        public float Hunger;
        public float Cleanliness;
    }

}
