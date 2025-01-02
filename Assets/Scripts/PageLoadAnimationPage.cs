using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using DG.Tweening;
using System;

public class PageLoadAnimationPage : Page
{
    #region Private Variables

    private const int           ROWS            = 20;
    private const int           WORDS_IN_ROW    = 7;

    private string[]            TEST_WORDS      = {"Briar", "Bucket", "Anvil", "Corsair", "Fish", "Smelt", "Abacus", "Another", "Prismatic"
                                                    , "Alligators", "Janitorial", "Musk", "Turtle", "Bunny", "Jump", "Jack", "Atom", "Bombastic" };

    private VisualElement[,]    wordBadges;

    private Type                pageTypeToLoad;
    private object[]            newPageArguments;

    #endregion

    #region Inherited Functions

    public override IEnumerator AnimateIn()
    {
        int min             = 0;
        int max             = wordBadges.GetLength(1) - 1;

        int currentLeft     = min;
        int currentRight    = max;

        while (currentLeft <= max && currentRight >= min)
        {
            Sequence s = DOTween.Sequence();

            for (int i = 0; i < wordBadges.GetLength(0); i++)
            {
                VisualElement b = wordBadges[i, i % 2 == 0 ? currentLeft : currentRight];
                b.SetVisibility(true);

                Tween t = DOTween.To
                        (
                            () => b.transform.scale,
                            x => b.transform.scale = x,
                            new Vector3(1f, 1f, 1f),
                            .25f
                        )
                        .SetEase(Ease.InOutBack, 2f)
                        .SetLoops(1);

                s.Join(t);
            }

            currentLeft++;
            currentRight--;

            yield return s.Play().WaitForCompletion();
        }

        yield return new WaitForSeconds(.7f);

        //////
        //Level dailyLevel;

        //string path = "Levels/Daily/"
        //        + System.DateTime.Now.Year.ToString() + "/"
        //        + System.DateTime.Now.Month.ToString() + "/"
        //        + System.DateTime.Now.Day.ToString();

        //Debug.Log("Daily Level Path: " + path);

        //dailyLevel = Resources.Load<Level>(path);

        //object[] args = new object[2];
        //args[0] = true;
        //args[1] = dailyLevel;
        //////
        ///

        PageManager.instance.CloseAllPagesUnderTop();

        var method  = typeof(PageManager).GetMethod("AddPageUnderTopPage");
        var refe    = method.MakeGenericMethod(pageTypeToLoad);

        refe.Invoke(PageManager.instance, new object[] { newPageArguments, true } );

        //PageManager.instance.AddPageUnderTopPage<GamePage>(args);

        PageManager.instance.StartCoroutine(PageManager.instance.CloseTopPage());
    }

    public override IEnumerator AnimateOut()
    {
        int min             = 0;
        int max             = wordBadges.GetLength(1) - 1;

        int currentLeft     = min;
        int currentRight    = max;

        while (currentLeft <= max && currentRight >= min)
        {
            Sequence s = DOTween.Sequence();

            for (int i = 0; i < wordBadges.GetLength(0); i++)
            {
                //wordBadges[i, i % 2 == 0 ? currentLeft : currentRight].SetVisibility(false);

                VisualElement b = wordBadges[i, i % 2 == 0 ? currentLeft : currentRight];


                Tween t = DOTween.To
                        (
                            () => b.transform.scale,
                            x => b.transform.scale = x,
                            new Vector3(0f, 0f, 0f),
                            .2f
                        )
                        .SetEase(Ease.Linear)
                        .SetLoops(1)
                        .OnComplete(() => b.SetVisibility(false));

                s.Join(t);
            }

            currentLeft++;
            currentRight--;

            yield return s.Play().WaitForCompletion();
        }
    }

    public override void HidePage()
    {
        UnregisterCallbacksAndEvents();
    }

    public override void ShowPage(object[] args)
    {
        //args[0]   -   Type        -   The Type of Page to load after the animation page
        //args[1]   -   object[]    -   The arguments to pass to the new page

        pageTypeToLoad      = (Type)args[0];
        newPageArguments    = (object[])args[1];

        SetupUI();
        RegisterCallbacksAndEvents();
    }

    #endregion

    #region Private Functions

    private void SetupUI()
    {
        VisualElement page  = uiDoc.rootVisualElement.Q<VisualElement>("Page");
        System.Random rWord = new System.Random();
        System.Random rIcon = new System.Random();

        wordBadges = new VisualElement[ROWS, WORDS_IN_ROW];

        for (int i = 0; i < wordBadges.GetLength(0); i++)
        {
            for (int j = 0; j < wordBadges.GetLength(1); j++)
            {
                wordBadges[i, j] = page.ElementAt(i).ElementAt(j);

                wordBadges[i, j].Q<Label>("Word").text = TEST_WORDS[rWord.Next(0, TEST_WORDS.Length)].ToUpper();

                int lengthVal = rIcon.Next(0, UIManager.instance.WordColorMax);

                wordBadges[i, j].Q<Label>("LengthCounter").text = new string(UIManager.WORD_LENGTH_INDICATOR_SYMBOL, lengthVal + 1).Aggregate(string.Empty, (c, i) => c + i + ' ').TrimEnd();

                VisualElement badgeContainer = wordBadges[i, j].Q<VisualElement>("Container");
                badgeContainer.SetColor(UIManager.instance.GetColor(lengthVal));
                badgeContainer.style.flexGrow = 1f;

                wordBadges[i, j].ElementAt(0).SetMargins(1f);
                wordBadges[i, j].SetVisibility(false);
                wordBadges[i, j].transform.scale = Vector3.zero;
            }
        }


    }

    private void RegisterCallbacksAndEvents()
    {

    }

    private void UnregisterCallbacksAndEvents()
    {

    }

    #endregion
}
