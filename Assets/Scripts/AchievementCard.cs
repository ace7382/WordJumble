using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AchievementCard
{
    #region Private Variables

    private Achievement     achievement;
    private VisualElement   root;
    private bool            isToastCard;

    #endregion

    #region Constructor

    public AchievementCard(VisualElement root, Achievement achievement, bool isToastCard = false)
    {
        this.root           = root;
        this.achievement    = achievement;
        this.isToastCard    = isToastCard;

        SetupUI();
    }

    #endregion

    #region Private Functions

    private void SetupUI()
    {
        Label title             = root.Q<Label>(UIManager.ACHIEVEMENT_CARD__TITLE_NAME);
        Label description       = root.Q<Label>(UIManager.ACHIEVEMENT_CARD__DESCRIPTION_NAME);
        Label barLabel          = root.Q<Label>(UIManager.ACHIEVEMENT_CARD__BAR_LABEL_NAME);

        VisualElement barFill   = root.Q<VisualElement>(UIManager.ACHIEVEMENT_CARD__BAR_FILL_NAME);

        title.text              = achievement.Name;
        description.text        = achievement.GetDescription();

        if (GameManager.instance.SaveData.IsAchievementUnlocked(achievement) && !isToastCard)
        {
            barLabel.text       = "Achievement Unlocked!";
            barFill.style.width = new StyleLength(new Length(100f, LengthUnit.Percent));
        }
        else
        {
            barLabel.text       = achievement.GetProgressString();
            barFill.style.width = new StyleLength(new Length(Mathf.Clamp(achievement.GetProgressPercent(), 4f, 100f), LengthUnit.Percent));
        }
    }
    
    #endregion
}
