using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Achievement
{
    #region Protected Variables

    protected string    name;
    protected int       id;

    #endregion

    #region Public Properties

    public string       Name        { get { return name; } }
    public int          ID          { get { return id; } }
    public bool         IsUnlocked  { get { return GameManager.instance.SaveData.IsAchievementUnlocked(this); } }

    #endregion

    #region Abstract Funcitons

    public abstract bool    CheckUnlock();
    public abstract string  GetDescription();
    public abstract string  GetProgressString();
    public abstract float   GetProgressPercent();

    #endregion

    #region Constructor

    public Achievement(string name, int id)
    {
        this.name           = name;
        this.id             = id;
    }

    #endregion
}

public class ACH_DailyCount : Achievement
{
    #region Private Variables

    private int numOfDays;

    #endregion

    #region Constructor

    public ACH_DailyCount(string name, int id, int numOfDays) : base(name, id)
    {
        this.numOfDays = numOfDays;
    }

    #endregion

    #region Inherited Functions

    public override bool CheckUnlock()
    {
        return GameManager.instance.SaveData.CompletedDailyPuzzles.Count >= numOfDays;
    }

    public override string GetDescription()
    {
        return $"Complete the Daily Jumblie {numOfDays} time{(numOfDays == 1 ? "" : "s (Jumblies do not need to be on consecutive days)")}";
    }

    public override string GetProgressString()
    {
        return $"{Mathf.Clamp(GameManager.instance.SaveData.CompletedDailyPuzzles.Count, 0, numOfDays)} / {numOfDays}";
    }

    public override float GetProgressPercent()
    {
        return (float)GameManager.instance.SaveData.CompletedDailyPuzzles.Count / (float)numOfDays * 100f;
    }

    #endregion
}

public class ACH_SpecificWord : Achievement
{
    #region Private Variables

    private string wordToFind;

    #endregion

    #region Private Properties

    private bool IsFound
    {
        get { return GameManager.instance.SaveData.IsWordInFoundWordsList(wordToFind); }
    }

    #endregion

    #region Constructor

    public ACH_SpecificWord(string name, int id, string wordToFind) : base(name, id)
    {
        this.wordToFind = wordToFind;
    }

    #endregion

    #region Inherited Functions

    public override bool CheckUnlock()
    {
        return IsFound;
    }

    public override string GetDescription()
    {
        return $"Find the word \"{wordToFind}\"";
    }

    public override string GetProgressString()
    {
        return $"{(IsFound ? "1" : "0")} / 1";
    }

    public override float GetProgressPercent()
    {
        return IsFound ? 100f : 0f;
    }

    #endregion
}

public class ACH_WordGroup : Achievement
{
    #region Private Variables

    private List<string>    words;
    private int             numToFind;

    #endregion

    #region Private Properties

    private int             WordsFound
    {
        get
        {
            int countFound = 0;

            for (int i = 0; i < words.Count; i++)
                if (GameManager.instance.SaveData.IsWordInFoundWordsList(words[i]))
                    countFound++;

            return countFound;
        }
    }

    #endregion

    #region Constructor

    public ACH_WordGroup(string name, int id, List<string> words, int numToFind) : base(name, id)
    {
        this.words      = words;
        this.numToFind  = numToFind;
    }

    #endregion

    #region Inherited Functions

    public override bool CheckUnlock()
    {
        int countFound = 0;

        for (int i = 0; i < words.Count; i++)
        {
            if (GameManager.instance.SaveData.IsWordInFoundWordsList(words[i]))
            {
                countFound++;

                if (countFound >= numToFind)
                    return true;
            }
        }

        return false;
    }

    public override string GetDescription()
    {
        return $"Find {(words.Count == numToFind ? "all" : numToFind.ToString())} of the following words: {String.Join(", ", words)}";
    }

    public override string GetProgressString()
    {
        return $"{Mathf.Clamp(WordsFound, 0, numToFind)} / {numToFind}";
    }

