using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System;
using System.Linq;

public class MainMenuPage : Page
{
    #region Private Consts

    private const float     animInTime      = .7f;
    private const float     secAnimInTime   = .4f;

    #endregion

    #region Private Variables

    private bool            canClick;

    private VisualElement   dailyLevelButtonContainer;

    private Label           loadingLabel;
    private Label           dailyJumblieLabel;

    private Button          beginnerButton;
    private Button          originalButton;
    private Button          fiveWordButton;
    private Button          sixWordButton;

    private VisualElement   titleContainer;
    private Label           title;
    private Label           subtitle;

    private Vector3         titleContainerPosition;

    #endregion

    #region Inherited Functions

    public override IEnumerator AnimateIn()
    {
        Sequence s = DOTween.Sequence();

        //Tween titleFlyIn = DOTween.To(
        //    () => titleContainer.transform.position
        //    , x => titleContainer.transform.position = x
        //    , titleContainerPosition
        //    , animInTime
        //).SetEase(Ease.InSine);

        Tween titleFlyIn = DOTween.To(
            () => titleContainer.resolvedStyle.translate.y
            , newY => titleContainer.style.translate = new Translate(new Length(0f), new Length(newY), 0f)
            , 0f
            , animInTime
        ).SetEase(Ease.InSine);

        Tween titleFadeIn = DOTween.To(
            () => titleContainer.style.opacity.value
            , x => titleContainer.style.opacity = new StyleFloat(x)
            , 1f
            , animInTime
        ).SetEase(Ease.InSine);

        s.Join(titleFlyIn);
        s.Join(titleFadeIn);

        subtitle.SetOpacity(0f);

        Tween subFadeIn     = DOTween.To(
            () => subtitle.style.opacity.value
            , x => subtitle.style.opacity = new StyleFloat(x)
            , 1f
            , secAnimInTime
        ).SetEase(Ease.InSine);

        s.Append(subFadeIn);

        VisualElement dividerLine = uiDoc.rootVisualElement.Q<VisualElement>("DividerLine");
        dividerLine.style.width = Length.Percent(0f);

        Tween lineGrow = DOTween.To(
            () => dividerLine.style.width.value.value
            , x => dividerLine.style.width = Length.Percent(x)
            , 100f
            , secAnimInTime
        ).SetEase(Ease.InSine);

        s.Join(lineGrow);

        yield return s.Play().WaitForCompletion();

        canClick = true;
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

        //string path = "Levels/Daily/"
        //                + DateTime.Now.Year.ToString() + "/"
        //                + DateTime.Now.Month.ToString() + "/"
        //                + DateTime.Now.Day.ToString();

        //dailyLevel  = Resources.Load<Level>(path);

        GameManager.instance.CurrentLevelCategory = LevelCategory.NULL;

        PlayFabManager.instance.DeviceLogin();

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

        titleContainer              = uiDoc.rootVisualElement.Q<VisualElement>(UIManager.MAIN_MENU_PAGE__TITLE_CONTAINER_NAME);
        title                       = uiDoc.rootVisualElement.Q<Label>(UIManager.MAIN_MENU_PAGE__TITLE_NAME);
        subtitle                    = uiDoc.rootVisualElement.Q<Label>(UIManager.MAIN_MENU_PAGE__SUBTITLE_NAME);

        //titleContainerPosition      = titleContainer.transform.position;
        //titleContainer.transform
        //    .position               = new Vector3(titleContainerPosition.x, -200f, titleContainerPosition.z);

        beginnerButton              = uiDoc.rootVisualElement.Q<Button>(UIManager.MAIN_MENU_PAGE__BEGINNER_BUTTON_NAME);
        originalButton              = uiDoc.rootVisualElement.Q<Button>(UIManager.MAIN_MENU_PAGE__ORIGINAL_BUTTON_NAME);
        fiveWordButton              = uiDoc.rootVisualElement.Q<Button>(UIManager.MAIN_MENU_PAGE__5_WORDS_BUTTON_NAME);
        sixWordButton               = uiDoc.rootVisualElement.Q<Button>(UIManager.MAIN_MENU_PAGE__6_WORDS_BUTTON_NAME);
        dailyLevelButtonContainer   = uiDoc.rootVisualElement.Q<VisualElement>(UIManager.MAIN_MENU_PAGE__DAILY_CONTAINER_NAME);
        dailyJumblieLabel           = uiDoc.rootVisualElement.Q<Label>(UIManager.MAIN_MENU_PAGE__DAILY_LABEL_NAME);
        loadingLabel                = uiDoc.rootVisualElement.Q<Label>(UIManager.MAIN_MENU_PAGE__LOADING_LABEL_NAME);

        dailyJumblieLabel.Hide();
        loadingLabel.Hide();
        dailyLevelButtonContainer.transform.scale = new Vector3(1f, 0f, 1f);

        if (PlayFabManager.instance.DailyLevel == null)
        {
            ShowLoading();
        }
        else
        {
            SetupDailyJumblieButton();
        }
    }

