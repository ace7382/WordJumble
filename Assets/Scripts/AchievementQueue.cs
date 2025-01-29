using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class AchievementQueue : MonoBehaviour
{
    #region Private Variables

    private Queue<Achievement>  achQueue;

    private VisualElement       cardRoot;
    private AchievementCard     cardController;

    private UIDocument          uiDoc;

    private IEnumerator         currentAnimationCoroutine;

    #endregion

    #region Inherited Functions

    private void Awake()
    {
        uiDoc           = gameObject.GetComponent<UIDocument>();
        cardRoot        = uiDoc.rootVisualElement.Q<VisualElement>("Card");
        achQueue        = new Queue<Achievement>();

        currentAnimationCoroutine = null;
    }

    private void Start()
    {
        cardRoot.style.translate =  new StyleTranslate(
                                        new Translate(
                                            new Length(0f)
                                            , new Length(-300f)
                                        )
                                    );

        cardRoot.SetBorderWidth(10f);
        cardRoot.SetBorderColor(Color.black);
        cardRoot.SetColor(new Color(1f, 1f, .39f));
    }

    private void Update()
    {
        if (achQueue.Count > 0 && currentAnimationCoroutine == null)
        {
            DisplayNext();
        }
    }

    #endregion

    #region Public Functions

    public void AddAchievementToQueue(Achievement ach)
    {
        achQueue.Enqueue(ach);
    }

    #endregion

    #region Private Functions

    private void SetAchievementCard(Achievement ach)
    {
        cardController = new AchievementCard(cardRoot, ach, true);
    }

    private void DisplayNext()
    {
        Achievement ach = achQueue.Dequeue();
        SetAchievementCard(ach);

        currentAnimationCoroutine = AnimateCard();
        StartCoroutine(currentAnimationCoroutine);
    }

    private IEnumerator AnimateCard()
    {
        Length zeroX                = new Length(0f);
        float animTime              = .5f;
        WaitForSeconds waitToRead   = new WaitForSeconds(4f);
        WaitForSeconds waitForNext  = new WaitForSeconds(1.5f);

        Tween slideIn = DOTween.To(
            () => cardRoot.style.translate.value.y.value
            , x => cardRoot.style.translate = new Translate(zeroX, new Length(x))
            , 0f
            , animTime
        ).SetEase(Ease.OutSine);

        yield return slideIn.Play().WaitForCompletion();

        yield return waitToRead;

        Tween slideOut = DOTween.To(
            () => cardRoot.style.translate.value.y.value
            , x => cardRoot.style.translate = new Translate(zeroX, new Length(x))
            , -300f
            , animTime
        ).SetEase(Ease.OutSine);

        yield return slideOut.Play().WaitForCompletion();

        yield return waitForNext;

        currentAnimationCoroutine = null;
    }

    #endregion
}
