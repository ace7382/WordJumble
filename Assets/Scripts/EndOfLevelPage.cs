using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;
using DG.Tweening;

public class EndOfLevelPage : Page
{
    #region Private Const

    private const float ANIM_TIME           = 1f;
    private const float FAST_ANIM_TIME      = .3f;

    #endregion

    #region Private Variables

    private bool            canClick            = false;
    private NewLevel        level;

    private int             guessINT;
    private int             wordsINT;
    private int             newWordsINT;

    private Label           header;
    private Label           headerDots;
    private Label           secretWordFound;
    private Label           guessesCount;
    private Label           wordsCount;
    private Label           newWordsCount;
    private Label           categoryTitle;
    private Label           completeCounter;
    private Label           secretCounter;

    private VisualElement   break1;
    private VisualElement   break2;
    private VisualElement   break3;
    private VisualElement   centerPanel;
    private VisualElement   categoryDetails;
    private VisualElement   buttonContainer;

    #endregion

    #region Inherited Functions

    public override IEnumerator AnimateIn()
    {
        uiDoc.rootVisualElement.transform.position = new Vector3(0f, Screen.height, 0f);

        Tween slideIn = DOTween.To(
                () => uiDoc.rootVisualElement.transform.position,
                x => uiDoc.rootVisualElement.transform.position = x,
                Vector3.zero,
                .5f
            );

        yield return slideIn.Play().WaitForCompletion();

        List<VisualElement> inElements = new List<VisualElement>()
        {
            secretWordFound
            , break1
            , centerPanel
            , break2
            , categoryTitle
            , categoryDetails
            , break3
            , buttonContainer
        };

        Sequence elemsIn = DOTween.Sequence();

        foreach (VisualElement ve in inElements)
        {
            Tween scaleIn = DOTween.To(
                () => ve.transform.scale
                , x => ve.transform.scale = x
                , Vector3.one
                , FAST_ANIM_TIME
            ).OnStart(() => ve.SetVisibility(true))
            .SetEase(Ease.InBack);

            Tween shake = DOTween.Shake(
                () => ve.transform.position
                , x => ve.transform.position = x
                , FAST_ANIM_TIME
                , new Vector3(10f, 10f, 0f)
                , 10
                , 0
                , true
            ).SetEase(Ease.OutSine);

            elemsIn.Join(scaleIn).Append(shake);
        }

        yield return elemsIn.Play().WaitForCompletion();

        int currentGuess    = 0;
        int currentWord     = 0;
        int currentNew      = 0;

        Tween countG = DOTween.To(
            () => currentGuess
            , x => guessesCount.text = x.ToString()
            , guessINT
            , ANIM_TIME
        );

        Tween countW = DOTween.To(
            () => currentWord
            , x => wordsCount.text = x.ToString()
            , wordsINT
            , ANIM_TIME
        );

        Tween countNW = DOTween.To(
            () => currentNew
            , x => newWordsCount.text = x.ToString()
            , newWordsINT
            , ANIM_TIME
        );

        Sequence s = DOTween.Sequence();

        s.Join(countG).Join(countW).Join(countNW).SetEase(Ease.Linear);

        yield return s.Play().WaitForCompletion();

        canClick = true;
    }

    public override IEnumerator AnimateOut()
    {
        canClick = false;

        Tween slideOut = DOTween.To(
                () => uiDoc.rootVisualElement.transform.position,
                x => uiDoc.rootVisualElement.transform.position = x,
                new Vector3(0f, Screen.height, 0f),
                .5f
            );

        yield return slideOut.Play().WaitForCompletion();
    }

    public override void HidePage()
    {

    }

    public override void ShowPage(object[] args)
    {
        //args[0]   -   NewLevel    -   The Level that was just completed
        //args[1]   -   int         -   Guesses made during the level
        //args[2]   -   int         -   Words found
        //args[3]   -   int         -   New words found

        level       = (NewLevel)args[0];
        guessINT    = (int)args[1];
        wordsINT    = (int)args[2];
        newWordsINT = (int)args[3];

        SetupUI();
    }

    #endregion

    #region Private Functions

