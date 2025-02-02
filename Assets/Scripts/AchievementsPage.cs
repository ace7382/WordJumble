using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AchievementsPage : Page
{
    #region Private Variables

    private bool            canClick;

    private Label           completeCounter;
    private Label           hideCompletedButton;
    private Label           allCompleteMessage;

    private VisualElement   backButton;
    private VisualElement   cardContainer;

    private bool            showCompleted           = true;

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

    }

    public override void ShowPage(object[] args)
    {
        SetupUI();
    }

    #endregion

    #region Private Functions

    private void SetupUI()
    {
        completeCounter             = uiDoc.rootVisualElement.Q<Label>(UIManager.ACHIEVE_PAGE__COMPLETE_COUNTER_NAME);
        backButton                  = uiDoc.rootVisualElement.Q<VisualElement>(UIManager.ACHIEVE_PAGE__BACK_BUTTON_NAME);
        hideCompletedButton         = uiDoc.rootVisualElement.Q<Label>(UIManager.ACHIEVE_PAGE__HIDE_COMP_BUTTON_NAME);
        cardContainer               = uiDoc.rootVisualElement.Q<VisualElement>(UIManager.ACHIEVE_PAGE__CARD_CONTAINER_NAME);
        allCompleteMessage          = uiDoc.rootVisualElement.Q<Label>(UIManager.ACHIEVE_PAGE__ALL_COMPLETE_TEXT_NAME);

        List<Achievement> achs      = AchievementDefinitions.ALL_ACHIEVEMENTS;

        for (int i = 0; i < achs.Count; i++)
        {
            VisualElement cardVE    = UIManager.instance.AchievementCard.Instantiate();
            AchievementCard control = new AchievementCard(cardVE, achs[i]);

            //cardVE.userData         = control;
            cardVE.userData         = achs[i];

            cardContainer.Add(cardVE); 
        }

        completeCounter.text        = $"{AchievementManager.instance.CompletedAchievementCount} / {achs.Count}";

        RegisterCallbacksAndEvents();

        canClick = true;
    }

    private void RegisterCallbacksAndEvents()
    {
        QuickButton backButtonQB    = new QuickButton(backButton, Color.black);
        backButton.AddManipulator(backButtonQB);

        backButton.RegisterCallback<ClickEvent>(_ => ReturnToMainMenu());

        QuickButton hideCompletedQB = new QuickButton(hideCompletedButton, Color.black);
        hideCompletedButton.AddManipulator(hideCompletedQB);

        hideCompletedButton.RegisterCallback<ClickEvent>(_ => ShowHideCompleted());
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

        showCompleted       = !showCompleted;

        bool showAllEarned  = !showCompleted;

        foreach (VisualElement child in cardContainer.Children())
        {
            if (child == allCompleteMessage)
                continue;

            Achievement ach = (Achievement)child.userData;

            child.Show(showCompleted || ach.IsUnlocked == showCompleted);

            Debug.Log($"Ach {ach.ID} is showing {child.IsShowing()}");
            if (child.IsShowing())
                showAllEarned = false;
        }

        hideCompletedButton.text = showCompleted ? "Hide Completed" : "Show Completed";

        allCompleteMessage.Show(showAllEarned);
    }

    #endregion
}
