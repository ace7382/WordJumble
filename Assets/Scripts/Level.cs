using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LevelCategory
{
    [EnumName("Beginner")]      BEGINNER    ,
    [EnumName("Original")]      ORIGINAL    ,
    [EnumName("Five Words")]    FIVE_WORD   ,
    [EnumName("Six Words")]     SIX_WORD    ,
    [EnumName("Daily")]         DAILY       ,
    [EnumName("null")]          NULL
};

[CreateAssetMenu(fileName = "Level", menuName = "New Level")]
public class Level : ScriptableObject
{
    #region Inspector Variables

    [SerializeField] private int            levelNumber;
    [SerializeField] private string         theme;
    [SerializeField] private List<string>   words;
    [SerializeField] private List<bool>     foundWords;
    [SerializeField] private string         secretWord;
    [SerializeField] private bool           secretWordFound;
    [SerializeField] private bool           complete;

    #endregion

    #region Public Properties

    public int              LevelNumber     { get { return levelNumber; } }
    public string           Theme           { get { return theme; } }
    public List<string>     Words           { get { return words; } }
    public string           SecretWord      { get { return secretWord; } }
    public bool             SecretWordFound { get { return secretWordFound; } set { secretWordFound = value; } }
    public List<bool>       FoundWords      { get { return foundWords; } }
    public bool             Complete        { get { return complete; } set { complete = value; } }

    #endregion

    #region Public Functions

    public void WordFound(int index)
    {
        FoundWords[index] = true;
    }

    #endregion
}
