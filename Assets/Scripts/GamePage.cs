using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System;

public class GamePage : Page
{
    #region Private Variables

    private Level                   currentLevel;

    private VisualElement           tileContainer;
    private VisualElement           foundWordContainer;
    private Label                   submittedWord;
    private Label                   themeLabel;
    private Button                  submitButton;
    private Button                  shuffleButton;
    private Button                  clearButton;
    private Button                  timer;
    private Button                  exitButton;
    private Button                  hideFoundButton;

    private List<Tile>              letterTiles;
    private List<Tile>              selectedTiles;
    private List<string>            incorrectGuesses    = new List<string>();

    private List<VisualElement>     solutionWordBadges;
    private VisualElement           secretWordBadge;

    private bool                    canClick;

    private bool                    dailyJumblie;

    private bool                    showFoundTiles      = false;

    private bool                    paused              = false;

    private Action                  action_MainMenu     = delegate { PageManager.instance.StartCoroutine(PageManager.instance.OpenPageOnAnEmptyStack<MainMenuPage>()); };
    private Action                  action_CloseOverlay = delegate { PageManager.instance.StartCoroutine(PageManager.instance.CloseTopPage()); };

    #endregion

    #region Public Properties

    public string           CurrentWord { get { return submittedWord.text.ToUpper(); } }
    public string           SecretWord  { get { return currentLevel.SecretWord.ToUpper(); } }

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
        Debug.Log("Game Page Hiding");

        GameManager.instance.StopTimer();

