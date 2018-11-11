using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour {

    public float Happiness;
    public float Hunger;
    public float Cleanliness;

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(this);

        PlayerPrefs.SetFloat("Happiness", 100);
        PlayerPrefs.SetFloat("Hunger", 100);
        PlayerPrefs.SetFloat("Cleanliness", 100);

        PlayerPrefs.GetFloat("Happiness", Happiness);
        PlayerPrefs.GetFloat("Hunger", Hunger);
        PlayerPrefs.GetFloat("Cleanliness", Cleanliness);
    }
	
	// Update is called once per frame
	void Update () {
        Happiness -= 0.01f;

        if (Happiness > 100)
        {
            Happiness = 100;
        }

        Hunger -= 0.01f;

        if (Hunger > 100)
        {
            Hunger = 100;
        }

        Cleanliness -= 0.01f;

        if (Cleanliness > 100)
        {
            Cleanliness = 100;
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
        PlayerPrefs.SetFloat("Happiness", Happiness);
        PlayerPrefs.SetFloat("Hunger", Hunger);
        PlayerPrefs.SetFloat("Cleanliness", Cleanliness);
        PlayerPrefs.Save();
    }
}
