using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelBadge
{
    #region Private Const

    private const int       MAX_BADGE_NUM   = 7;

    #endregion

    #region Private Variables

    private NewLevel        level;

    private VisualElement   root;

    private QuickButton     button;

    private VisualElement   miniBadge;
    private QuickButton     miniButton;

    private bool            fullBadge;

    #endregion

    #region Private Properties

    private bool            FullBadge
    {
        get { return fullBadge; }
        set
        {
            fullBadge = value;
            root.ElementAt(0).Show(fullBadge);
            miniBadge.Show(!fullBadge);
        }
    }

    #endregion

    #region Constructor

    public LevelBadge(VisualElement root, NewLevel level, bool fullBadge = true)
    {
        this.root       = root;
        miniBadge       = root.Q<VisualElement>(UIManager.MINI_BADGE__NAME);

        this.level      = level;

        FullBadge       = fullBadge;

        Setup();
    }

    #endregion

    #region Private Functions

    private void Setup()
    {
        miniBadge.Q<Label>(UIManager.MINI_BADGE__LEVEL_NUM_NAME)
            .text                       = level.LevelNumber.ToString();

        root.Q<Label>(UIManager.LEVEL_SELECT_BADGE__THEME_NAME)
            .text                       = level.Theme;

        for (int i = 1; i <= MAX_BADGE_NUM; i++)
        {
            VisualElement currentBadge  = root.Q<VisualElement>(UIManager.LEVEL_SELECT_BADGE__WORDBADGE_NAME + i.ToString());
            VisualElement currentMini   = miniBadge.Q<VisualElement>(UIManager.MINI_BADGE__WORDBADGE_NAME + i.ToString());

            if (level.Words.Count < i)
            {
                currentBadge.SetVisibility(false);
                currentMini.SetVisibility(false);
                continue;
            }

            bool isWordFound            = GameManager.instance.SaveData.IsWordFound(level, i - 1);
            currentBadge.style
                .backgroundColor        =  isWordFound ?
                                            UIManager.instance.GetColor(i - 1) :
                                            new Color(
                                                UIManager.instance.GetColor(i - 1).r
                                                , UIManager.instance.GetColor(i - 1).g
                                                , UIManager.instance.GetColor(i - 1).b
                                                , .4f
                                            );
            currentMini.style
                .backgroundColor        = currentBadge.style.backgroundColor;

            currentBadge.Q<Label>(UIManager.LEVEL_SELECT_BADGE__WORD_NAME)
                .text                   = isWordFound ?
                                            level.Words[i - 1].ToUpper() :
                                            UIManager.LEVEL_SELECT_BADGE__UNFOUND_WORD_NAME;
        }

        VisualElement secretBadge       = root.Q<VisualElement>(UIManager.LEVEL_SELECT_BADGE__SECRET_BADGE_NAME);
        VisualElement miniSecret        = miniBadge.Q<VisualElement>(UIManager.MINI_BADGE__SECRET_ICON_NAME);

        if (level.Category == LevelCategory.DAILY)
        {
            secretBadge.SetVisibility(false);
            miniBadge.SetVisibility(false);
        }
        else
        {
            bool isSecretFound          = GameManager.instance.SaveData.IsSecretWordFound(level);

            secretBadge.style
                .backgroundColor        =  isSecretFound?
                                            UIManager.instance.GetColor(-1) :
                                            new Color(
                                                UIManager.instance.GetColor(-1).r
                                                , UIManager.instance.GetColor(-1).g
                                                , UIManager.instance.GetColor(-1).b
                                                , .4f
                                            );

            miniSecret.SetVisibility(isSecretFound);

            secretBadge.Q<Label>(UIManager.LEVEL_SELECT_BADGE__WORD_NAME)
                .text                   = isSecretFound ?
                                            level.SecretWord.ToUpper()
                                            : UIManager.LEVEL_SELECT_BADGE__UNFOUND_WORD_NAME;

            VisualElement miniComplete  = miniBadge.Q<VisualElement>(UIManager.MINI_BADGE__COMPLETE_ICON_NAME);

            bool isLevelComplete        = GameManager.instance.SaveData.IsLevelComplete(level);
            miniComplete.SetVisibility(isLevelComplete);

            if (GameManager.instance.SaveData.IsLevelFullyComplete(level))
            {
                miniBadge.SetBorderColor(Color.black);
                miniBadge.SetBorderWidth(8f);

                root.ElementAt(0).SetBorderColor(Color.black);
                root.ElementAt(0).SetBorderWidth(8f);
            }
        }

        if (level.Words.Count < 5 && string.IsNullOrEmpty(level.SecretWord))
            root.Q<VisualElement>(UIManager.LEVEL_SELECT_BADGE__ROW_2_NAME).Hide();

        if (level.Category != LevelCategory.DAILY)
        {
            root.Q<VisualElement>(UIManager.LEVEL_SELECT_BADGE__DAILY_ROW_NAME).Hide();
        }
        else
        {
            Label dailyStatusLabel      = root.Q<Label>(UIManager.LEVEL_SELECT_BADGE__DAILY_STATUS_LABEL_NAME);
            string timeString           = Utilities.GetTimerStringFromFloat(GameManager.instance.SaveData.DailyPuzzleTimeInSeconds);

            if (GameManager.instance.SaveData.IsLevelComplete(level))
            {
                dailyStatusLabel.text   = "Complete! - " + timeString;
            }
            else if (GameManager.instance.SaveData.DailyPuzzleTimeInSeconds > 0f)
            {
                dailyStatusLabel.text   = "Paused - " + timeString;
            }
            else
            {
                root.Q<VisualElement>(UIManager.LEVEL_SELECT_BADGE__DAILY_ROW_NAME).Hide();
            }
        }
    }

    #endregion

    #region Public Functions

    public void SwapDisplayMode(bool showFullBadge)
    {
        FullBadge = showFullBadge;
    }

    public void RegisterOnClick(System.Action onClick)
    {
        button = new QuickButton(root.ElementAt(0), Color.white);
        root.AddManipulator(button);

        miniButton = new QuickButton(miniBadge, Color.white);
        miniBadge.AddManipulator(miniButton);

        root.RegisterCallback<ClickEvent>((_) => onClick.Invoke());
        miniBadge.RegisterCallback<ClickEvent>((_) => onClick.Invoke());
    }

    #endregion
}