    private void HandleDailyLevelNotification(object sender, object info)
    {
        this.RemoveObserver(HandleDailyLevelNotification, Notifications.DAILY_PUZZLE_LOADED_FROM_SERVER);

        Tween fadeOut = DOTween.To(
            () => loadingLabel.resolvedStyle.opacity
            , x => loadingLabel.style.opacity = new StyleFloat(x)
            , 0f
            , .5f
        )
        .Play()
        .OnComplete(() => { loadingLabel.Hide(); SetupDailyJumblieButton(); });
    }

    private void ShowLoading()
    {
        this.AddObserver(HandleDailyLevelNotification, Notifications.DAILY_PUZZLE_LOADED_FROM_SERVER);
        loadingLabel.Show();
    }

    private void SetupDailyJumblieButton()
    {
        dailyJumblieLabel.text      = "Daily Jumblie - " + PlayFabManager.instance.ServerDate.ToString("M/d/yyyy");
        dailyJumblieLabel.Show();

        //Setup Daily Jumblie Button
        VisualElement levelButton   = UIManager.instance.LevelTile.Instantiate();
        VisualElement wordContainer = levelButton.Q("WordContainer");

        levelButton.Q<Label>(UIManager.LEVEL_SELECT_BADGE__THEME_NAME)
            .text                   = PlayFabManager.instance.DailyLevel.Theme;

        if(PlayFabManager.instance.DailyLevel.Complete)
        {
            levelButton.Q(UIManager.LEVEL_SELECT_BADGE__COMPLETE_ICON_NAME).Show();
            levelButton.ElementAt(0).SetBorderColor(Color.black);

            //TODO: Add a timer/score for completed levels on the badge
        }

        for (int i = 0; i < PlayFabManager.instance.DailyLevel.Words.Count; i++)
        {
            //TODO: Create a controller? Similar instantiation on GamePage setup
            VisualElement badge     = UIManager.instance.SolvedWordTile.Instantiate();
            badge.SetMargins(5f);

            badge.ElementAt(0).style
                .backgroundColor    = PlayFabManager.instance.DailyLevel.FoundWords[i] ?
                                        UIManager.instance.GetColor(i) :
                                        new Color(
                                            UIManager.instance.GetColor(i).r
                                            , UIManager.instance.GetColor(i).g
                                            , UIManager.instance.GetColor(i).b
                                            , .4f
                                        );

            Label word              = badge.Q<Label>(UIManager.SOLVED_WORD__WORD_NAME);
            word.text = PlayFabManager.instance.DailyLevel.FoundWords[i] ? PlayFabManager.instance.DailyLevel.Words[i].ToUpper() : "???";
            word.style.fontSize     = UIManager.GLOBAL_STYLE__SMALL_WORD_BADGE_WORDS_FONT_SIZE;

            Label icons             = badge.Q<Label>(UIManager.SOVLED_WORD__LENGTH_INDICATOR_NAME);
            icons.text              = new string(UIManager.WORD_LENGTH_INDICATOR_SYMBOL, i + 1).Aggregate(string.Empty, (c, i) => c + i + ' ').TrimEnd();
            icons.style.fontSize    = UIManager.GLOBAL_STYLE__SMALL_WORD_BADGE_ICONS_FONT_SIZE;

            wordContainer.Add(badge);
        }

        //levelButton.RegisterCallback<ClickEvent>((_) => OpenDailyLevel(_));

        levelButton.RegisterButtonStateVisualChanges(levelButton.ElementAt(0), Color.white, true, Color.white); 

        dailyLevelButtonContainer.Add(levelButton);
        /////////////

        Tween t = DOTween.To(
            () => dailyLevelButtonContainer.transform.scale
            , (x) => dailyLevelButtonContainer.transform.scale = x
            , Vector3.one
            , animInTime
        ).Play()
        .OnComplete(() => levelButton.RegisterCallback<ClickEvent>((_) => OpenDailyLevel(_)));
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

        object[] args   = new object[2];
        args[0]         = typeof(GamePage);
        args[1]         = new object[2] { true, PlayFabManager.instance.DailyLevel };

        PageManager.instance.StartCoroutine(PageManager.instance.AddPageToStack<PageLoadAnimationPage>(args));
    }

    #endregion
}