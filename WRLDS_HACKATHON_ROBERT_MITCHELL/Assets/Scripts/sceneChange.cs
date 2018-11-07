using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneChange : MonoBehaviour {
    public Stats stats;
    public string Bathroom, Bedroom, SDKintegration;
    public Scene m_Scene;

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(this);
        stats = GameObject.FindObjectOfType<Stats>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
