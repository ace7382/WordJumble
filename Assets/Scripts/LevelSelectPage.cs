using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;

public class LevelSelectPage : Page
{
    #region Private Variables

    private bool            canClick;
    private bool            showCompleted           = true;

    private LevelCategory   levelCat;
    private List<NewLevel>  levels;

    private Label           titleLabel;
    private VisualElement   levelIconContainer;
    private Label           completeCounter;
    private Label           secretCounter;
    private Label           backButton;
    private Label           hideCompletedButton;

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
        titleLabel          = uiDoc.rootVisualElement.Q<Label>(UIManager.LEVEL_SELECT_PAGE__TITLE_NAME);
        levelIconContainer  = uiDoc.rootVisualElement.Q<ScrollView>(UIManager.LEVEL_SELECT_PAGE__ICON_CONTAINER_NAME).contentContainer;
        completeCounter     = uiDoc.rootVisualElement.Q<Label>(UIManager.LEVEL_SELECT_PAGE__COMPLETE_COUNTER_NAME);
        secretCounter       = uiDoc.rootVisualElement.Q<Label>(UIManager.LEVEL_SELECT_PAGE__SECRET_COUNTER_NAME);
        backButton          = uiDoc.rootVisualElement.Q<Label>(UIManager.LEVEL_SELECT_PAGE__BACK_BUTTON_NAME);
        hideCompletedButton = uiDoc.rootVisualElement.Q<Label>(UIManager.LEVEL_SELECT_PAGE__HIDE_COMP_BUTTON_NAME);

        titleLabel.text     = levelCat.Name();

        int completeCount   = 0;
        int secretCount     = 0;

        foreach (NewLevel level in levels)
        {
            VisualElement levelButton   = UIManager.instance.LevelTile.Instantiate();
            VisualElement wordContainer = levelButton.Q("WordContainer");

            levelButton.userData        = level;

            levelButton.Q<Label>(UIManager.LEVEL_SELECT_BADGE__THEME_NAME)
                .text                   = level.LevelNumber.ToString() + " - " + level.Theme;

            if (GameManager.instance.SaveData.IsLevelComplete(level.Category, level.LevelNumber))
            {
                levelButton.Q(UIManager.LEVEL_SELECT_BADGE__COMPLETE_ICON_NAME).Show();
                levelButton.ElementAt(0).SetBorderColor(Color.black);
                completeCount++;
            }

            for (int i = 0; i < level.Words.Count; i++)
            {
                //TODO: Create a controller? Similar instantiation on GamePage setup
                VisualElement badge     = UIManager.instance.SolvedWordTile.Instantiate();
                badge.SetMargins(5f);

                badge.ElementAt(0).style
                    .backgroundColor    = GameManager.instance.SaveData.IsWordFound(level.Category, level.LevelNumber, i) ? UIManager.instance.GetColor(i) :
                                            new Color(
                                                UIManager.instance.GetColor(i).r
                                                , UIManager.instance.GetColor(i).g
                                                , UIManager.instance.GetColor(i).b
                                                , .4f
                                            );

                Label word              = badge.Q<Label>(UIManager.SOLVED_WORD__WORD_NAME);
                word.text               = GameManager.instance.SaveData.IsWordFound(level.Category, level.LevelNumber, i) ? level.Words[i].ToUpper() : "???";
                word.style.fontSize     = UIManager.GLOBAL_STYLE__SMALL_WORD_BADGE_WORDS_FONT_SIZE;

                Label icons             = badge.Q<Label>(UIManager.SOVLED_WORD__LENGTH_INDICATOR_NAME);
                icons.text              = new string(UIManager.WORD_LENGTH_INDICATOR_SYMBOL, i + 1).Aggregate(string.Empty, (c, i) => c + i + ' ').TrimEnd();
                icons.style.fontSize    = UIManager.GLOBAL_STYLE__SMALL_WORD_BADGE_ICONS_FONT_SIZE;

                wordContainer.Add(badge);
            }

            //Secret Word Badge
            VisualElement secretBadge   = UIManager.instance.SolvedWordTile.Instantiate();
            secretBadge.SetMargins(10f);

            secretBadge.ElementAt(0).style
                .backgroundColor        = GameManager.instance.SaveData.IsSecretWordFound(level.Category, level.LevelNumber) ? UIManager.instance.GetColor(-1) :
                                            new Color(
                                                UIManager.instance.GetColor(-1).r
                                                , UIManager.instance.GetColor(-1).g
                                                , UIManager.instance.GetColor(-1).b
                                                , .4f
                                            );

            Label sbWord                = secretBadge.Q<Label>(UIManager.SOLVED_WORD__WORD_NAME);
            sbWord.text                 = GameManager.instance.SaveData.IsSecretWordFound(level.Category, level.LevelNumber) ? level.SecretWord.ToUpper() : "???";
            sbWord.style.fontSize       = UIManager.GLOBAL_STYLE__SMALL_WORD_BADGE_WORDS_FONT_SIZE;

            Label sbIcons               = secretBadge.Q<Label>(UIManager.SOVLED_WORD__LENGTH_INDICATOR_NAME);
            sbIcons.text                = UIManager.WORD_LENGTH_INDICATOR_SYMBOL.ToString() + " Secret Word " + UIManager.WORD_LENGTH_INDICATOR_SYMBOL.ToString();
            sbIcons.style.fontSize      = UIManager.GLOBAL_STYLE__SMALL_WORD_BADGE_ICONS_FONT_SIZE;

            wordContainer.Add(secretBadge);

            if (GameManager.instance.SaveData.IsSecretWordFound(level.Category, level.LevelNumber))
                secretCount++;
            //

            completeCounter.text        = "TODO"; //completeCount.ToString() + " / " + levels_old.Count.ToString();
            secretCounter.text          = "TODO"; //secretCount.ToString() + " / " + levels_old.Count.ToString();
            levelButton.RegisterCallback<ClickEvent>((_) => LoadLevel(_, level));


            levelButton.RegisterButtonStateVisualChanges(levelButton.ElementAt(0), Color.white, true, Color.white);

            levelIconContainer.Add(levelButton);
        }
    }

    private void RegisterCallbacksAndEvents()
    {
        backButton.RegisterCallback<ClickEvent>((_) => ReturnToMainMenu());
        hideCompletedButton.RegisterCallback<ClickEvent>((_) => ShowHideCompleted());

        backButton.RegisterButtonStateVisualChanges(backButton, Color.black, false, Color.white);
        hideCompletedButton.RegisterButtonStateVisualChanges(hideCompletedButton, Color.black, false, Color.white);
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
            bool completed      = GameManager.instance.SaveData.IsLevelComplete(lev.Category, lev.LevelNumber);

            child.Show(showCompleted || completed == showCompleted);
        }

        hideCompletedButton.text = showCompleted ? "Hide Completed" : "Show Completed";
    }

    #endregion
}