        RemoveListeners();
    }

    public override void ShowPage(object[] args)
    {
        //args[0]   -   bool    -   True = Daily Puzzle
        //args[1]   -   Level   -   The level to load

        dailyJumblie    = (bool)args[0];
        currentLevel    = (Level)args[1];

        SetupUI();
        AddListeners();
    }

    public override void OnFocusReturnedToPage()
    {
        canClick        = true;

        Pause();
    }

    #endregion

    #region Private Functions

    private void SetupUI()
    {
        tileContainer       = uiDoc.rootVisualElement.Q(UIManager.GAME_PAGE__TILE_CONTAINER_NAME);
        foundWordContainer  = uiDoc.rootVisualElement.Q(UIManager.GAME_PAGE__FOUND_WORD_CONTAINER_NAME);

        submittedWord       = uiDoc.rootVisualElement.Q<Label>(UIManager.GAME_PAGE__SUBMITTED_WORD_NAME);
        themeLabel          = uiDoc.rootVisualElement.Q<Label>(UIManager.GAME_PAGE__THEME_LABEL_NAME);

        submitButton        = uiDoc.rootVisualElement.Q<Button>(UIManager.GAME_PAGE__SUBMIT_BUTTON_NAME);
        shuffleButton       = uiDoc.rootVisualElement.Q<Button>(UIManager.GAME_PAGE__SHUFFLE_BUTTON_NAME);
        clearButton         = uiDoc.rootVisualElement.Q<Button>(UIManager.GAME_PAGE__CLEAR_BUTTON_NAME);
        exitButton          = uiDoc.rootVisualElement.Q<Button>(UIManager.GAME_PAGE__EXIT_BUTTON_NAME);
        timer               = uiDoc.rootVisualElement.Q<Button>(UIManager.GAME_PAGE__TIMER_NAME);
        hideFoundButton     = uiDoc.rootVisualElement.Q<Button>(UIManager.GAME_PAGE__HIDE_FOUND_BUTTON_NAME);

        letterTiles         = new List<Tile>();
        selectedTiles       = new List<Tile>();

        solutionWordBadges  = new List<VisualElement>();

        for (int i = 0; i < currentLevel.Words.Count; i++)
        {
            bool found = currentLevel.FoundWords[i];

            foreach (char letter in currentLevel.Words[i])
            {
                VisualElement tileVE    = UIManager.instance.LetterTile.Instantiate();
                Tile tile               = new Tile(tileVE, letter.ToString().ToUpper(), found);
                tileVE.userData         = tile;

                tileContainer.Add(tileVE);

                if (found)
                {
                    tile.SetColor(UIManager.instance.GetColor(i));
                    tile.Show(false);
                }

                letterTiles.Add(tile);
            }

            VisualElement badge     = UIManager.instance.SolvedWordTile.Instantiate();
            badge.userData          = currentLevel.Words[i];

            badge.ElementAt(0).style
                .backgroundColor    = UIManager.instance.GetColor(i);

            badge.Q<Label>(UIManager.SOLVED_WORD__WORD_NAME)
                .text               = currentLevel.Words[i].ToUpper();

            badge.Q<Label>(UIManager.SOVLED_WORD__LENGTH_INDICATOR_NAME)
                .text               = new string(UIManager.WORD_LENGTH_INDICATOR_SYMBOL, i + 1).Aggregate(string.Empty, (c, i) => c + i + ' ').TrimEnd();

            badge.Show(currentLevel.FoundWords[i]);

            foundWordContainer.Add(badge);
            solutionWordBadges.Add(badge);
        }

        if (!dailyJumblie)
        {
            //Secret Word Badge
            secretWordBadge         = UIManager.instance.SolvedWordTile.Instantiate();

            secretWordBadge.ElementAt(0).style
                .backgroundColor    = UIManager.instance.GetColor(-1);

            secretWordBadge.Q<Label>(UIManager.SOLVED_WORD__WORD_NAME)
                .text               = SecretWord;

            secretWordBadge.Q<Label>(UIManager.SOVLED_WORD__LENGTH_INDICATOR_NAME)
                .text               = UIManager.WORD_LENGTH_INDICATOR_SYMBOL.ToString() + " Secret Word " + UIManager.WORD_LENGTH_INDICATOR_SYMBOL.ToString();

            secretWordBadge.Show(currentLevel.SecretWordFound);
            foundWordContainer.Add(secretWordBadge);
            ////////

            timer.Hide();
        }

        submitButton.clicked    += () => SubmitWord();
        shuffleButton.clicked   += () => Shuffle();
        clearButton.clicked     += () => ClearAll();
        exitButton.clicked      += () => ExitLevel();
        hideFoundButton.clicked += () => ShowHideFoundTiles();

        themeLabel.text         = currentLevel.Theme;

        Shuffle();

        if (dailyJumblie)
            GameManager.instance.SetTime(timer);
    }

    private void AddListeners()
    {
        this.AddObserver(AddTile, Notifications.LETTER_SELECTED);
        this.AddObserver(RemoveTile, Notifications.LETTER_UNSELECTED);
    }

    private void RemoveListeners()
    {
        this.RemoveObserver(AddTile, Notifications.LETTER_SELECTED);
        this.RemoveObserver(RemoveTile, Notifications.LETTER_UNSELECTED);
    }

    private void AddTile(object sender, object info)
    {
        Tile t = (Tile)sender;

        selectedTiles.Add(t);

        UpdateWord();
    }

    private void RemoveTile(object sender, object info)
    {
        Tile t = (Tile)sender;

        selectedTiles.Remove(t);

        UpdateWord();
    }

    private void UpdateWord()
    {
        string final = "";

        foreach (Tile t in selectedTiles)
            final += t.Letter;

        submittedWord.text = final;
    }

    private void SubmitWord()
    {
        int index = -1;

        for (int i = 0; i < currentLevel.Words.Count; i++)
        {
            if (currentLevel.Words[i].Equals(CurrentWord, StringComparison.OrdinalIgnoreCase))
            {
                index = i;
                break;
            }    
        }

        if (index > -1 && !currentLevel.FoundWords[index])
        {
            foreach (Tile t in selectedTiles)
            {
                if (t.Found) //if a tile is used from show tiles
                {
                    Tile newT = letterTiles.Find(x => !x.Found && x.Letter == t.Letter);

                    newT.Found = true;
                    newT.SetColor(UIManager.instance.GetColor(index));
                    newT.Show(showFoundTiles);
                }
                else
                {
                    t.Found = true;
                    t.SetColor(UIManager.instance.GetColor(index));
                    t.Show(showFoundTiles);
                }
            }

            foreach (VisualElement badge in solutionWordBadges)
            {
                if (CurrentWord.Equals((string)badge.userData, StringComparison.OrdinalIgnoreCase))
                {
                    badge.Show();
                    break;
                }
            }

            currentLevel.WordFound(index);
        }
        else 
        {
            if (CurrentWord.Equals(SecretWord, StringComparison.OrdinalIgnoreCase))
                SecretWordFound();
            else
                incorrectGuesses.Add(CurrentWord);
        }

        ClearAll();
        FinishLevel();
    }

    private void ClearAll()
    {
        foreach (Tile t in selectedTiles)
            t.ForceUnselect();

        selectedTiles.Clear();
        UpdateWord();
    }

    private void Shuffle()
    {
        VisualElement[] tiles = tileContainer.Children().ToArray();

        Utilities.Shuffle(tiles);

        foreach (VisualElement child in tiles)
        {
            child.RemoveFromHierarchy();

            tileContainer.Add(child);
        }
    }

    private void SecretWordFound()
    {
        if (currentLevel.SecretWordFound)
            return;

        currentLevel.SecretWordFound = true;

        secretWordBadge.Show();

        FinishLevel();
    }

    private void ExitLevel()
    {
        if (!canClick)
            return;

        Pause();

        canClick        = false;

        object[] args   = new object[5];
        args[0]         = "Are you sure you would like to exit the level?\n\nFound words will be saved";
        args[1]         = "Exit";
        args[2]         = "Play On";
        args[3]         = action_MainMenu;
        args[4]         = action_CloseOverlay;

        PageManager.instance.StartCoroutine(PageManager.instance.AddPageToStack<GameOverlayPage>(args));
    }

    private void Pause()
    {
        paused = !paused;

        Debug.Log(paused ? "Pausing Game" : "Unpausing Game");

        tileContainer.SetVisibility(!paused);
        foundWordContainer.SetVisibility(!paused);

        if (dailyJumblie)
        {
            if (paused)
                GameManager.instance.PauseTimer();
            else
                GameManager.instance.SetTime(timer, false);
        }
    }

    private void ShowHideFoundTiles()
    {
        showFoundTiles = !showFoundTiles;

        hideFoundButton.text = (showFoundTiles ? "Hide" : "Show") + " Found";

        foreach (Tile t in letterTiles)
            if (t.Found)
                t.Show(showFoundTiles);
    }

    private void FinishLevel()
    {
        if (currentLevel.FoundWords.FindIndex(x => !x) > -1)
            return;

        currentLevel.Complete = true;

        canClick        = false;

        Pause();

        object[] args   = new object[7];
        Level nextLevel = dailyJumblie ? null : GameManager.instance.GetNextLevel(currentLevel);

        args[0]         = dailyJumblie ? "Daily Jumblie Complete!\n" + timer.text
                                        : (nextLevel == null ? "All Levels Complete!" : "Level Complete");
        args[1]         = "Exit";
        args[2]         = nextLevel == null ? null : "Next Level";
        args[3]         = action_MainMenu;

        Action openNext = delegate {
                            object[] args   = new object[2];
                            args[0]         = false;
                            args[1]         = nextLevel;

                            PageManager.instance.StartCoroutine(PageManager.instance.OpenPageOnAnEmptyStack<GamePage>(args));
                        };

        args[4]         = openNext;

        if (!dailyJumblie && !currentLevel.SecretWordFound)
        {
            args[5]     = "Find Secret";
            args[6]     = action_CloseOverlay;
        }
        else
        {
            args[5]     = null;
            args[6]     = null;
        }

        PageManager.instance.StartCoroutine(PageManager.instance.AddPageToStack<GameOverlayPage>(args));
    }

    #endregion
}