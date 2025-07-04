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

    private const string SAVE_DIRECTORY                     = "/SaveData/";
    private const string FILE_NAME                          = "abc.sav";

    #endregion

    #region Inspector Variables

    [SerializeField] private TextAsset textAsset_WordList;

    #endregion

    #region Private Variables

    private float                       time;
    private Button                      timerButton;
    private bool                        paused              = false;
    private LevelCategory               currentCategory;

    private SaveFile                    saveData;

    private Dictionary<string, byte>    wordList;

    private System.Random               rand                = new System.Random();
    private Stack<string>               preRandWords        = new Stack<string>();

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
        LoadSaveData();
        SetAllWords();

        PreloadRandomWords(1000);
    }

    public void Update()
    {
        if (preRandWords.Count < 500)
            PreloadRandomWords(2);

        if (timerButton == null || paused)
            return;

        time                += Time.deltaTime;
        timerButton.text    = Utilities.GetTimerStringFromFloat(time);

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

        foreach (KeyValuePair<LevelCategory, Dictionary<int, bool>> pair in SaveData.SecretWordProgress)
        {
            Dictionary<string, object> catData  = new Dictionary<string, object>();

            catData["Category"]                 = (int)pair.Key;

            List<object> progress               = new List<object>();

            foreach (KeyValuePair<int, bool> progPair in pair.Value)
            {
                Dictionary<string, object> progData = new Dictionary<string, object>();

                progData["LevelNum"]            = progPair.Key;
                progData["Progress"]            = progPair.Value;

                progress.Add(progData);
            }

            catData["ProgressData"]             = progress;

            secretProgress.Add(catData);
        }

        List<object> foundWordsData = new List<object>();

        foreach (KeyValuePair<string, int> pair in SaveData.FoundWords)
        {
            Dictionary<string, object> wordData = new Dictionary<string, object>();

            wordData["Word"]                    = pair.Key;
            wordData["Num"]                     = pair.Value;

            foundWordsData.Add(wordData);
        }

        finalData["FoundWords"]                 = foundWordsData;

        finalData["CompletedDailyPuzzles"]      = SaveData.CompletedDailyPuzzles;
        finalData["TodaysDailyProgress"]        = SaveData.TodaysDailyProgress;
        finalData["DailyPuzzleTimeInSeconds"]   = SaveData.DailyPuzzleTimeInSeconds;
        finalData["LevelProgress"]              = levelProgress;
        finalData["SecretWordProgress"]         = secretProgress;
        finalData["DailyPuzzleDate"]            = Utilities.GetDateAsString(SaveData.DailyPuzzleDate);
        finalData["ACH"]                        = SaveData.AchievementsUnlocked;

        string dir = Application.persistentDataPath + SAVE_DIRECTORY;

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        string fileContents = Utilities.ConvertToJsonString(finalData);

        File.WriteAllText(dir + FILE_NAME, fileContents);
    }

    public string GetWordFromList()
    {
        if (preRandWords.Count > 0)
            return preRandWords.Pop();

        PreloadRandomWords(50);

        return preRandWords.Pop();
    }

    public bool IsWordInList(string word)
    {
        return wordList.ContainsKeyIgnoreCase(word);
    }

    #endregion

    #region Private Functions

    private void LoadSaveData()
    {
        saveData        = new SaveFile();

        string filePath = Application.persistentDataPath + SAVE_DIRECTORY + FILE_NAME;

        if (File.Exists(filePath))
        {
            string      contents                = File.ReadAllText(filePath);
            JSONNode    json                    = JSON.Parse(contents);

            JSONArray   levelProgress           = json["LevelProgress"].AsArray;

            for (int i = 0; i < levelProgress.Count; i++)
            {
                LevelCategory category          = (LevelCategory)levelProgress[i]["Category"].AsInt;
                JSONArray categoryProgress      = levelProgress[i]["ProgressData"].AsArray;

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

            JSONArray   secretProgress  = json["SecretWordProgress"].AsArray;

            for (int i = 0; i < secretProgress.Count; i++)
            {
                LevelCategory category          = (LevelCategory)secretProgress[i]["Category"].AsInt;
                JSONArray categoryProgress      = secretProgress[i]["ProgressData"].AsArray;

                for (int j = 0; j < categoryProgress.Count; j++)
                {
                    int levelNum                = categoryProgress[j]["LevelNum"].AsInt;
                    bool catLevelProgress       = categoryProgress[j]["Progress"].AsBool;

                    SaveData.SecretWordProgress[category][levelNum] = catLevelProgress;
                }
            }

            JSONArray   dailyProgress           = json["TodaysDailyProgress"].AsArray;
            List<bool>  finalDP                 = new List<bool>();

            for (int i = 0; i < dailyProgress.Count; i++)
            {
                finalDP.Add(dailyProgress[i]);
            }

            SaveData.TodaysDailyProgress        = finalDP;

            JSONArray   completeDailies         = json["CompletedDailyPuzzles"].AsArray;

            SaveData.CompletedDailyPuzzles      = new List<string>();

            for (int i = 0; i < completeDailies.Count; i++)
            {
                SaveData.CompletedDailyPuzzles.Add(completeDailies[i].Value);
            }

            SaveData.DailyPuzzleDate            = DateTime.Parse(json["DailyPuzzleDate"].Value);
            SaveData.DailyPuzzleTimeInSeconds   = json["DailyPuzzleTimeInSeconds"].AsFloat;

            JSONArray foundWords                = json["FoundWords"].AsArray;
            SaveData.FoundWords                 = new Dictionary<string, int>();

            for (int i = 0; i < foundWords.Count; i++)
            {
                string word         = foundWords[i]["Word"].Value;
                int num             = foundWords[i]["Num"].AsInt;

                SaveData.FoundWords
                    [word]          = num;
            }

            /////Achievements
            JSONArray achUnlocks                = json["ACH"].AsArray;
            SaveData.AchievementsUnlocked       = new List<int>();

            for (int i = 0; i < achUnlocks.Count; i++)
            {
                SaveData.AchievementsUnlocked.Add(achUnlocks[i].AsInt);
            }

            //TODO: Check platform achieevements and compare/set that way too
            ////////
        }
    }

    private void SetAllWords()
    {
        string[] allWords   = textAsset_WordList.text.Split('\n');
        wordList            = new Dictionary<string, byte>();

        for (int i = 0; i < allWords.Length; i++)
        {
            string word     = allWords[i].TrimEnd('\r', '\n');

            if (!string.IsNullOrEmpty(word) && word.Length >= 3 && word.Length < 12)
            {
                wordList.Add(word, new byte());
            }
        }

        Debug.Log(wordList.Count.ToString() + " Words Loaded Into Word List");
    }

    private void PreloadRandomWords(int num)
    {
        for (int i = 0; i < num; i++)
        {
            preRandWords.Push(wordList.ElementAt(rand.Next(0, wordList.Count)).Key);
        }
    }

    #endregion
}
