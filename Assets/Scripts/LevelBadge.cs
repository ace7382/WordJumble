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

    #endregion

    #region Constructor

    public LevelBadge(VisualElement root, NewLevel level)
    {
        this.root       = root;
        this.level      = level;

        Setup();
    }

    #endregion

    #region Private Functions

    private void Setup()
    {
        root.Q<Label>(UIManager.LEVEL_SELECT_BADGE__THEME_NAME).text    = level.Theme;

        for (int i = 1; i <= MAX_BADGE_NUM; i++)
        {
            VisualElement currentBadge = root.Q<VisualElement>(UIManager.LEVEL_SELECT_BADGE__WORDBADGE_NAME + i.ToString());

            if (level.Words.Count < i)
            {
                currentBadge.SetVisibility(false);
                continue;
            }

            currentBadge.style
                .backgroundColor        = GameManager.instance.SaveData.IsWordFound(level.Category, level.LevelNumber, i - 1) ?
                                            UIManager.instance.GetColor(i - 1) :
                                            new Color(
                                                UIManager.instance.GetColor(i - 1).r
                                                , UIManager.instance.GetColor(i - 1).g
                                                , UIManager.instance.GetColor(i - 1).b
                                                , .4f
                                            );

            currentBadge.Q<Label>(UIManager.LEVEL_SELECT_BADGE__WORD_NAME)
                .text                   = GameManager.instance.SaveData.IsWordFound(level.Category, level.LevelNumber, i - 1) ?
                                            level.Words[i - 1].ToUpper() :
                                            UIManager.LEVEL_SELECT_BADGE__UNFOUND_WORD_NAME;
        }

        VisualElement secretBadge       = root.Q<VisualElement>(UIManager.LEVEL_SELECT_BADGE__SECRET_BADGE_NAME);

        if (level.Category == LevelCategory.DAILY)
        {
            secretBadge.SetVisibility(false);
        }
        else
        {
            secretBadge.style
                .backgroundColor        = GameManager.instance.SaveData.IsSecretWordFound(level) ?
                                            UIManager.instance.GetColor(-1) :
                                            new Color(
                                                UIManager.instance.GetColor(-1).r
                                                , UIManager.instance.GetColor(-1).g
                                                , UIManager.instance.GetColor(-1).b
                                                , .4f
                                            );

            secretBadge.Q<Label>(UIManager.LEVEL_SELECT_BADGE__WORD_NAME)
                .text                   = GameManager.instance.SaveData.IsSecretWordFound(level.Category, level.LevelNumber) ?
                                            level.SecretWord.ToUpper()
                                            : UIManager.LEVEL_SELECT_BADGE__UNFOUND_WORD_NAME;
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

            var minutes                 = GameManager.instance.SaveData.DailyPuzzleTimeInSeconds / 60;
            var seconds                 = GameManager.instance.SaveData.DailyPuzzleTimeInSeconds % 60;
            string timeString           = string.Format("{0:00} : {1:00}", minutes, seconds);

            if (GameManager.instance.SaveData.IsLevelComplete_Daily(PlayFabManager.instance.ServerDate))
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

    public void RegisterOnClick(System.Action onClick)
    {
        root.RegisterButtonStateVisualChanges(root.ElementAt(0), Color.white, true, Color.white);
        root.RegisterCallback<ClickEvent>((_) => onClick.Invoke());
    }

    #endregion
}
