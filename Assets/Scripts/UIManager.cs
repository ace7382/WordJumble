using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    #region Singleton

    public static UIManager instance;

    #endregion

    #region Public Consts

    public const string MAIN_MENU_PAGE__ICON_NAME                   = "Icon";
    public const string MAIN_MENU_PAGE__DAILY_CONTAINER_NAME        = "DailyContainer";
    public const string MAIN_MENU_PAGE__BEGINNER_BUTTON_NAME        = "BeginnerButton";
    public const string MAIN_MENU_PAGE__ORIGINAL_BUTTON_NAME        = "OriginalButton";
    public const string MAIN_MENU_PAGE__5_WORDS_BUTTON_NAME         = "FiveWordButton";
    public const string MAIN_MENU_PAGE__6_WORDS_BUTTON_NAME         = "SixWordButton";
    public const string MAIN_MENU_PAGE__DAILY_LABEL_NAME            = "DailyLabel";

    public const string LEVEL_SELECT_PAGE__TITLE_NAME               = "Title";
    public const string LEVEL_SELECT_PAGE__ICON_CONTAINER_NAME      = "LevelIconContainer";
    public const string LEVEL_SELECT_PAGE__COMPLETE_COUNTER_NAME    = "CompleteCounter";
    public const string LEVEL_SELECT_PAGE__SECRET_COUNTER_NAME      = "SecretWordCounter";
    public const string LEVEL_SELECT_PAGE__BACK_BUTTON_NAME         = "BackButton";
    public const string LEVEL_SELECT_PAGE__HIDE_COMP_BUTTON_NAME    = "HideCompletedButton";

    public const string LEVEL_SELECT_BADGE__THEME_NAME              = "ThemeLabel";
    public const string LEVEL_SELECT_BADGE__COMPLETE_ICON_NAME      = "CompletedIcon";

    public const string GAME_PAGE__TILE_CONTAINER_NAME              = "TileContainer";
    public const string GAME_PAGE__SUBMITTED_WORD_NAME              = "SubmittedWord";
    public const string GAME_PAGE__SUBMIT_BUTTON_NAME               = "SubmitButton";
    public const string GAME_PAGE__SHUFFLE_BUTTON_NAME              = "ShuffleButton";
    public const string GAME_PAGE__CLEAR_BUTTON_NAME                = "ClearButton";
    public const string GAME_PAGE__FOUND_WORD_CONTAINER_NAME        = "FoundWordContainer";
    public const string GAME_PAGE__TIMER_NAME                       = "Timer";
    public const string GAME_PAGE__EXIT_BUTTON_NAME                 = "ExitButton";
    public const string GAME_PAGE__THEME_LABEL_NAME                 = "ThemeLabel";
    public const string GAME_PAGE__HIDE_FOUND_BUTTON_NAME           = "HideFoundButton";

    public const string GAME_OVERLAY_PAGE__MESSAGE_NAME             = "Message";
    public const string GAME_OVERLAY_PAGE__BUTTON_1_NAME            = "Button1";
    public const string GAME_OVERLAY_PAGE__BUTTON_2_NAME            = "Button2";
    public const string GAME_OVERLAY_PAGE__BUTTON_3_NAME            = "Button3";
    public const string GAME_OVERLAY_PAGE__BUTTON_4_NAME            = "Button4";

    public const string LETTER_TILE__CONTAINER_NAME                 = "Tile";
    public const string LETTER_TILE__LETTER_LABEL_NAME              = "TileLetter";

    public const string SOLVED_WORD__WORD_NAME                      = "Word";
    public const string SOVLED_WORD__LENGTH_INDICATOR_NAME          = "LengthCounter";

    public const string GLOBAL_STYLE__TILE_SELECTED_CLASS           = "TileSelected";
    public const string GLOBAL_STYLE__TILE_SELECTED_LETTER_CLASS    = "TileSelectedLetter";
    public const int GLOBAL_STYLE__SMALL_WORD_BADGE_WORDS_FONT_SIZE = 35;
    public const int GLOBAL_STYLE__SMALL_WORD_BADGE_ICONS_FONT_SIZE = 20;

    public const char WORD_LENGTH_INDICATOR_SYMBOL                  = '●';

    #endregion

    #region Inspector Variables

    [SerializeField] private VisualTreeAsset    letterTile;
    [SerializeField] private VisualTreeAsset    solvedWordTile;
    [SerializeField] private VisualTreeAsset    levelTile;

    [SerializeField] private List<Color>        solvedWordColors;
    [SerializeField] private Color              secretWordColor;

    #endregion

    #region Public Properties

    public VisualTreeAsset LetterTile       { get { return letterTile; } }
    public VisualTreeAsset SolvedWordTile   { get { return solvedWordTile; } }
    public VisualTreeAsset LevelTile        { get { return levelTile; } }

    #endregion

    #region Inherited Functions

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    #endregion

    #region Public Functions

    public Color GetColor(int i)
    {
        if (i == -1)
            return secretWordColor;

        return solvedWordColors[i];
    }

    #endregion
}
