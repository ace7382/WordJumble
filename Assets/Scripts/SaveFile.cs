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

    public SaveFile()
    {
        LevelProgress           = new Dictionary<LevelCategory, Dictionary<int, List<bool>>>();
        SecretWordProgress      = new Dictionary<LevelCategory, Dictionary<int, bool>>();
        TodaysDailyProgress     = Enumerable.Repeat(false, 10).ToList();
        CompletedDailyPuzzles   = new List<string>();
        DailyPuzzleDate         = new DateTime(1900, 01, 01);

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

    public bool IsLevelComplete(LevelCategory cat, int levNum)
    {
        NewLevel def = LevelDefinitions.ALL_LEVELS.Find(x => x.Category == cat && x.LevelNumber == levNum);

        return def.Words.Count == LevelProgress[cat][levNum].FindAll(x => x == true).Count();
    }

    public bool IsLevelComplete(NewLevel lev)
    {
        return IsLevelComplete(lev.Category, lev.LevelNumber);
    }

    public bool IsLevelComplete_Daily(NewLevel level)
    {
        return IsLevelComplete_Daily(level.Date);
    }

    public bool IsLevelComplete_Daily(DateTime date)
    {
        string dateAsString = Utilities.GetDateAsString(date);

        return CompletedDailyPuzzles.FindIndex(x => string.Equals(x, dateAsString)) > -1;
        //return CompletedDailyPuzzles.FindIndex(x => x.Date == date.Date) > -1;
    }

    public bool IsLevelComplete_Daily()
    {
        return IsLevelComplete_Daily(PlayFabManager.instance.ServerDate);
    }

    public bool IsLevelFullyComplete(NewLevel level)
    {
        if (level.Category == LevelCategory.DAILY)
        {
            return IsLevelComplete_Daily(level);
        }

        return IsLevelComplete(level) && IsSecretWordFound(level);
    }

    public bool IsWordFound(LevelCategory cat, int levNum, int wordIndex)
    {
        if (cat == LevelCategory.DAILY)
            return IsWordFound_Daily(wordIndex);

        return LevelProgress[cat][levNum][wordIndex];
    }

    public bool IsWordFound_Daily(int index)
    {
        return TodaysDailyProgress[index];
    }

    public bool IsSecretWordFound(LevelCategory cat, int levNum)
    {
        return SecretWordProgress[cat][levNum];
    }

    public bool IsSecretWordFound(NewLevel lev)
    {
        return IsSecretWordFound(lev.Category, lev.LevelNumber);
    }

    public void MarkWordFound(NewLevel lev, int index)
    {
        if (lev.Category == LevelCategory.DAILY)
            MarkWordFound_Daily(index);
        else
            LevelProgress[lev.Category][lev.LevelNumber][index] = true;
    }

    public void MarkWordFound_Daily(int index)
    {
        TodaysDailyProgress[index] = true;

        if (TodaysDailyProgress.Contains(false))
            return;

        CompletedDailyPuzzles.Add(Utilities.GetDateAsString(PlayFabManager.instance.ServerDate));
        //CompletedDailyPuzzles.Add(PlayFabManager.instance.ServerDate);
    }

    public void MarkSecretWordFound(NewLevel lev)
    {
        SecretWordProgress[lev.Category][lev.LevelNumber] = true;
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
}
