using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;

public class LevelSelectPage : Page
{
    #region Private Variables

    private bool                canClick;
    private bool                showCompleted           = true;
    private bool                showSecretFound         = true;

    private LevelCategory       levelCat;
    private List<NewLevel>      levels;
    private List<LevelBadge>    badges;

    private Label               titleLabel;
    private VisualElement       levelIconContainer;
    private Label               completeCounter;
    private Label               secretCounter;
    private VisualElement       backButton;
    private Label               hideCompletedButton;
    private Label               hideSecretFoundButton;
    private VisualElement       gridButton;
    private VisualElement       listButton;
    private Label               allCompleteMessage;

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
        //args[0]   -   Level Category  -   The type of levels to display

        levelCat    = (LevelCategory)args[0];
        levels      = LevelDefinitions.ALL_LEVELS.FindAll(x => x.Category == levelCat);

        GameManager.instance.CurrentLevelCategory = levelCat;

        SetupUI();
        RegisterCallbacksAndEvents();
    }

    #endregion

    #region Private Functions

    private void SetupUI()
    {
        badges                  = new List<LevelBadge>();

        titleLabel              = uiDoc.rootVisualElement.Q<Label>(UIManager.LEVEL_SELECT_PAGE__TITLE_NAME);
        levelIconContainer      = uiDoc.rootVisualElement.Q<ScrollView>(UIManager.LEVEL_SELECT_PAGE__ICON_CONTAINER_NAME).contentContainer;
        completeCounter         = uiDoc.rootVisualElement.Q<Label>(UIManager.LEVEL_SELECT_PAGE__COMPLETE_COUNTER_NAME);
        secretCounter           = uiDoc.rootVisualElement.Q<Label>(UIManager.LEVEL_SELECT_PAGE__SECRET_COUNTER_NAME);
        backButton              = uiDoc.rootVisualElement.Q<VisualElement>(UIManager.LEVEL_SELECT_PAGE__BACK_BUTTON_NAME);
        hideCompletedButton     = uiDoc.rootVisualElement.Q<Label>(UIManager.LEVEL_SELECT_PAGE__HIDE_COMP_BUTTON_NAME);
        hideSecretFoundButton   = uiDoc.rootVisualElement.Q<Label>(UIManager.LEVEL_SELECT_PAGE__HIDE_SECRET_BUTTON_NAME);
        gridButton              = uiDoc.rootVisualElement.Q<VisualElement>(UIManager.LEVEL_SELECT_PAGE__GRID_BUTTON_NAME);
        listButton              = uiDoc.rootVisualElement.Q<VisualElement>(UIManager.LEVEL_SELECT_PAGE__LIST_BUTTON_NAME);
        allCompleteMessage      = uiDoc.rootVisualElement.Q<Label>(UIManager.LEVEL_SELECT_PAGE__ALL_COMPLETE_TEXT_NAME);

        titleLabel.text         = levelCat.Name();

        levelIconContainer.style
            .justifyContent = Justify.SpaceAround;

        SwitchLevelBadgeDisplayMode();

        int completeCount   = 0;
        int levsWithSecret  = 0;
        int secretCount     = 0;

        foreach (NewLevel level in levels)
        {
            VisualElement levelBadge    = UIManager.instance.LevelBadge.Instantiate();
            LevelBadge controller       = new LevelBadge(levelBadge, level, false);
            System.Action onClick       = delegate
                                        {
                                            LoadLevel(null, level);
                                        };

            levelBadge.userData         = level;

            controller.RegisterOnClick(onClick);
            levelIconContainer.Add(levelBadge);

            if (GameManager.instance.SaveData.IsLevelComplete(level))
                completeCount++;

            if (level.HasSecretWord)
            {
                levsWithSecret++;

                if (GameManager.instance.SaveData.IsSecretWordFound(level))
                    secretCount++;
            }

            completeCounter.text        = completeCount.ToString() + " / " + levels.Count.ToString();
            secretCounter.text = secretCount.ToString() + " / " + levsWithSecret.ToString();

            badges.Add(controller);
        }
    }

    private void RegisterCallbacksAndEvents()
    {
        QuickButton backQB      = new QuickButton(backButton, Color.black);
        QuickButton hideQB      = new QuickButton(hideCompletedButton, Color.black);
        QuickButton hideSecQB   = new QuickButton(hideSecretFoundButton, Color.black);
        QuickButton gridControl = new QuickButton(gridButton, Color.black);
        QuickButton listControl = new QuickButton(listButton, Color.black);

        backButton.AddManipulator(backQB);
        hideCompletedButton.AddManipulator(hideQB);
        hideSecretFoundButton.AddManipulator(hideSecQB);
        gridButton.AddManipulator(gridControl);
        listButton.AddManipulator(listControl);

        backButton.RegisterCallback<ClickEvent>((_) => ReturnToMainMenu());

        hideCompletedButton.RegisterCallback<ClickEvent>((_) => {
            if (!canClick)
                return;

            showCompleted = !showCompleted;

            ShowHideLevelButtons();
        });

        hideSecretFoundButton.RegisterCallback<ClickEvent>((_) => {
            if (!canClick)
                return;

            showSecretFound = !showSecretFound;

            ShowHideLevelButtons();
        });

        gridButton.RegisterCallback<ClickEvent>(_ => SwitchLevelBadgeDisplayMode());
        listButton.RegisterCallback<ClickEvent>(_ => SwitchLevelBadgeDisplayMode());
    }

    private void UnregisterCallbacksAndEvents()
    {
        backButton.UnregisterCallback<ClickEvent>((_) => ReturnToMainMenu());
        hideCompletedButton.UnregisterCallback<ClickEvent>((_) => ShowHideCompleted());
    }

    private void LoadLevel(ClickEvent _, NewLevel level)
    {
        if (!canClick)
            return;

        canClick        = false;

        object[] args   = new object[2];
        args[0]         = typeof(GamePage);
        args[1]         = new object[2] { false, level };

        PageManager.instance.StartCoroutine(PageManager.instance.AddPageToStack<PageLoadAnimationPage>(args));
    }

    private void ReturnToMainMenu()
    {
        if (!canClick)
            return;

        canClick = false;

        PageManager.instance.StartCoroutine(PageManager.instance.OpenPageOnAnEmptyStack<MainMenuPage>());
    }

    private void ShowHideCompleted()
    {
        if (!canClick)
            return;

        showCompleted = !showCompleted;

        foreach (VisualElement child in levelIconContainer.Children())
        {
            if (child == allCompleteMessage)
                continue;

            NewLevel lev        = (NewLevel)child.userData;
            bool completed      = GameManager.instance.SaveData.IsLevelComplete(lev);

            child.Show(showCompleted || completed == showCompleted);
        }

        hideCompletedButton.text = showCompleted ? "Hide Completed" : "Show Completed";
    }

    private void ShowHideLevelButtons()
    {
        //TODO: Remove both hide buttons, and just make a "Hide Fully Completed" button

        if (!canClick)
            return;

        //Hide completed    = hide all complete levels, regardless of secret
        //Hide secret       = hide all levels without a secret, or w secret found
        //
        //button.Show(
        //  (showCompleted || completed == showCompleted) &&
        //  (showSecret || showSecret == level.hasSecret || showSecret == secretFound)
        //)

        bool showAllComplete        = true;

        foreach (VisualElement child in levelIconContainer.Children())
        {
            if (child == allCompleteMessage)
                continue;

            NewLevel level          = (NewLevel)child.userData;
            bool completed          = GameManager.instance.SaveData.IsLevelComplete(level);

            //child.Show(
            //    (showCompleted || completed == showCompleted)
            //    &&  showSecretFound
            //        || showSecretFound == lev.HasSecretWord
            //        || showSecretFound == GameManager.instance.SaveData.IsSecretWordFound(lev)
            //    );

            if (showCompleted && showSecretFound)
                child.Show();
            else if (showCompleted && !showSecretFound)
                if (level.HasSecretWord)
                    child.Show(!GameManager.instance.SaveData.IsSecretWordFound(level));
                else
                    child.Hide();
            else if (!showCompleted && showSecretFound)
                child.Show(!completed);
            else //Hide completed, hide secret found
                child.Show(!GameManager.instance.SaveData.IsLevelFullyComplete(level));

            if (child.IsShowing())
                showAllComplete     = false;
        }

        allCompleteMessage.Show(showAllComplete);

        hideCompletedButton.text    = showCompleted ? "Hide Completed" : "Show Completed";
        hideSecretFoundButton.text  = showSecretFound ? "Hide Secret Found" : "Show Secret Found";
    }


    private void SwitchLevelBadgeDisplayMode()
    {
        listButton.Show(!listButton.IsShowing());
        gridButton.Show(!listButton.IsShowing());

        if (listButton.IsShowing())
        {
            levelIconContainer.style.flexDirection  = FlexDirection.Column;
            levelIconContainer.style.flexWrap       = Wrap.NoWrap;
        }
        else
        {
            levelIconContainer.style.flexWrap       = Wrap.Wrap;
            levelIconContainer.style.flexDirection  = FlexDirection.Row;
            //levelIconContainer.style.justifyContent = Justify.SpaceAround;
        }

        foreach (LevelBadge badge in badges)
        {
            badge.SwapDisplayMode(listButton.IsShowing());
        }
    }

    #endregion
}
