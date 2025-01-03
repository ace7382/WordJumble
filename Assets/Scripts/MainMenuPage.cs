using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System;
using System.Linq;

public class MainMenuPage : Page
{
    #region Private Variables

    private bool            canClick;

    private VisualElement   dailyLevelButtonContainer;
    private Button          beginnerButton;
    private Button          originalButton;
    private Button          fiveWordButton;
    private Button          sixWordButton;

    private Level           dailyLevel;

    #endregion

    #region Inherited Functions

    public override IEnumerator AnimateIn()
    {

        canClick = true;
        return null;
    }

    public override IEnumerator AnimateOut()
    {
        canClick = false;

        return null;
    }

    public override void HidePage()
    {
        UnregisterCallbacksAndEvents();
    }

    public override void ShowPage(object[] args)
    {
        canClick    = false;

        //TODO: Calculate based on years vs hard dates
        //      or link to firebase or something

        string path = "Levels/Daily/"
                        + DateTime.Now.Year.ToString() + "/"
                        + DateTime.Now.Month.ToString() + "/"
                        + DateTime.Now.Day.ToString();

        dailyLevel  = Resources.Load<Level>(path);

        GameManager.instance.CurrentLevelCategory = LevelCategory.NULL;

        SetupUI();
        RegisterCallbacksAndEvents();
    }

    #endregion
        
    #region Private Functions

    private void SetupUI()
    {
        VisualElement icon          = uiDoc.rootVisualElement.Q(UIManager.MAIN_MENU_PAGE__ICON_NAME);

        Tween rotateIcon = DOTween.To(
                () => icon.worldTransform.rotation.eulerAngles,
                x => icon.transform.rotation = Quaternion.Euler(x),
                new Vector3(0f, 0f, 360f), 5.0f).SetEase(Ease.Linear).SetLoops(-1);

        beginnerButton              = uiDoc.rootVisualElement.Q<Button>(UIManager.MAIN_MENU_PAGE__BEGINNER_BUTTON_NAME);
        originalButton              = uiDoc.rootVisualElement.Q<Button>(UIManager.MAIN_MENU_PAGE__ORIGINAL_BUTTON_NAME);
        fiveWordButton              = uiDoc.rootVisualElement.Q<Button>(UIManager.MAIN_MENU_PAGE__5_WORDS_BUTTON_NAME);
        sixWordButton               = uiDoc.rootVisualElement.Q<Button>(UIManager.MAIN_MENU_PAGE__6_WORDS_BUTTON_NAME);
        dailyLevelButtonContainer   = uiDoc.rootVisualElement.Q<VisualElement>(UIManager.MAIN_MENU_PAGE__DAILY_CONTAINER_NAME);

        uiDoc.rootVisualElement.Q<Label>(UIManager.MAIN_MENU_PAGE__DAILY_LABEL_NAME)
            .text                   = "Daily Jumblie - " + DateTime.Now.ToString("M/d/yyyy");

        //Setup Daily Jumblie Button
        VisualElement levelButton   = UIManager.instance.LevelTile.Instantiate();
        VisualElement wordContainer = levelButton.Q("WordContainer");

        levelButton.Q<Label>(UIManager.LEVEL_SELECT_BADGE__THEME_NAME)
            .text                   = dailyLevel.Theme;

        if (dailyLevel.Complete)
        {
            levelButton.Q(UIManager.LEVEL_SELECT_BADGE__COMPLETE_ICON_NAME).Show();
            levelButton.ElementAt(0).SetBorderColor(Color.black);

            //TODO: Add a timer/score for completed levels on the badge
        }

        for (int i = 0; i < dailyLevel.Words.Count; i++)
        {
            //TODO: Create a controller? Similar instantiation on GamePage setup
            VisualElement badge     = UIManager.instance.SolvedWordTile.Instantiate();
            badge.SetMargins(5f);

            badge.ElementAt(0).style
                .backgroundColor    = dailyLevel.FoundWords[i] ? UIManager.instance.GetColor(i) :
                                        new Color(
                                            UIManager.instance.GetColor(i).r
                                            , UIManager.instance.GetColor(i).g
                                            , UIManager.instance.GetColor(i).b
                                            , .4f
                                        );

            Label word              = badge.Q<Label>(UIManager.SOLVED_WORD__WORD_NAME);
            word.text               = dailyLevel.FoundWords[i] ? dailyLevel.Words[i].ToUpper() : "???";
            word.style.fontSize     = UIManager.GLOBAL_STYLE__SMALL_WORD_BADGE_WORDS_FONT_SIZE;

            Label icons             = badge.Q<Label>(UIManager.SOVLED_WORD__LENGTH_INDICATOR_NAME);
            icons.text              = new string(UIManager.WORD_LENGTH_INDICATOR_SYMBOL, i + 1).Aggregate(string.Empty, (c, i) => c + i + ' ').TrimEnd();
            icons.style.fontSize    = UIManager.GLOBAL_STYLE__SMALL_WORD_BADGE_ICONS_FONT_SIZE;

            wordContainer.Add(badge);
        }

        levelButton.RegisterCallback<ClickEvent>((_) => OpenDailyLevel(_));

        dailyLevelButtonContainer.Add(levelButton);
        /////////////
    }

    private void RegisterCallbacksAndEvents()
    {
        beginnerButton.clicked  += () => GoToLevelSelect(LevelCategory.BEGINNER);
        originalButton.clicked  += () => GoToLevelSelect(LevelCategory.ORIGINAL);
        fiveWordButton.clicked  += () => GoToLevelSelect(LevelCategory.FIVE_WORD);
        sixWordButton.clicked   += () => GoToLevelSelect(LevelCategory.SIX_WORD);
    }

    private void UnregisterCallbacksAndEvents()
    {
        beginnerButton.clicked  -= () => GoToLevelSelect(LevelCategory.BEGINNER);
        originalButton.clicked  -= () => GoToLevelSelect(LevelCategory.ORIGINAL);
        fiveWordButton.clicked  -= () => GoToLevelSelect(LevelCategory.FIVE_WORD);
        sixWordButton.clicked   -= () => GoToLevelSelect(LevelCategory.SIX_WORD);
    }

    private void GoToLevelSelect(LevelCategory category)
    {
        if (!canClick)
            return;

        canClick        = false;

        object[] args   = new object[1];
        args[0]         = category;

        PageManager.instance.StartCoroutine(PageManager.instance.OpenPageOnAnEmptyStack<LevelSelectPage>(args));
    }

    private void OpenDailyLevel(ClickEvent _)
    {
        if (!canClick)
            return;

        canClick        = false;

        //object[] args   = new object[2];
        //args[0]         = true;
        //args[1]         = dailyLevel;

        //PageManager.instance.StartCoroutine(PageManager.instance.OpenPageOnAnEmptyStack<GamePage>(args));

        object[] args   = new object[2];
        args[0]         = typeof(GamePage);
        args[1]         = new object[2] { true, dailyLevel };

        PageManager.instance.StartCoroutine(PageManager.instance.AddPageToStack<PageLoadAnimationPage>(args));
    }

    #endregion
}