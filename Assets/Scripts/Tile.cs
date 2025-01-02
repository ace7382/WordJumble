using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Tile
{
    #region Private Variables

    private string          letter;

    private VisualElement   root;
    private Button          button;
    private Label           letterLabel;

    private bool            found;

    #endregion

    #region Properties

    public string Letter {
        get { return letter; }
        private set { letterLabel.text = value; letter = value; }
    }

    public bool Found {
        get { return found; }
        set { found = value; }
    }

    public VisualElement Root { get { return root; } }

    #endregion

    #region Constructor

    public Tile(VisualElement root, string letter, bool found)
    {
        this.root       = root;
        button          = root.Q<Button>();
        letterLabel     = button.Q<Label>(UIManager.LETTER_TILE__LETTER_LABEL_NAME);

        Letter          = letter;
        Found           = found;

        button.clicked  += () => Select();
    }

    #endregion

    #region Public Functions

    public void Show(bool s)
    {
        button.Show(s);
    }

    public void ForceUnselect()
    {
        button.RemoveFromClassList(UIManager.GLOBAL_STYLE__TILE_SELECTED_CLASS);
        letterLabel.RemoveFromClassList(UIManager.GLOBAL_STYLE__TILE_SELECTED_LETTER_CLASS);
    }

    public void SetColor(Color c)
    {
        button.style.backgroundColor = c;
    }

    #endregion

    #region Private Functions

    private void Select()
    {
        if (button.ClassListContains(UIManager.GLOBAL_STYLE__TILE_SELECTED_CLASS))
        {
            button.RemoveFromClassList(UIManager.GLOBAL_STYLE__TILE_SELECTED_CLASS);
            letterLabel.RemoveFromClassList(UIManager.GLOBAL_STYLE__TILE_SELECTED_LETTER_CLASS);

            this.PostNotification(Notifications.LETTER_UNSELECTED);
        }
        else
        {
            button.AddToClassList(UIManager.GLOBAL_STYLE__TILE_SELECTED_CLASS);
            letterLabel.AddToClassList(UIManager.GLOBAL_STYLE__TILE_SELECTED_LETTER_CLASS);

            this.PostNotification(Notifications.LETTER_SELECTED);
        }
    }

    #endregion
}
