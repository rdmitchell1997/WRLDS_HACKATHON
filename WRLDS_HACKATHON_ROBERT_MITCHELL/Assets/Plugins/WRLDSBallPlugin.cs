using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using AOT;

/**
 * This class interfaces with the Unity WRLDS iOS Native plugin 
 */
public class WRLDSBallPlugin : Singleton<WRLDSBallPlugin>
{
#if UNITY_IOS

    #region Native Implementation Interface
    [DllImport("__Internal")]
    private static extern void initializer();

    [DllImport("__Internal")]
    private static extern void finalizer();

    // Interaction Delegate Callback Setters
    [DllImport("__Internal")]
    private static extern void set_interaction_delegate_ball_did_bounce_callback(BallDidBounceCallback callback);

    [DllImport("__Internal")]
    private static extern void set_interaction_delegate_ball_did_shake_callback(BallDidShakeCallback callback);

    [DllImport("__Internal")]
    private static extern void set_connection_delegate_ball_did_change_state_callback(BallDidChangeConnectionStateCallback callback);

    // Properties
    [DllImport("__Internal")]
    private static extern void set_low_threshold(double threshold); // Default 30.0

    [DllImport("__Internal")]
    private static extern void set_high_threshold(double threshold); // Default 60.0

    [DllImport("__Internal")]
    private static extern void set_shake_interval(double intervalSeconds); // Default 0.250

    [DllImport("__Internal")]
    private static extern string get_connection_state();

    [DllImport("__Internal")]
    private static extern string get_device_address();

    // Actions
    [DllImport("__Internal")]
    private static extern void start_scanning();

    [DllImport("__Internal")]
    private static extern void stop_scanning();

    [DllImport("__Internal")]
    private static extern void connect_to_ball(string address);

    // Helpers
    [DllImport("__Internal")]
    private static extern void framework_trigger_delegate();

    #endregion


    #region Native Callbacks
    // Define function pointers that can be passed as arguments
    private delegate void BallDidBounceCallback(int type, float sumG);
    private delegate void BallDidShakeCallback(double shakeProgress);
    private delegate void BallDidChangeConnectionStateCallback(int state, string stateMessage);
    #endregion


    #region Native Callback function pointer retainers
    // Holds on to the function pointers so that they cannot be deallocated when GC runs
    private BallDidBounceCallback _ball_did_bounce_callback_holder;
    private BallDidShakeCallback _ball_did_shake_callback_holder;
    private BallDidChangeConnectionStateCallback _ball_did_change_connection_state_callback_holder;
    #endregion

    public delegate void BounceDelegate(int bounceType, float sumG);
    public delegate void ShakeDelegate(float shakeProgress);
    public delegate void FifoDataDelegate(AndroidJavaObject fifoDataObject);
    public delegate void ConnectionStateChangeDelegate(int connectionState, string stateMessage);

    public event BounceDelegate OnBounce;
    public event ShakeDelegate OnShake;
    public event FifoDataDelegate OnFifoData;
    public event ConnectionStateChangeDelegate OnConnectionStateChange;

#elif UNITY_ANDROID

    private AndroidJavaObject pluginClass;
    private WRLDSAndroidPluginCallback pluginCallback;

    public event WRLDSAndroidPluginCallback.BounceDelegate OnBounce
    {
        add { pluginCallback.OnBounce += value; }
        remove { pluginCallback.OnBounce -= value; }
    }
    public event WRLDSAndroidPluginCallback.ShakeDelegate OnShake
    {
        add { pluginCallback.OnShake += value; }
        remove { pluginCallback.OnShake -= value; }
    }
    public event WRLDSAndroidPluginCallback.FifoDataDelegate OnFifoData
    {
        add { pluginCallback.OnFifoData += value; }
        remove { pluginCallback.OnFifoData -= value; }
    }
    public event WRLDSAndroidPluginCallback.ConnectionStateChangeDelegate OnConnectionStateChange
    {
        add { pluginCallback.OnConnectionStateChange += value; }
        remove { pluginCallback.OnConnectionStateChange -= value; }
    }

#endif

