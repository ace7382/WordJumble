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
    public const string MAIN_MENU_PAGE__TITLE_CONTAINER_NAME        = "TitleContainer";
    public const string MAIN_MENU_PAGE__TITLE_NAME                  = "Title";
    public const string MAIN_MENU_PAGE__SUBTITLE_NAME               = "Subtitle";
    public const string MAIN_MENU_PAGE__DAILY_CONTAINER_NAME        = "DailyContainer";
    public const string MAIN_MENU_PAGE__MENU_CONTAINER_NAME         = "MenuContainer";
    public const string MAIN_MENU_PAGE__RED_BUTTON_NAME             = "RedButton";
    public const string MAIN_MENU_PAGE__BLUE_BUTTON_NAME            = "BlueButton";
    public const string MAIN_MENU_PAGE__GREEN_BUTTON_NAME           = "GreenButton";
    public const string MAIN_MENU_PAGE__YELLOW_BUTTON_NAME          = "YellowButton";
    public const string MAIN_MENU_PAGE__DAILY_LABEL_NAME            = "DailyLabel";
    public const string MAIN_MENU_PAGE__LOADING_LABEL_NAME          = "LoadingLabel";
    public const string MAIN_MENU_PAGE__ACHIEVEMENT_BUTTON_NAME     = "AchievementsButton";
    public const string MAIN_MENU_PAGE__SETTINGS_BUTTON_NAME        = "SettingsButton";

    public const string LEVEL_SELECT_PAGE__TITLE_NAME               = "Title";
    public const string LEVEL_SELECT_PAGE__ICON_CONTAINER_NAME      = "LevelIconContainer";
    public const string LEVEL_SELECT_PAGE__COMPLETE_COUNTER_NAME    = "CompleteCounter";
    public const string LEVEL_SELECT_PAGE__SECRET_COUNTER_NAME      = "SecretWordCounter";
    public const string LEVEL_SELECT_PAGE__BACK_BUTTON_NAME         = "BackButton";
    public const string LEVEL_SELECT_PAGE__HIDE_COMP_BUTTON_NAME    = "HideCompletedButton";
    public const string LEVEL_SELECT_PAGE__HIDE_SECRET_BUTTON_NAME  = "HideSecretButton";
    public const string LEVEL_SELECT_PAGE__GRID_BUTTON_NAME         = "GridButton";
    public const string LEVEL_SELECT_PAGE__LIST_BUTTON_NAME         = "ListButton";
    public const string LEVEL_SELECT_PAGE__ALL_COMPLETE_TEXT_NAME   = "AllCompleteMessage";

    public const string ACHIEVE_PAGE__CARD_CONTAINER_NAME           = "CardContainer";
    public const string ACHIEVE_PAGE__COMPLETE_COUNTER_NAME         = "CompleteCounter";
    public const string ACHIEVE_PAGE__BACK_BUTTON_NAME              = "BackButton";
    public const string ACHIEVE_PAGE__HIDE_COMP_BUTTON_NAME         = "HideCompletedButton";
    public const string ACHIEVE_PAGE__ALL_COMPLETE_TEXT_NAME        = "AllCompleteMessage";

    public const string ACHIEVEMENT_CARD__TITLE_NAME                = "Title";
    public const string ACHIEVEMENT_CARD__DESCRIPTION_NAME          = "Description";
    public const string ACHIEVEMENT_CARD__ICON_NAME                 = "Icon";
    public const string ACHIEVEMENT_CARD__BAR_FILL_NAME             = "Fill";
    public const string ACHIEVEMENT_CARD__BAR_LABEL_NAME            = "ProgressLabel";

    public const string LEVEL_SELECT_BADGE__THEME_NAME              = "ThemeLabel";
    public const string LEVEL_SELECT_BADGE__COMPLETE_ICON_NAME      = "CompletedIcon";
    public const string LEVEL_SELECT_BADGE__WORDBADGE_NAME          = "WordBadge";
    public const string LEVEL_SELECT_BADGE__WORD_NAME               = "Word";
    public const string LEVEL_SELECT_BADGE__UNFOUND_WORD_NAME       = "???";
    public const string LEVEL_SELECT_BADGE__SECRET_BADGE_NAME       = "SecretBadge";
    public const string LEVEL_SELECT_BADGE__DAILY_ROW_NAME          = "DailyPuzzleStatusRow";
    public const string LEVEL_SELECT_BADGE__DAILY_STATUS_LABEL_NAME = "DailyPuzzleStatusLabel";
    public const string LEVEL_SELECT_BADGE__ROW_2_NAME              = "Row2";

    public const string MINI_BADGE__NAME                            = "MiniBadge";
    public const string MINI_BADGE__COMPLETE_ICON_NAME              = "MB_CompleteIcon";
    public const string MINI_BADGE__SECRET_ICON_NAME                = "MB_SecretIcon";
    public const string MINI_BADGE__WORDBADGE_NAME                  = "MB_";
    public const string MINI_BADGE__LEVEL_NUM_NAME                  = "MB_LevelNum";

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

    public const string END_OF_LEVEL_PAGE__HEADER_NAME              = "LevelComplete";
    public const string END_OF_LEVEL_PAGE__HEADER_DOTS_NAME         = "HeaderDots";
    public const string END_OF_LEVEL_PAGE__SECRET_FOUND_NAME        = "SecretFound";
    public const string END_OF_LEVEL_PAGE__CENTER_PANEL_NAME        = "CenterPanel";
    public const string END_OF_LEVEL_PAGE__GUESSES_NAME             = "GuessesNumber";
    public const string END_OF_LEVEL_PAGE__WORDS_FOUND_NAME         = "WordsNumber";
    public const string END_OF_LEVEL_PAGE__NEW_WORDS_NAME           = "NewWordsNumber";
    public const string END_OF_LEVEL_PAGE__CATEGORY_NAME            = "Category";
    public const string END_OF_LEVEL_PAGE__CATEGORY_DETAILS_NAME    = "CategoryDetails";
    public const string END_OF_LEVEL_PAGE__COMPLETE_COUNT_NAME      = "CompleteCounter";
    public const string END_OF_LEVEL_PAGE__SECRET_COUNT_NAME        = "SecretWordCounter";
    public const string END_OF_LEVEL_PAGE__NEXT_BUTTON_NAME         = "NextLevel";
    public const string END_OF_LEVEL_PAGE__MAIN_MENU_BUTTON_NAME    = "MainMenu";
    public const string END_OF_LEVEL_PAGE__SECRET_FIND_BUTTON_NAME  = "SecretFind";
    public const string END_OF_LEVEL_PAGE__LEVEL_SELECT_BUTTON_NAME = "LevelSelect";
    public const string END_OF_LEVEL_PAGE__BREAK_NAME               = "Break";
    public const string END_OF_LEVEL_PAGE__BUTTON_CONTAINER_NAME    = "ButtonContainer";

    public const string LETTER_TILE__CONTAINER_NAME                 = "Tile";
    public const string LETTER_TILE__LETTER_LABEL_NAME              = "TileLetter";

    public const string SOLVED_WORD__WORD_NAME                      = "Word";
    public const string SOVLED_WORD__LENGTH_INDICATOR_NAME          = "LengthCounter";

    public const string GLOBAL_STYLE__TILE_SELECTED_CLASS           = "TileSelected";
    public const string GLOBAL_STYLE__TILE_SELECTED_LETTER_CLASS    = "TileSelectedLetter";
    public const string GLOBAL_STYLE__LOADING_ROW_CLASS             = "LoadingScreenRow";
    public const int GLOBAL_STYLE__SMALL_WORD_BADGE_WORDS_FONT_SIZE = 35;
    public const int GLOBAL_STYLE__SMALL_WORD_BADGE_ICONS_FONT_SIZE = 20;

    public const char WORD_LENGTH_INDICATOR_SYMBOL                  = '●';

    #endregion

    #region Inspector Variables

    [SerializeField] private VisualTreeAsset    letterTile;
    [SerializeField] private VisualTreeAsset    solvedWordTile;
    [SerializeField] private VisualTreeAsset    levelBadge;
    [SerializeField] private VisualTreeAsset    achievementCard;

    [SerializeField] private List<Color>        solvedWordColors;
    [SerializeField] private Color              secretWordColor;

    [SerializeField] private int                frameRateTarget;

    #endregion

    #region Public Properties

    public VisualTreeAsset  LetterTile          { get { return letterTile; } }
    public VisualTreeAsset  SolvedWordTile      { get { return solvedWordTile; } }
    public VisualTreeAsset  LevelBadge          { get { return levelBadge; } }
    public VisualTreeAsset  AchievementCard     { get { return achievementCard; } }
    public int              WordColorMax        { get { return solvedWordColors.Count; } }

    #endregion

    #region Inherited Functions

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        Application.targetFrameRate = frameRateTarget;
        Screen.orientation          = ScreenOrientation.Portrait;
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
