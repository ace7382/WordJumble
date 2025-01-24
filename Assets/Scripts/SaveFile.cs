using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[System.Serializable]
public class SaveFile
{
    public Dictionary<LevelCategory, Dictionary<int, List<bool>>>   LevelProgress;
    public Dictionary<LevelCategory, Dictionary<int, bool>>         SecretWordProgress;
    public List<bool>                                               TodaysDailyProgress;
    public List<string>                                             CompletedDailyPuzzles;
    public float                                                    DailyPuzzleTimeInSeconds    = 0f;
    public DateTime                                                 DailyPuzzleDate;

    public Dictionary<string, int>                                  FoundWords;

    public SaveFile()
    {
        LevelProgress           = new Dictionary<LevelCategory, Dictionary<int, List<bool>>>();
        SecretWordProgress      = new Dictionary<LevelCategory, Dictionary<int, bool>>();
        TodaysDailyProgress     = Enumerable.Repeat(false, 10).ToList();
        CompletedDailyPuzzles   = new List<string>();
        DailyPuzzleDate         = new DateTime(1900, 01, 01);
        FoundWords              = new Dictionary<string, int>();

        Dictionary<int, List<bool>> final = new Dictionary<int, List<bool>>();
        Dictionary<int, bool> secretFinal = new Dictionary<int, bool>();

        foreach (NewLevel lev in LevelDefinitions.ALL_LEVELS)
        {
            if (lev.Category == LevelCategory.NULL || lev.Category == LevelCategory.DAILY)
                continue; //TODO: Probably remove this once i have levels defined correctly

            if (!LevelProgress.ContainsKey(lev.Category))
                LevelProgress.Add(lev.Category, new Dictionary<int, List<bool>>());

            List<bool> blankFoundList = Enumerable.Repeat(false, lev.Words.Count).ToList();
            LevelProgress[lev.Category].Add(lev.LevelNumber, blankFoundList);

            if (!SecretWordProgress.ContainsKey(lev.Category))
                SecretWordProgress.Add(lev.Category, new Dictionary<int, bool>());

            SecretWordProgress[lev.Category].Add(lev.LevelNumber, false);
        }
    }

    public bool IsLevelComplete(NewLevel level)
    {
        if (level.Category == LevelCategory.DAILY)
        {
            string dateAsString = Utilities.GetDateAsString(level.Date);

            return CompletedDailyPuzzles.FindIndex(x => string.Equals(x, dateAsString)) > -1;
        }
        
        return LevelProgress[level.Category][level.LevelNumber].FindIndex(x => x == false) == -1;
    }

    public bool IsLevelFullyComplete(NewLevel level)
    {
        return IsLevelComplete(level) && IsSecretWordFound(level);
    }

    public bool IsWordFound(NewLevel level, int wordIndex)
    {
        if (level.Category == LevelCategory.DAILY)
            return TodaysDailyProgress[wordIndex];

        return LevelProgress[level.Category][level.LevelNumber][wordIndex];
    }

    public bool IsSecretWordFound(NewLevel level)
    {
        return SecretWordProgress[level.Category][level.LevelNumber];
    }

    public void MarkWordFound(NewLevel level, int wordIndex)
    {
        if (level.Category == LevelCategory.DAILY)
        {
            TodaysDailyProgress[wordIndex] = true;

            if (!TodaysDailyProgress.Contains(false))
                CompletedDailyPuzzles.Add(Utilities.GetDateAsString(level.Date));
        }
        else
            LevelProgress[level.Category][level.LevelNumber][wordIndex] = true;
    }

    public void MarkSecretWordFound(NewLevel level)
    {
        SecretWordProgress[level.Category][level.LevelNumber] = true;
    }

    public void AddFoundWord(string word)
    {
        word = word.ToUpper();

        if (FoundWords.ContainsKey(word))
            FoundWords[word]++;
        else
        {
            FoundWords.Add(word, 1);

            this.PostNotification(Notifications.WORD_ADDED_TO_FOUND_WORDS, word);
        };
    }

    public void NewDay_ResetTimeAndFoundList()
    {
        TodaysDailyProgress         = Enumerable.Repeat(false, PlayFabManager.instance.DailyLevel.Words.Count).ToList();
        DailyPuzzleTimeInSeconds    = 0f;
    }

    public int LevelCompleteCount(LevelCategory category)
    {
        int ret = 0;

        foreach (KeyValuePair<int, List<bool>> levelProg in LevelProgress[category])
        {
            if (levelProg.Value.Contains(false))
                continue;

            ret++;
        }

        return ret;
    }

    public int SecretFoundCount(LevelCategory category)
    {
        int ret = 0;

        foreach(KeyValuePair<int, bool> prog in SecretWordProgress[category])
        {
            if (prog.Value == true)
                ret++;
        }

        return ret;
    }

    public bool IsAchievementUnlocked(Achievement achievement)
    { 
        return false;
    }

    public bool IsWordInFoundWordsList(string word)
    {
        return FoundWords.ContainsKeyIgnoreCase(word);
    }
}