    void Awake()
    {
#if UNITY_IOS
        DontDestroyOnLoad(this.gameObject);

        // Store the callback functions
        _ball_did_bounce_callback_holder = new BallDidBounceCallback(_ball_did_bounce_callback);
        _ball_did_shake_callback_holder = new BallDidShakeCallback(_ball_did_shake_callback);
        _ball_did_change_connection_state_callback_holder = new BallDidChangeConnectionStateCallback(_ball_did_change_connection_state_callback);

        // Set native pointers to internal callbacks
        set_interaction_delegate_ball_did_bounce_callback(_ball_did_bounce_callback_holder);
        set_interaction_delegate_ball_did_shake_callback(_ball_did_shake_callback_holder);
        set_connection_delegate_ball_did_change_state_callback(_ball_did_change_connection_state_callback_holder);

        initializer();
#elif UNITY_ANDROID
        DontDestroyOnLoad(this.gameObject);

        // First the SDK needs the activity context, so we create a reference to the UnityPlayer class.
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        if (jc == null)
            Debug.Log("unable to get unity activity class");

        // Second we get a reference to the activity.
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");

        if (jo == null) {
            Debug.Log("unable to get current activity");
        }
        else
        {
            Debug.Log("Current activity read");
        }

        // The SDK needs to be instatiated with a reference to the activity and the Ball Listener callback interface.
        pluginCallback = new WRLDSAndroidPluginCallback();
        pluginClass = new AndroidJavaObject("com.wrlds.sdk.Ball", new object[] { jo, pluginCallback });

        pluginCallback.pluginClass = pluginClass;

#else
        Destroy(this);
#endif
    }


#if UNITY_IOS
    #region Internal Interaction Callbacks
    [MonoPInvokeCallback(typeof(BallDidBounceCallback))]
    private static void _ball_did_bounce_callback(int type, float sumG)
    {
        Debug.Log("WRLDSBall_iOS::BallDidBounce(" + type + ", " + sumG + ")");
        if (WRLDSBallPlugin.Instance.OnBounce != null)
        {
            WRLDSBallPlugin.Instance.OnBounce.Invoke(type, sumG);
        }
    }
    [MonoPInvokeCallback(typeof(BallDidShakeCallback))]
    private static void _ball_did_shake_callback(double shakeProgress)
    {
        // TODO: Trigger Unity based callback
        Debug.Log("WRLDSBall_iOS::BallDidShake(" + shakeProgress + ")");
    }
    [MonoPInvokeCallback(typeof(BallDidChangeConnectionStateCallback))]
    private static void _ball_did_change_connection_state_callback(int state, string stateMessage)
    {
        // TODO: Trigger Unity based callback
        Debug.Log("WRLDSBall_iOS::BallDidChangeConnectionState(" + state + ")");
        if (WRLDSBallPlugin.Instance.OnConnectionStateChange != null)
        {
            WRLDSBallPlugin.Instance.OnConnectionStateChange.Invoke(state, stateMessage);
        }
    }
    #endregion
#endif

    private void _teardown()
    {
#if UNITY_IOS
        set_interaction_delegate_ball_did_bounce_callback(null);
        set_interaction_delegate_ball_did_shake_callback(null);
        set_connection_delegate_ball_did_change_state_callback(null);

        _ball_did_bounce_callback_holder = null;
        _ball_did_shake_callback_holder = null;
#elif UNITY_ANDROID
#endif
    }

    void OnDestroy()
    {
#if UNITY_IOS
        _teardown();
#endif
    }

    void OnApplicationQuit()
    {
#if UNITY_IOS
        _teardown();
#endif
    }

    public void ScanForDevices()
    {
        Debug.Log("ScanForDevices");
#if UNITY_IOS
        start_scanning();
#elif UNITY_ANDROID
        pluginCallback.pluginClass.Call("scanForDevices");
#endif
    }

    public string GetDeviceAddress()
    {
#if UNITY_IOS
        string hash = get_device_address();
#elif UNITY_ANDROID
		string hash = pluginCallback.pluginClass.Call<string>("getDeviceAddress");
#else
        string hash = "00:00:00:00:00:00";
#endif
        Debug.Log("Device Address: " + hash);
        return hash;
    }

    public void DisconnectDevice() {
#if UNITY_IOS
//
#elif UNITY_ANDROID
		pluginCallback.pluginClass.Call("disconnectDevice");
		pluginCallback.pluginClass.Call("destroy");
#endif
	}

	public void ConnectDevice(string hash)
	{
#if UNITY_IOS
//
#elif UNITY_ANDROID
		pluginCallback.pluginClass.Call("connectDevice", hash);
#endif
	}

#if UNITY_ANDROID
    public class WRLDSAndroidPluginCallback : AndroidJavaProxy
    {

        public delegate void BounceDelegate(int bounceType, float sumG);
        public delegate void ShakeDelegate(float shakeProgress);
        public delegate void FifoDataDelegate(AndroidJavaObject fifoDataObject);
        public delegate void ConnectionStateChangeDelegate(int connectionState, string stateMessage);

        public event BounceDelegate OnBounce;
        public event ShakeDelegate OnShake;
        public event FifoDataDelegate OnFifoData;
        public event ConnectionStateChangeDelegate OnConnectionStateChange;

        public WRLDSAndroidPluginCallback() : base("com.wrlds.sdk.Ball$Listener") { }
        public AndroidJavaObject pluginClass;

        public void onBounce(int bounceType, float sumG)
        {
            if (OnBounce != null)
            {
                OnBounce.Invoke(bounceType, sumG);
            }
        }

        public void onShake(float shakeProgress)
        {
            if (OnShake != null)
            {
                OnShake.Invoke(shakeProgress);
            }
        }

        public void onFifoData(AndroidJavaObject fifoDataObject)
        {
            if (OnFifoData != null)
            {
                OnFifoData.Invoke(fifoDataObject);
            }
        }

        public void onConnectionStateChange(int connectionState, string stateMessage)
        {
            if (OnConnectionStateChange != null)
            {
                OnConnectionStateChange.Invoke(connectionState, stateMessage);
            }
        }
    }

#endif

}

