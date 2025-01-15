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

    private LevelCategory       levelCat;
    private List<NewLevel>      levels;
    private List<LevelBadge>    badges;

    private Label               titleLabel;
    private VisualElement       levelIconContainer;
    private Label               completeCounter;
    private Label               secretCounter;
    private VisualElement       backButton;
    private Label               hideCompletedButton;
    private VisualElement       gridButton;
    private VisualElement       listButton;

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
        badges              = new List<LevelBadge>();

        titleLabel          = uiDoc.rootVisualElement.Q<Label>(UIManager.LEVEL_SELECT_PAGE__TITLE_NAME);
        levelIconContainer  = uiDoc.rootVisualElement.Q<ScrollView>(UIManager.LEVEL_SELECT_PAGE__ICON_CONTAINER_NAME).contentContainer;
        completeCounter     = uiDoc.rootVisualElement.Q<Label>(UIManager.LEVEL_SELECT_PAGE__COMPLETE_COUNTER_NAME);
        secretCounter       = uiDoc.rootVisualElement.Q<Label>(UIManager.LEVEL_SELECT_PAGE__SECRET_COUNTER_NAME);
        backButton          = uiDoc.rootVisualElement.Q<VisualElement>(UIManager.LEVEL_SELECT_PAGE__BACK_BUTTON_NAME);
        hideCompletedButton = uiDoc.rootVisualElement.Q<Label>(UIManager.LEVEL_SELECT_PAGE__HIDE_COMP_BUTTON_NAME);
        gridButton          = uiDoc.rootVisualElement.Q<VisualElement>(UIManager.LEVEL_SELECT_PAGE__GRID_BUTTON_NAME);
        listButton          = uiDoc.rootVisualElement.Q<VisualElement>(UIManager.LEVEL_SELECT_PAGE__LIST_BUTTON_NAME);

        titleLabel.text     = levelCat.Name();

        levelIconContainer.style
            .justifyContent = Justify.SpaceAround;

        SwitchLevelBadgeDisplayMode();

        int completeCount   = 0;
        int secretCount     = 0;

        foreach (NewLevel level in levels)
        {
            VisualElement levelBadge    = UIManager.instance.LevelBadge.Instantiate();
            LevelBadge controller       = new LevelBadge(levelBadge, level, false);
            System.Action onClick       = delegate
                                        {
                                            LoadLevel(null, level);
                                        };

            controller.RegisterOnClick(onClick);
            levelIconContainer.Add(levelBadge);

            if (GameManager.instance.SaveData.IsLevelComplete(level))
                completeCount++;

            if (GameManager.instance.SaveData.IsSecretWordFound(level))
                secretCount++;

            completeCounter.text        = completeCount.ToString() + " / " + levels.Count.ToString();
            secretCounter.text          = secretCount.ToString() + " / " + levels.Count.ToString();

            badges.Add(controller);
        }

        QuickButton gridControl = new QuickButton(gridButton, Color.black);
        QuickButton listControl = new QuickButton(listButton, Color.black);
        gridButton.AddManipulator(gridControl);
        listButton.AddManipulator(listControl);
    }

    private void RegisterCallbacksAndEvents()
    {
        backButton.RegisterCallback<ClickEvent>((_) => ReturnToMainMenu());
        hideCompletedButton.RegisterCallback<ClickEvent>((_) => ShowHideCompleted());

        backButton.RegisterButtonStateVisualChanges(backButton, Color.black, false, Color.white);
        hideCompletedButton.RegisterButtonStateVisualChanges(hideCompletedButton, Color.black, false, Color.white);

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
            NewLevel lev        = (NewLevel)child.userData;
            bool completed      = GameManager.instance.SaveData.IsLevelComplete(lev);

            child.Show(showCompleted || completed == showCompleted);
        }

        hideCompletedButton.text = showCompleted ? "Hide Completed" : "Show Completed";
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
