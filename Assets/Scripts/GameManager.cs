using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager   instance;

    #endregion

    #region Inspector Variables

    #endregion

    #region Private Variables

    private float           time;
    private Button          timerButton;
    private bool            paused          = false;
    private LevelCategory   currentCategory;

    #endregion

    #region Public Properties

    public LevelCategory    CurrentLevelCategory { get { return currentCategory; } set { currentCategory = value; } }

    #endregion

    #region Inherited Functions

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    public void Update()
    {
        if (timerButton == null || paused)
            return;

        time += Time.deltaTime;

        var minutes         = time / 60;
        var seconds         = time % 60;
        //var fraction        = (time * 100) % 100;
        //timerButton.text    = string.Format("{0:00} : {1:00} : {2:000}", minutes, seconds, fraction);
        timerButton.text    = string.Format("{0:00} : {1:00}", minutes, seconds);
    }

    #endregion

    #region Public Functions

    public void SetTime(Button ve, bool resetTime = false)
    {
        Debug.Log("Setting Time: " + resetTime);

        if (resetTime)
            time    = 0f;

        timerButton = ve;
        paused      = false;
    }

    public void StopTimer()
    {
        Debug.Log("Stopping timer");

        time        = 0f;
        timerButton = null;
        paused      = true;
    }

    public void PauseTimer()
    {
        paused      = true;
    }

    public Level GetNextLevel(Level currentLevel)
    {  
        List<Level> catLevels = Resources.LoadAll<Level>("Levels/" + GameManager.instance.CurrentLevelCategory.ToString()).ToList();

        int nextIndex = catLevels.FindIndex(x => x.LevelNumber == currentLevel.LevelNumber + 1);

        if (nextIndex == -1)
            return null;
        else
            return catLevels[nextIndex];
    }

    #endregion
}
