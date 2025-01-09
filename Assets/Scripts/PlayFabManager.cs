using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class LevelData
{
    public string theme;
    public List<string> words;
}

public class PlayFabManager : MonoBehaviour
{
    #region Singleton

    public static PlayFabManager instance;

    #endregion

    #region Private Variables

    private DateTime    serverDate;
    private NewLevel    dailyLevel = null;

    #endregion

    #region Public Properties

    public NewLevel     DailyLevel  { get { return dailyLevel; } }
    public DateTime     ServerDate
    {
        get { return serverDate; }
        private set
        {
            serverDate = value;
        }
    }

    #endregion

    #region Inherited Functions

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    //public void Start()
    //{
    //    DeviceLogin();
    //}

    #endregion


    public void DeviceLogin()
    {
        var req = new LoginWithCustomIDRequest
        {
            CustomId        = SystemInfo.deviceUniqueIdentifier,
            CreateAccount   = true
        };

        PlayFabClientAPI.LoginWithCustomID(
            req,
            DeviceLogin_Success,
            DeviceLogin_Failure
        );
    }

    public void DeviceLogin_Success(LoginResult result)
    {
        PlayFabClientAPI.GetTime(
            new GetTimeRequest(),
            GetServerTime_Success,
            GetServerTime_Failure
        );
    }

    public void GetServerTime_Success(GetTimeResult result)
    {
        if (ServerDate.Date == result.Time.Date)
            return;

        ServerDate              = result.Time;
        string lookupString     = ServerDate.ToString("yyyy_MM_dd");

        Debug.Log("Looking for level: " + lookupString);

        GetTitleDataRequest req = new GetTitleDataRequest();
        req.Keys                = new List<string>() { lookupString };

        PlayFabClientAPI.GetTitleData(
            req,
            LoadDailyLevel_Success,
            LoadDailyLevel_Failure
        );
    }

    private void LoadDailyLevel_Success(GetTitleDataResult result)
    {
        string jsonString       = result.Data[ServerDate.ToString("yyyy_MM_dd")];

        LevelData l             = JsonUtility.FromJson<LevelData>(jsonString);

        dailyLevel              = new NewLevel(LevelCategory.DAILY, l.theme, -1, l.words, "");

        GameManager.instance.SaveData.NewDay_ResetTimeAndFoundList();

        this.PostNotification(Notifications.DAILY_PUZZLE_LOADED_FROM_SERVER);
    }

    #region PlayFab Failure Handlers

    private void DeviceLogin_Failure(PlayFabError e)
    {
        Debug.Log("Device Login Failed: " + e.ErrorMessage);
    }

    private void GetServerTime_Failure(PlayFabError e)
    {
        Debug.Log("Server Time not retrieved: " + e.ErrorMessage);
    }

    private void LoadDailyLevel_Failure(PlayFabError e)
    {
        Debug.Log("Issue loading daily level: " + e.ErrorMessage);
    }

    #endregion
}
