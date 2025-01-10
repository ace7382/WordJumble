using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager   instance;

    #endregion

    #region Private Consts

    private const string SAVE_DIRECTORY             = "/SaveData/";
    private const string FILE_NAME                  = "abc.sav";

    #endregion

    #region Private Variables

    private float               time;
    private Button              timerButton;
    private bool                paused              = false;
    private LevelCategory       currentCategory;

    private SaveFile            saveData;

    #endregion

    #region Public Properties

    public LevelCategory    CurrentLevelCategory    { get { return currentCategory; } set { currentCategory = value; } }
    public SaveFile         SaveData                { get { return saveData; } }

    #endregion

    #region Inherited Functions

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    public void Start()
    {
        //if (!LoadSaveData())
        //    saveData = new SaveFile();

        LoadSaveData();
    }

    public void Update()
    {
        if (timerButton == null || paused)
            return;

        time += Time.deltaTime;

        var minutes         = time / 60;
        var seconds         = time % 60;
        //var fraction        = (time * 100) % 100;
        //timerButton.text    = string.Format("{0:00} : {1:00} : {2:000}", minutes, seconds, fraction);
        timerButton.text    = string.Format("{0:00} : {1:00}", minutes, seconds);

        if (time > 0f)
            SaveDailyTime();
    }

    #endregion

    #region Public Functions

    public void SetTime(Button ve, bool resetTime = false, float startingTime = 0f)
    {
        Debug.Log("Setting Time: " + resetTime);

        if (resetTime)
            time    = 0f;
        else
            time    = startingTime;

        timerButton = ve;
        paused      = false;
    }

    public void StopTimer()
    {
        Debug.Log("Stopping timer");

        time        = 0f;
        timerButton = null;
        paused      = true;
    }

    public void PauseTimer()
    {
        paused      = true;
    }

    public void SaveDailyTime()
    {
        saveData.DailyPuzzleTimeInSeconds = time;
    }

    public void ResetDailyTime()
    {
        saveData.DailyPuzzleTimeInSeconds = 0f;
    }

    public NewLevel GetNextLevel(NewLevel currentLevel)
    {
        int nextIndex = LevelDefinitions.ALL_LEVELS.FindIndex(x => x.LevelNumber == currentLevel.LevelNumber + 1 && x.Category == currentLevel.Category);

        if (nextIndex == -1)    return null;
        else return             LevelDefinitions.ALL_LEVELS[nextIndex];
    }

    //public void SaveGame()
    //{
    //    string dir = Application.persistentDataPath + SAVE_DIRECTORY;

    //    if (!Directory.Exists(dir))
    //        Directory.CreateDirectory(dir);

    //    string json = JsonUtility.ToJson(saveData, true);

    //    File.WriteAllText(dir + FILE_NAME, json);

    //    Debug.Log("Game Saved to " + dir + FILE_NAME);

    //    NewSave();
    //}

    public void SaveGame()
    {
        Dictionary<string, object>  finalData   = new Dictionary<string, object>();
        List<object> levelProgress              = new List<object>();
        List<object> secretProgress             = new List<object>();

        foreach (KeyValuePair<LevelCategory, Dictionary<int, List<bool>>> pair in SaveData.LevelProgress)
        {
            Dictionary<string, object> catData  = new Dictionary<string, object>();

            catData["Category"]                 = (int)pair.Key;

            List<object> progress               = new List<object>();

            foreach (KeyValuePair<int, List<bool>> progPair in pair.Value)
            {
                Dictionary<string, object> progData = new Dictionary<string, object>();

                progData["LevelNum"]            = progPair.Key;
                progData["Progress"]            = progPair.Value;

                progress.Add(progData);
            }

            catData["ProgressData"]             = progress;

            levelProgress.Add(catData);
        }

        foreach(KeyValuePair<LevelCategory, Dictionary<int, bool>> pair in SaveData.SecretWordProgress)
        {
            Dictionary<string, object> catData  = new Dictionary<string, object>();

            catData["Category"]                 = pair.Key;
            catData["ProgressData"]             = pair.Value;

            secretProgress.Add(catData);
        }

        finalData["CompletedDailyPuzzles"]      = SaveData.CompletedDailyPuzzles;
        finalData["TodaysDailyProgress"]        = SaveData.TodaysDailyProgress;
        finalData["DailyPuzzleTimeInSeconds"]   = SaveData.DailyPuzzleTimeInSeconds;
        finalData["LevelProgress"]              = levelProgress;
        finalData["SecretWordProgress"]         = secretProgress;

        string dir = Application.persistentDataPath + SAVE_DIRECTORY;

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        string fileContents = Utilities.ConvertToJsonString(finalData);

        File.WriteAllText(dir + FILE_NAME, fileContents);
    }

    #endregion

    #region Private Functions

    private bool LoadSaveData()
    {
        //string filePath     = Application.persistentDataPath + SAVE_DIRECTORY + FILE_NAME;

        //Debug.Log("Loading Save From: " + filePath);

        //if (File.Exists(filePath))
        //{
        //    string json     = File.ReadAllText(filePath);
        //    saveData        = JsonUtility.FromJson<SaveFile>(json);

        //    Debug.Log(json);

        //    return true;
        //}

        //return false;

        saveData        = new SaveFile();

        string filePath = Application.persistentDataPath + SAVE_DIRECTORY + FILE_NAME;

        if (File.Exists(filePath))
        {
            string      contents        = File.ReadAllText(filePath);
            JSONNode    json            = JSON.Parse(contents);

            JSONArray   levelProgress   = json["LevelProgress"].AsArray;

            for (int i = 0; i < levelProgress.Count; i++)
            {
                Debug.Log(levelProgress[i]);
                Debug.Log(levelProgress[i]["Category"]);

                LevelCategory category      = (LevelCategory)levelProgress[i]["Category"].AsInt;
                JSONArray categoryProgress = levelProgress[i]["ProgressData"].AsArray;

                for (int j = 0; j < categoryProgress.Count; j++)
                {
                    int levelNumber             = categoryProgress[j]["LevelNum"].AsInt;
                    JSONArray catLevelProgress  = categoryProgress[j]["Progress"].AsArray;

                    for (int k = 0; k < catLevelProgress.Count; k++)
                    {
                        SaveData.LevelProgress[category][levelNumber][k] = catLevelProgress[k];
                    }
                }
            }
        }

        return false;
    }

    #endregion
}
