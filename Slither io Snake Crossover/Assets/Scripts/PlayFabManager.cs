using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using System;
using PlayFab.ClientModels;

public class PlayFabManager : MonoBehaviour
{
    // used for device ID
    public static string android_id = string.Empty; // device ID to use with PlayFab login
    public static string custom_id = string.Empty; // custom id for other platforms

    public static PlayFabManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(this);
        }
    }
    /// <summary>
    /// Get an id depending on the device, specific for Android or 
    /// </summary>
    /// <param name="silent">Suppresses the error</param>
    /// <returns></returns>
    public static bool GetDeviceId(bool silent = false)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
#if UNITY_ANDROID
            //http://answers.unity3d.com/questions/430630/how-can-i-get-android-id-.html
            AndroidJavaClass clsUnity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject objActivity = clsUnity.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject objResolver = objActivity.Call<AndroidJavaObject>("getContentResolver");
            AndroidJavaClass clsSecure = new AndroidJavaClass("android.provider.Settings$Secure");
            android_id = clsSecure.CallStatic<string>("getString", objResolver, "android_id");
#endif
            return true;
        }
        else
        {
            custom_id = SystemInfo.deviceUniqueIdentifier;
            return false;
        }
    }

    /// <summary>
    /// Try to login with Android device ID
    /// </summary>
    /// <param name="onSuccess">Method to call on success</param>
    /// <param name="onFail">M;ethod to call on fail</param>
    public void AndroidLogin(Action<LoginResult> onSuccess, Action<PlayFabError> onFail)
    {
        LoginWithAndroidDeviceIDRequest request = new LoginWithAndroidDeviceIDRequest
        {
            AndroidDeviceId = android_id,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithAndroidDeviceID(request, onSuccess, onFail);
    }

    /// <summary>
    /// Login with generic device ID
    /// </summary>
    /// <param name="onSuccess">Method to call on success</param>
    /// <param name="onFail">Method to call on fail</param>
    public void Login(Action<LoginResult> onSuccess, Action<PlayFabError> onFail)
    {
        LoginWithCustomIDRequest request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,

        };
        PlayFabClientAPI.LoginWithCustomID(request, onSuccess, onFail);
    }

    /// <summary>
    /// Upload own statistics to Playfab
    /// </summary>
    /// <param name="value">Score to submit</param>
    public void SubmitStatistics(int value)
    {

        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>{
               new StatisticUpdate
               {
                   StatisticName = "Accumulated Points",
                   Value = value
               }, new StatisticUpdate
               {
                   StatisticName = "Max Points",
                   Value = value
               }

            }
        }, result => OnStatisticsUpdated(result), FailureCallback);
        GetStatistics();
    }

    private void OnStatisticsUpdated(UpdatePlayerStatisticsResult updateResult)
    {
        Debug.Log("Successfully submitted high score");
    }

    private void FailureCallback(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    /// <summary>
    /// Download Playfab own statistics
    /// </summary>
    public void GetStatistics()
    {
        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(),
            OnGetStatistics,
            error => Debug.LogError(error.GenerateErrorReport())
            );
    }

    /// <summary>
    /// Update GameManager highscore
    /// </summary>
    /// <param name="result">Player Statiscitcs Result</param>
    private void OnGetStatistics(GetPlayerStatisticsResult result)
    {
        foreach (var eachStat in result.Statistics)
        {
            if (eachStat.StatisticName == "Max Points")
            {
                GameManager.instance.maxPoints = eachStat.Value;
                Debug.Log("Updated max value: " + eachStat.Value);
            }
        }

    }
}
