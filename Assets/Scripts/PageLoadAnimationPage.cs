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

    private const float         MIN_BADGE_WIDTH = 300f;
    private const float         ROW_HEIGHT      = 150f;

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

        PageManager.instance.CloseAllPagesUnderTop();

        var method  = typeof(PageManager).GetMethod("AddPageUnderTopPage");
        var refe    = method.MakeGenericMethod(pageTypeToLoad);

        refe.Invoke(PageManager.instance, new object[] { newPageArguments, true } );

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
        VisualElement page      = uiDoc.rootVisualElement.Q<VisualElement>("Page");
        System.Random rIcon     = new System.Random();

        int badgesInRow         = Mathf.CeilToInt(Screen.width / MIN_BADGE_WIDTH) + 5;
        int rowsNeeded          = Mathf.CeilToInt(Screen.height / ROW_HEIGHT) + 5;

        wordBadges              = new VisualElement[rowsNeeded, badgesInRow];

        Stack<VisualElement>
            preloadedBadges     = new Stack<VisualElement>(page.Children());

        for (int i = 0; i < rowsNeeded; i++)
        {
            VisualElement row   = new VisualElement();
            row.AddToClassList(UIManager.GLOBAL_STYLE__LOADING_ROW_CLASS);

            for (int j = 0; j < badgesInRow; j++)
            {
                wordBadges[i, j]    = preloadedBadges.Count > 0 ?
                                        preloadedBadges.Pop()
                                        : UIManager.instance.SolvedWordTile.Instantiate();

                wordBadges[i, j].Show();
                wordBadges[i, j].Q<Label>("Word").text = GameManager.instance.GetWordFromList().ToUpper();

                int lengthVal = rIcon.Next(0, UIManager.instance.WordColorMax);

                wordBadges[i, j].Q<Label>("LengthCounter").text = new string(UIManager.WORD_LENGTH_INDICATOR_SYMBOL, lengthVal + 1).Aggregate(string.Empty, (c, i) => c + i + ' ').TrimEnd();
                wordBadges[i, j].style.flexGrow = 1f;
                wordBadges[i, j].style.flexShrink = 0f;

                VisualElement badgeContainer = wordBadges[i, j].Q<VisualElement>("Container");
                badgeContainer.SetColor(UIManager.instance.GetColor(lengthVal));
                badgeContainer.style.flexGrow = 1f;

                wordBadges[i, j].ElementAt(0).SetMargins(0f);
                wordBadges[i, j].SetVisibility(false);
                wordBadges[i, j].transform.scale = Vector3.zero;

                row.Add(wordBadges[i, j]);
            }

            page.Add(row);
        }

        if (wordBadges.Length < 25)
        {
            for (int i = 24; i >= wordBadges.Length; i--)
                page.ElementAt(i).Hide();
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