    private void SetupUI()
    {
        header                      = uiDoc.rootVisualElement.Q<Label>(UIManager.END_OF_LEVEL_PAGE__HEADER_NAME);
        headerDots                  = uiDoc.rootVisualElement.Q<Label>(UIManager.END_OF_LEVEL_PAGE__HEADER_DOTS_NAME);
        secretWordFound             = uiDoc.rootVisualElement.Q<Label>(UIManager.END_OF_LEVEL_PAGE__SECRET_FOUND_NAME);
        guessesCount                = uiDoc.rootVisualElement.Q<Label>(UIManager.END_OF_LEVEL_PAGE__GUESSES_NAME);
        wordsCount                  = uiDoc.rootVisualElement.Q<Label>(UIManager.END_OF_LEVEL_PAGE__WORDS_FOUND_NAME);
        newWordsCount               = uiDoc.rootVisualElement.Q<Label>(UIManager.END_OF_LEVEL_PAGE__NEW_WORDS_NAME);
        categoryTitle               = uiDoc.rootVisualElement.Q<Label>(UIManager.END_OF_LEVEL_PAGE__CATEGORY_NAME);
        completeCounter             = uiDoc.rootVisualElement.Q<Label>(UIManager.END_OF_LEVEL_PAGE__COMPLETE_COUNT_NAME);
        secretCounter               = uiDoc.rootVisualElement.Q<Label>(UIManager.END_OF_LEVEL_PAGE__SECRET_COUNT_NAME);

        break1                      = uiDoc.rootVisualElement.Q<VisualElement>(UIManager.END_OF_LEVEL_PAGE__BREAK_NAME + "1");
        break2                      = uiDoc.rootVisualElement.Q<VisualElement>(UIManager.END_OF_LEVEL_PAGE__BREAK_NAME + "2");
        break3                      = uiDoc.rootVisualElement.Q<VisualElement>(UIManager.END_OF_LEVEL_PAGE__BREAK_NAME + "3");
        centerPanel                 = uiDoc.rootVisualElement.Q<VisualElement>(UIManager.END_OF_LEVEL_PAGE__CENTER_PANEL_NAME);
        categoryDetails             = uiDoc.rootVisualElement.Q<VisualElement>(UIManager.END_OF_LEVEL_PAGE__CATEGORY_DETAILS_NAME);
        buttonContainer             = uiDoc.rootVisualElement.Q<VisualElement>(UIManager.END_OF_LEVEL_PAGE__BUTTON_CONTAINER_NAME);

        VisualElement mainMenu      = uiDoc.rootVisualElement.Q<VisualElement>(UIManager.END_OF_LEVEL_PAGE__MAIN_MENU_BUTTON_NAME);
        VisualElement nextLevel     = uiDoc.rootVisualElement.Q<VisualElement>(UIManager.END_OF_LEVEL_PAGE__NEXT_BUTTON_NAME);
        VisualElement findSecret    = uiDoc.rootVisualElement.Q<VisualElement>(UIManager.END_OF_LEVEL_PAGE__SECRET_FIND_BUTTON_NAME);
        VisualElement levelSelect   = uiDoc.rootVisualElement.Q<VisualElement>(UIManager.END_OF_LEVEL_PAGE__LEVEL_SELECT_BUTTON_NAME);
        VisualElement headerBG      = header.parent;

        headerBG.SetColor(UIManager.instance.GetColor(level.Words.Count - 1));
        header.text                 = level.Category == LevelCategory.DAILY ?
                                        level.Date.ToString("M/d/yyyy") + "\nJumblie Complete"
                                        : "Level " + level.LevelNumber.ToString() + " Complete";
        headerDots.text             = new string(UIManager.WORD_LENGTH_INDICATOR_SYMBOL, level.Words.Count).Aggregate(string.Empty, (c, i) => c + i + ' ').TrimEnd();
        headerBG.style.top          = level.Category == LevelCategory.DAILY ?
                                        new StyleLength(-215f)
                                        : new StyleLength(-125f);

        secretWordFound.Show(level.HasSecretWord);
        secretWordFound.text        = level.HasSecretWord ?
                                        (GameManager.instance.SaveData.IsSecretWordFound(level) ?
                                            "Secret Word Found!"
                                            : "Try Finding the Secret Word!")
                                        : "";

        //For Anim In
        guessesCount.text           = "0"; //guessINT.ToString();
        wordsCount.text             = "0"; //wordsINT.ToString();
        newWordsCount.text          = "0"; //newWordsINT.ToString();

        secretWordFound.SetVisibility(false);
        break1.SetVisibility(false);
        break2.SetVisibility(false);
        break3.SetVisibility(false);
        centerPanel.SetVisibility(false);
        categoryTitle.SetVisibility(false);
        categoryDetails.SetVisibility(false);
        buttonContainer.SetVisibility(false);

        secretWordFound.transform
            .scale                  = new Vector3(1.5f, 1.5f, 1f);
        break1.transform.scale      = new Vector3(1.5f, 1.5f, 1f);
        break2.transform.scale      = new Vector3(1.5f, 1.5f, 1f);
        break3.transform.scale      = new Vector3(1.5f, 1.5f, 1f);
        centerPanel.transform.scale = new Vector3(1.5f, 1.5f, 1f);
        categoryTitle.transform
            .scale                  = new Vector3(1.5f, 1.5f, 1f);
        categoryDetails.transform
            .scale                  = new Vector3(1.5f, 1.5f, 1f);
        buttonContainer.transform
            .scale                  = new Vector3(1.5f, 1.5f, 1f);
        ///

        if (level.Category == LevelCategory.DAILY)
        {
            completeCounter.parent.Hide();
            secretCounter.parent.Hide();

            categoryTitle.text      = "00:00";
        }
        else
        {
            categoryTitle.text      = level.Category.Name();
            completeCounter.text    = GameManager.instance.SaveData.LevelCompleteCount(level.Category) + " / " + LevelDefinitions.LevelCount(level.Category);
            secretCounter.text      = GameManager.instance.SaveData.SecretFoundCount(level.Category) + " / " + LevelDefinitions.SecretCount(level.Category);
        }
    }

    private void ReturnToMainMenu()
    {
        if (canClick)
            return;
    }

    #endregion
}