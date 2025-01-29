using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    //TODO: remove
    public bool achTest;

    #region Singleton

    public static AchievementManager    instance;

    #endregion

    #region Private Variables

    private AchievementQueue            achQueue;

    #endregion

    #region Inherited Functions

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        SetupListeners();

        achQueue = gameObject.GetComponent<AchievementQueue>();
    }

    public void OnDestroy()
    {
        RemoveListeners();
    }

    //TODO:Remove
    public void Update()
    {
        if (achTest)
        {
            for (int i = 0; i < 5; i++)
            {
                UnlockAchievement(AchievementDefinitions.ALL_ACHIEVEMENTS[i]);
            }

            achTest = false;
        }
    }

    #endregion

    #region Private Functions

    private void SetupListeners()
    {
        this.AddObserver(OnLevelComplete, Notifications.LEVEL_COMPLTED);
        this.AddObserver(OnWordAddedToFoundList, Notifications.WORD_ADDED_TO_FOUND_WORDS);
    }

    private void RemoveListeners()
    {
        this.RemoveObserver(OnLevelComplete, Notifications.LEVEL_COMPLTED);
    }

    private void UnlockAchievement(Achievement ach)
    {
        Debug.Log($"Achievement {ach.ID} unlocked: {ach.Name} - {ach.GetDescription()}");

        GameManager.instance.SaveData.AchievementsUnlocked.Add(ach.ID);
        
        achQueue.AddAchievementToQueue(ach);
    }

    private void OnLevelComplete(object sender, object info)
    {
        //If level is daily
        //Go through list of daily levels
        //if ach is unlocked, skip
        //else check CheckUnlock
        //if true, unlock achievement

        NewLevel levelCompleted = (NewLevel)info;

        if (levelCompleted.Category == LevelCategory.DAILY)
        {
            for (int i = 0; i < AchievementDefinitions.DAILY_COUNT_ACHIEVEMENTS.Count; i++)
            {
                ACH_DailyCount ach = AchievementDefinitions.DAILY_COUNT_ACHIEVEMENTS[i];

                if (GameManager.instance.SaveData.IsAchievementUnlocked(ach))
                    continue;

                if (ach.CheckUnlock())
                    UnlockAchievement(ach);
            }
        }
        else //Category level completed
        {
            for (int i = 0; i < AchievementDefinitions.CATEGORY_COMPLETE_ACHIEVEMENTS.Count; i++)
            {
                ACH_CategoryComplete ach = AchievementDefinitions.CATEGORY_COMPLETE_ACHIEVEMENTS[i];

                if (GameManager.instance.SaveData.IsAchievementUnlocked(ach))
                    continue;

                if (levelCompleted.Category == ach.Category)
                    if (ach.CheckUnlock())
                        UnlockAchievement(ach);
            }
        }
    }

    private void OnWordAddedToFoundList(object sender, object info)
    {
        for (int i = 0; i < AchievementDefinitions.WORD_GROUP_ACHIEVEMENTS.Count; i++)
        {
            ACH_WordGroup ach = AchievementDefinitions.WORD_GROUP_ACHIEVEMENTS[i];

            if (GameManager.instance.SaveData.IsAchievementUnlocked(ach))
                continue;

            if (ach.CheckUnlock())
                UnlockAchievement(ach);
        }

        for (int i = 0; i < AchievementDefinitions.SPECIFIC_WORD_ACHIEVEMENTS.Count; i++)
        {
            ACH_SpecificWord ach = AchievementDefinitions.SPECIFIC_WORD_ACHIEVEMENTS[i];

            if (GameManager.instance.SaveData.IsAchievementUnlocked(ach))
                continue;

            if (ach.CheckUnlock())
                UnlockAchievement(ach);
        }
    }

    #endregion
}
