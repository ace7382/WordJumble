using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LevelCategory
{
    [EnumName("Beginner")]      BEGINNER    = 0 ,
    [EnumName("Original")]      ORIGINAL    = 1 ,
    [EnumName("Five Words")]    FIVE_WORD   = 2 ,
    [EnumName("Six Words")]     SIX_WORD    = 3 ,
    [EnumName("Daily")]         DAILY       = 4 ,
    [EnumName("null")]          NULL        = 5
};

[System.Serializable]
public class NewLevel
{
    public LevelCategory    Category;
    public string           Theme;
    public int              LevelNumber;
    public List<string>     Words;
    public string           SecretWord;

    public bool             IsNull          { get { return Category == LevelCategory.NULL; } }

    public NewLevel()
    {
        Category            = LevelCategory.NULL;
        Theme               = string.Empty;
        LevelNumber         = -1;
        Words               = new List<string>();
        SecretWord          = string.Empty;
    }

    public NewLevel(LevelCategory category, string theme, int levelNumber, List<string> words, string secretWord)
    {
        Category            = category;
        Theme               = theme;
        LevelNumber         = levelNumber;
        Words               = words;
        SecretWord          = secretWord;
    }
}

public static class LevelDefinitions
{
    public static List<NewLevel> ALL_LEVELS = new List<NewLevel>()
    {
        new NewLevel(LevelCategory.BEGINNER, "Test", 0, new List<string>() { "aa", "bbb", "cccc", "dddd" }, "Bad"),
        new NewLevel(LevelCategory.BEGINNER, "Test2", 1, new List<string>() { "xx", "yyy", "zzzz", "adam" }, "Yam"),
        new NewLevel(LevelCategory.ORIGINAL, "Ocean", 1, new List<string>() { "Crab", "Beach", "Dolphin", "Seahorse" }, "Sea"),
        new NewLevel(LevelCategory.ORIGINAL, "Forest", 2, new List<string>() { "Bear", "Trail", "Hiking", "Campfire" }, "Camping"),
        new NewLevel(LevelCategory.ORIGINAL, "Desert", 3, new List<string>() { "Sand", "Snake", "Cactus", "Drought" }, "Sun"),
        new NewLevel(LevelCategory.ORIGINAL, "History", 97, new List<string>() { "Past", "Relic", "Ancient", "Artifact" }, "Art"),
    };
}