using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ball : MonoBehaviour
{
    public Stats stats;
    public Rigidbody rb;
    public bool ground = false;
    public sceneChange scenes;

    public Animator ball;
    public Animator food;

    private float ballG = 0.0f;

    // Use this for initialization
    void Start()
    {
        
        scenes = GameObject.FindObjectOfType<sceneChange>();
        scenes.m_Scene = SceneManager.GetActiveScene();
        stats = GameObject.FindObjectOfType<Stats>();
        // By calling the singelton instance of WRLDSBallPlugin, we instantiate 
        // the plugin that communicate with the ball.

        // Let's scan for devices!
#if UNITY_IOS
        // Because of the way iOS takes a bit of time to instantiate the BLE 
        // stack, we have to wait a couple of seconds. In this case we wait 
        // 3 seconds before we call a method that start scanning for balls.
        // That is usually enough time.
        Singleton<WRLDSBallPlugin>.Instance.OnConnectionStateChange += ConnectionStateChangeHandler;
        Singleton<WRLDSBallPlugin>.Instance.ScanForDevices();

#elif UNITY_ANDROID
        // We add a handler for connection state change so we know when 
        // the SDK is ready for scanning for balls or if the connection is 
        // dropped.
        Singleton<WRLDSBallPlugin>.Instance.OnConnectionStateChange += ConnectionStateChangeHandler;
#endif
        // We also add a handler for the bounce event to make some fun things happen.
        Singleton<WRLDSBallPlugin>.Instance.OnBounce += BounceHandler;

        rb = GetComponent<Rigidbody>();
    }

    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        Singleton<WRLDSBallPlugin>.Instance.ScanForDevices();
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.L))
        {
            stats.Load();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            stats.Save();
        }


        if (Input.GetKeyDown("space"))
        {

            if (scenes.m_Scene.name == "Bedroom")
            {
                stats.Happiness += 3;
            }

            if (scenes.m_Scene.name == "Bathroom")
            {
                stats.Cleanliness += 3;
            }

            if (scenes.m_Scene.name == "SDKintegration")
            {
                stats.Hunger += 3;
            }
        }

        if (ballG > 0.0f && ground)
        {
            ground = false;
            rb.velocity = new Vector3(0, ballG / 6, 0);
            ballG = 0.0f;
            Debug.Log("jump");

            if (scenes.m_Scene.name == "Bedroom")
            {
                stats.Happiness += 3;
            }

            if (scenes.m_Scene.name == "Bathroom")
            {
                stats.Cleanliness += 3;
            }

            if (scenes.m_Scene.name == "SDKintegration")
            {
                stats.Hunger += 3;
            }
        }
    }

    void BounceHandler (int type, float sumG) {
        ballG = sumG;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (scenes.m_Scene.name == "Bedroom")
        {
            stats.Happiness += 3;
            ground = true;
        }

        if (scenes.m_Scene.name == "Bathroom")
        {
            stats.Cleanliness += 3;
            ground = true;
        }

        if (scenes.m_Scene.name == "SDKintegration")
        {
            stats.Hunger += 3;
            ball.SetTrigger("Eating");
            food.SetTrigger("Eating");
            ground = true;

        }
    }

    void ConnectionStateChangeHandler (int connectionState, string stateMessage) {
        Debug.Log("ConnectionStateChange: " + connectionState);
        if (connectionState == 2) {
            Singleton<WRLDSBallPlugin>.Instance.ScanForDevices();
        } else if (connectionState == 6) {
            string deviceAddress = Singleton<WRLDSBallPlugin>.Instance.GetDeviceAddress();
            Debug.Log("Device Address: " + deviceAddress);
        }
    }
}
