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

    private VisualElement   menuContainer;
    private VisualElement   redButton;
    private VisualElement   yellowButton;
    private VisualElement   blueButton;
    private VisualElement   greenButton;

    private QuickButton     redQB;
    private QuickButton     yellowQB;
    private QuickButton     blueQB;
    private QuickButton     greenQB;

    private VisualElement   titleContainer;
    private Label           title;
    private Label           subtitle;

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

        //Tween rotateMenu = DOTween.To(
        //    () => menuContainer.style.rotate.value.angle.value
        //    , x => menuContainer.style.rotate = new StyleRotate(new Rotate(new Angle(x)))
        //    , 360f
        //    , animInTime
        //).SetEase(Ease.InSine);

        redButton.transform.scale = Vector3.zero;
        blueButton.transform.scale = Vector3.zero;
        yellowButton.transform.scale = Vector3.zero;
        greenButton.transform.scale = Vector3.zero;

        Tween redIn = DOTween.To(
            () => redButton.transform.scale
            , x => redButton.transform.scale = x
            , Vector3.one
            , secAnimInTime
        ).SetEase(Ease.OutBack);

        Tween blueIn = DOTween.To(
            () => blueButton.transform.scale
            , x => blueButton.transform.scale = x
            , Vector3.one
            , secAnimInTime
        ).SetEase(Ease.OutBack);

        Tween yellowIn = DOTween.To(
            () => yellowButton.transform.scale
            , x => yellowButton.transform.scale = x
            , Vector3.one
            , secAnimInTime
        ).SetEase(Ease.OutBack);

        Tween greenIn = DOTween.To(
            () => greenButton.transform.scale
            , x => greenButton.transform.scale = x
            , Vector3.one
            , secAnimInTime
        ).SetEase(Ease.OutBack);

        System.Random r = new System.Random();

        s.Insert((float)r.NextDouble() * .8f * animInTime, redIn);
        s.Insert((float)r.NextDouble() * .8f * animInTime, yellowIn);
        s.Insert((float)r.NextDouble() * .8f * animInTime, blueIn);
        s.Insert((float)r.NextDouble() * .8f * animInTime, greenIn);

        yield return s.Play().WaitForCompletion();

        canClick = true;

        redQB = new QuickButton(redButton, UIManager.instance.GetColor(0));
        redButton.AddManipulator(redQB);

        blueQB = new QuickButton(blueButton, UIManager.instance.GetColor(3));
        blueButton.AddManipulator(blueQB);

        greenQB = new QuickButton(greenButton, UIManager.instance.GetColor(2));
        greenButton.AddManipulator(greenQB);

        yellowQB = new QuickButton(yellowButton, UIManager.instance.GetColor(1));
        yellowButton.AddManipulator(yellowQB);
    }

    public override IEnumerator AnimateOut()
    {
        canClick = false;

        redButton.RemoveManipulator(redQB);
        blueButton.RemoveManipulator(blueQB);
        greenButton.RemoveManipulator(greenQB);
        yellowButton.RemoveManipulator(yellowQB);

        Sequence s = DOTween.Sequence();

        Tween redOut = DOTween.To(
            () => redButton.transform.scale
            , x => redButton.transform.scale = x
            , Vector3.zero
            , secAnimInTime
        ).SetEase(Ease.InBack);

        Tween blueOut = DOTween.To(
            () => blueButton.transform.scale
            , x => blueButton.transform.scale = x
            , Vector3.zero
            , secAnimInTime
        ).SetEase(Ease.InBack);

        Tween yellowOut = DOTween.To(
            () => yellowButton.transform.scale
            , x => yellowButton.transform.scale = x
            , Vector3.zero
            , secAnimInTime
        ).SetEase(Ease.InBack);

        Tween greenOut = DOTween.To(
            () => greenButton.transform.scale
            , x => greenButton.transform.scale = x
            , Vector3.zero
            , secAnimInTime
        ).SetEase(Ease.InBack);

        s.Join(redOut);
        s.Join(blueOut);
        s.Join(greenOut);
        s.Join(yellowOut);

        yield return s.Play().WaitForCompletion();
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

        menuContainer               = uiDoc.rootVisualElement.Q<VisualElement>(UIManager.MAIN_MENU_PAGE__MENU_CONTAINER_NAME);
        redButton                   = uiDoc.rootVisualElement.Q<VisualElement>(UIManager.MAIN_MENU_PAGE__RED_BUTTON_NAME);
        yellowButton                = uiDoc.rootVisualElement.Q<VisualElement>(UIManager.MAIN_MENU_PAGE__YELLOW_BUTTON_NAME);
        blueButton                  = uiDoc.rootVisualElement.Q<VisualElement>(UIManager.MAIN_MENU_PAGE__BLUE_BUTTON_NAME);
        greenButton                 = uiDoc.rootVisualElement.Q<VisualElement>(UIManager.MAIN_MENU_PAGE__GREEN_BUTTON_NAME);

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
            //if (!GameManager.instance.SaveData.IsLevelComplete_Daily(PlayFabManager.instance.DailyLevel))
            if (!GameManager.instance.SaveData.IsLevelComplete(PlayFabManager.instance.DailyLevel))
                controller.RegisterOnClick(onClick);
        });
    }

    private void RegisterCallbacksAndEvents()
    {
        redButton.RegisterCallback<ClickEvent>(_ => GoToLevelSelect(LevelCategory.BEGINNER));
        greenButton.RegisterCallback<ClickEvent>(_ => GoToLevelSelect(LevelCategory.ORIGINAL));
        blueButton.RegisterCallback<ClickEvent>(_ => GoToLevelSelect(LevelCategory.ADVANCED));
        yellowButton.RegisterCallback<ClickEvent>(_ => { });
    }

    private void UnregisterCallbacksAndEvents()
    {

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