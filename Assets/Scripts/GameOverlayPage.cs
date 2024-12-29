using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameOverlayPage : Page
{
    #region Private Variables

    private bool    canClick;

    private Label   messageLabel;
    private Button  button1;
    private Button  button2;
    private Button  button3;
    private Button  button4;

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
        RemoveListeners();
    }

    public override void ShowPage(object[] args)
    {
        //args[0]   -   string          -   The text that should display in th modal
        //args[1]   -   string          -   Button1 text
        //args[2]   -   string          -   Button2 text, if blank, button will be hidden
        //args[3]   -   System.Action   -   The action for button1
        //args[4]   -   System.Action   -   The action for button2
        //args[5]   -   string          -   Button3 text, if blank, button will be hidden
        //args[6]   -   System.Action   -   The action for button3

        SetupUI(
            (string)args[0]
            , (string)args[1]
            , (string)args[2]
            , (System.Action)args[3]
            , (System.Action)args[4]
            , (string)args[5]
            , (System.Action)args[6]
        );

        AddListeners();
    }

    #endregion

    #region Private Functions

    private void SetupUI(string message, string button1Label, string button2Label
                        , System.Action button1Action, System.Action button2Action = null
                        , string button3Label = "", System.Action button3Action = null
                        , string button4Label = "", System.Action button4Action = null)
    {
        messageLabel        = uiDoc.rootVisualElement.Q<Label>(UIManager.GAME_OVERLAY_PAGE__MESSAGE_NAME);
        button1             = uiDoc.rootVisualElement.Q<Button>(UIManager.GAME_OVERLAY_PAGE__BUTTON_1_NAME);
        button2             = uiDoc.rootVisualElement.Q<Button>(UIManager.GAME_OVERLAY_PAGE__BUTTON_2_NAME);
        button3             = uiDoc.rootVisualElement.Q<Button>(UIManager.GAME_OVERLAY_PAGE__BUTTON_3_NAME);
        button4             = uiDoc.rootVisualElement.Q<Button>(UIManager.GAME_OVERLAY_PAGE__BUTTON_4_NAME);

        messageLabel.text   = message;
        button1.text        = button1Label;

        button1.clicked     += () => button1Action.Invoke();

        if (string.IsNullOrEmpty(button2Label))
            button2.Hide();
        else
        {
            button2.text    = button2Label;
            button2.clicked += () => button2Action.Invoke();
        }

        if (string.IsNullOrEmpty(button3Label))
            button3.Hide();
        else
        {
            button3.text = button3Label;
            button3.clicked += () => button3Action.Invoke();
        }

        if (string.IsNullOrEmpty(button4Label))
            button4.Hide();
        else
        {
            button4.text = button4Label;
            button4.clicked += () => button4Action.Invoke();
        }        
    }

    private void AddListeners()
    { }

    private void RemoveListeners()
    { }

    #endregion
}
