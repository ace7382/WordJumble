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

        if (PlayFabManager.instance.DailyLevel.IsNull)
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

        VisualElement levelBadge    = UIManager.instance.LevelBadge.Instantiate();
        LevelBadge controller       = new LevelBadge(levelBadge, PlayFabManager.instance.DailyLevel);

        dailyLevelButtonContainer.Add(levelBadge);

        Action onClick              = delegate { OpenDailyLevel(null); };

        Tween t = DOTween.To(
            () => dailyLevelButtonContainer.transform.scale
            , (x) => dailyLevelButtonContainer.transform.scale = x
            , Vector3.one
            , animInTime
        ).Play()
        .OnComplete(() => {
            if (!GameManager.instance.SaveData.IsLevelComplete_Daily())
                controller.RegisterOnClick(onClick);
        });
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