using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AchievementsPage : Page
{
    #region Private Variables

    private bool            canClick;

    private VisualElement   completeCounter;
    private VisualElement   backButton;
    private VisualElement   hideCompletedButton;
    private VisualElement   cardContainer;

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
        completeCounter             = uiDoc.rootVisualElement.Q<VisualElement>(UIManager.ACHIEVE_PAGE__COMPLETE_COUNTER_NAME);
        backButton                  = uiDoc.rootVisualElement.Q<VisualElement>(UIManager.ACHIEVE_PAGE__BACK_BUTTON_NAME);
        hideCompletedButton         = uiDoc.rootVisualElement.Q<VisualElement>(UIManager.ACHIEVE_PAGE__HIDE_COMP_BUTTON_NAME);
        cardContainer               = uiDoc.rootVisualElement.Q<VisualElement>(UIManager.ACHIEVE_PAGE__CARD_CONTAINER_NAME);

        List<Achievement> achs      = AchievementDefinitions.ALL_ACHIEVEMENTS;

        for (int i = 0; i < achs.Count; i++)
        {
            VisualElement cardVE    = UIManager.instance.AchievementCard.Instantiate();
            AchievementCard control = new AchievementCard(cardVE, achs[i]);

            cardContainer.Add(cardVE);
        }

        RegisterCallbacksAndEvents();

        canClick = true;
    }

    private void RegisterCallbacksAndEvents()
    {
        QuickButton backButtonQB    = new QuickButton(backButton, Color.black);
        backButton.AddManipulator(backButtonQB);

        backButton.RegisterCallback<ClickEvent>(_ => ReturnToMainMenu());
    }

    private void ReturnToMainMenu()
    {
        if (!canClick)
            return;

        canClick = false;

        PageManager.instance.StartCoroutine(PageManager.instance.OpenPageOnAnEmptyStack<MainMenuPage>());
    }

    #endregion
}
