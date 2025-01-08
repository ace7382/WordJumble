using System.Collections;
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
        if (!LoadSaveData())
            saveData = new SaveFile();
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
    }

    #endregion

    #region Public Functions

    public void SetTime(Button ve, bool resetTime = false)
    {
        Debug.Log("Setting Time: " + resetTime);

        if (resetTime)
            time    = 0f;

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

    public NewLevel GetNextLevel(NewLevel currentLevel)
    {
        int nextIndex = LevelDefinitions.ALL_LEVELS.FindIndex(x => x.LevelNumber == currentLevel.LevelNumber + 1 && x.Category == currentLevel.Category);

        if (nextIndex == -1)    return null;
        else return             LevelDefinitions.ALL_LEVELS[nextIndex];
    }

    public void SaveGame()
    {
        string dir = Application.persistentDataPath + SAVE_DIRECTORY;

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        string json = JsonUtility.ToJson(saveData, true);

        File.WriteAllText(dir + FILE_NAME, json);
    }

    #endregion

    #region Private Functions

    private bool LoadSaveData()
    {
        string filePath     = Application.persistentDataPath + SAVE_DIRECTORY + FILE_NAME;

        if (File.Exists(filePath))
        {
            string json     = File.ReadAllText(filePath);
            saveData        = JsonUtility.FromJson<SaveFile>(json);

            return true;
        }

        return false;
    }

    #endregion
}