    public override float GetProgressPercent()
    {
        return Mathf.Clamp((float)WordsFound / (float)numToFind * 100f, 0f, 100f);
    }

    #endregion
}

public class ACH_CategoryComplete : Achievement
{
    #region Private Variables

    private LevelCategory category;

    #endregion

    #region Private Properties

    private int NumberOfLevelsInCategory
    {
        get
        {
            return LevelDefinitions.GetLevelCountByCategory(category);
        }
    }

    private int NumberOfLevelsCompletedInCategory
    {
        get
        {
            return GameManager.instance.SaveData.GetLevelCompleteCount(category);
        }
    }

    #endregion

    #region Public Properties

    public LevelCategory Category { get { return category; } }

    #endregion

    #region Constructor

    public ACH_CategoryComplete(string name, int id, LevelCategory category) : base(name, id)
    {
        this.category = category;
    }

    #endregion

    #region Inherited Functions

    public override bool CheckUnlock()
    {
        return NumberOfLevelsCompletedInCategory >= NumberOfLevelsInCategory;
    }

    public override string GetDescription()
    {
        return $"Complete all levels in the \"{category.Name()}\" category";
    }

    public override string GetProgressString()
    {
        return $"{Mathf.Clamp(NumberOfLevelsCompletedInCategory, 0, NumberOfLevelsInCategory)} / {NumberOfLevelsInCategory}";
    }

    public override float GetProgressPercent()
    {
        return Mathf.Clamp((float)NumberOfLevelsCompletedInCategory
                    / (float)NumberOfLevelsInCategory * 100f, 0f, 100f);
    }

    #endregion
}

public static class AchievementDefinitions
{
    public static List<Achievement> ALL_ACHIEVEMENTS
    {
        get
        {
            List<Achievement> ret = new List<Achievement>();

            ret.AddRange(DAILY_COUNT_ACHIEVEMENTS);
            ret.AddRange(SPECIFIC_WORD_ACHIEVEMENTS);
            ret.AddRange(WORD_GROUP_ACHIEVEMENTS);
            ret.AddRange(CATEGORY_COMPLETE_ACHIEVEMENTS);

            return ret;
        }
    }

    public static int ACHIEVEMENT_COUNT
    {
        get
        {
            return ALL_ACHIEVEMENTS.Count;
        }
    }

    public static List<ACH_DailyCount> DAILY_COUNT_ACHIEVEMENTS = new List<ACH_DailyCount>()
    {
        new ACH_DailyCount("Baby's First Daily Jumblie", 0, 1),
        new ACH_DailyCount("5 Daily", 1, 5),
        new ACH_DailyCount("30 Daily", 2, 30),
        new ACH_DailyCount("60 Daily", 3, 60),
        new ACH_DailyCount("100 Daily", 4, 100),
    };

    public static List<ACH_SpecificWord> SPECIFIC_WORD_ACHIEVEMENTS = new List<ACH_SpecificWord>()
    {
        new ACH_SpecificWord("The Best Dog", 100, "Bucket"),
        new ACH_SpecificWord("A Man, a Plan, and a Canal Walk into a Bar", 101, "Palindrome")
    };

    public static List<ACH_WordGroup> WORD_GROUP_ACHIEVEMENTS = new List<ACH_WordGroup>()
    {
        new ACH_WordGroup("Best Dogs", 200, new List<string>() {"Bucket", "Briar", "Corsair", "Anvil"}, 4),
        new ACH_WordGroup("GG", 201, new List<string>() {"Game", "Controller", "Console", "Chat", "Party", "Video"}, 3),
    };

    public static List<ACH_CategoryComplete> CATEGORY_COMPLETE_ACHIEVEMENTS = new List<ACH_CategoryComplete>()
    {
        new ACH_CategoryComplete("Baby Steps", 300, LevelCategory.BEGINNER),
        new ACH_CategoryComplete("The OG", 301, LevelCategory.ORIGINAL),
        new ACH_CategoryComplete("Jumblie Expert", 302, LevelCategory.ADVANCED)
    };
}