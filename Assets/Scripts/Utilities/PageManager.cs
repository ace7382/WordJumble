using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PageManager : MonoBehaviour
{
    #region Singleton

    public static PageManager                       instance;

    #endregion

    #region Inspector Variables

    [SerializeField] private GameObject             blankPagePrefab;

    [Space]

    [SerializeField] private List<VisualTreeAsset>  templates;

    #endregion

    #region Private Variables

    private SimplePool<GameObject>                  GOPages;
    private OrderedDictionary                       stack; //Key: Page, Value: GameObject

    private Dictionary<Label, string>               asyncUpdates;

    #endregion

    #region Public Properties

    public int                                      HighestSortOrder    { get { return stack.Count; } }

    #endregion

    #region Unity Functions

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        stack           = new OrderedDictionary();

        GOPages         = new SimplePool<GameObject>(blankPagePrefab);
        GOPages.OnPush  = (item) => { item.SetActive(false); };
        GOPages.Populate(1);

        asyncUpdates    = new Dictionary<Label, string>();
    }

    private void Start()
    {
        StartCoroutine(OpenPageOnAnEmptyStack<MainMenuPage>());
    }

    private void Update()
    {
        //This is needed because async functions cannot update the UI since they are not on the main thread
        foreach (KeyValuePair<Label, string> pair in asyncUpdates)
        {
            if (pair.Key.text != pair.Value)
                pair.Key.text = pair.Value;
        }
    }

    #endregion

    #region Public Functions

    public void RegisterAsyncUpdater(Label l)
    {
        if (asyncUpdates.ContainsKey(l))
            return;

        asyncUpdates.Add(l, l.text);
    }

    public void ChangeAsyncUpdater(Label l, string newText)
    {
        if (!asyncUpdates.ContainsKey(l))
            return;

        asyncUpdates[l] = newText;
    }

    public IEnumerator CloseTopPage(bool animateOut = true, bool executeHideCall = true)
    {
        Page outgoingPage = stack.Cast<DictionaryEntry>().ElementAt(stack.Count - 1).Key as Page;

        //if (animateOut)
        //    yield return (stack.Cast<DictionaryEntry>().ElementAt(stack.Count - 1).Key as Page).AnimateOut();

        //if (executeHideCall)
        //    (stack.Cast<DictionaryEntry>().ElementAt(stack.Count - 1).Key as Page).HidePage();

        if (animateOut)
            yield return outgoingPage.AnimateOut();

        if (executeHideCall)
            outgoingPage.HidePage();

        GOPages.Push(stack[stack.Count - 1] as GameObject);
        stack.RemoveAt(stack.Count - 1);

        (stack.Cast<DictionaryEntry>().ElementAt(stack.Count -1).Key as Page).OnFocusReturnedToPage();
    }

    public IEnumerator OpenPageOnAnEmptyStack<T>(object[] arfs = null, bool animateOut = true
        , bool executeHideCalls = true, bool animateIn = true, bool executeShowCall = true) where T : Page, new()
    {
        for (int i = stack.Count - 1; i >= 0; i--)
        {
            Page p = stack.Cast<DictionaryEntry>().ElementAt(i).Key as Page;

            if (animateOut)
                yield return p.AnimateOut();

            if (executeHideCalls)
                p.HidePage();

            GOPages.Push((GameObject)stack[i]);

            stack.RemoveAt(i);
        }

        GameObject page             = GOPages.Pop();
        page.transform.localScale   = Vector3.one;

        T pageToAdd                 = new T();
        UIDocument uIDocument       = page.GetComponent<UIDocument>();

        string templateName         = typeof(T).ToString().Split("`")[0].Trim(); //TODO: Maybe handle this better~

        uIDocument.visualTreeAsset  = templates.Find(x => x.name == templateName);
        pageToAdd.SetUIDoc(uIDocument);

        stack.Add(pageToAdd, page);

        page.SetActive(true);
        pageToAdd.SetSortOrder(stack.Count);

        if (executeShowCall) pageToAdd.ShowPage(arfs);

        pageToAdd.SetSafeAreaMargins();

        if (animateIn) yield return pageToAdd.AnimateIn();
    }

    public IEnumerator AddPageToStack<T>(object[] args = null, bool animateIn = true, bool executeShowCall = true) where T : Page, new()
    {
        GameObject page             = GOPages.Pop();
        page.transform.localScale   = Vector3.one;

        T pageToAdd                 = new T();
        UIDocument uIDocument       = page.GetComponent<UIDocument>();

        string templateName = typeof(T).ToString().Split("`")[0].Trim();
        uIDocument.visualTreeAsset = templates.Find(x => x.name == templateName);
        pageToAdd.SetUIDoc(uIDocument);

        stack.Add(pageToAdd, page);

        page.SetActive(true);
        pageToAdd.SetSortOrder(stack.Count);

        if (executeShowCall) pageToAdd.ShowPage(args);

        pageToAdd.SetSafeAreaMargins();

        if (animateIn) yield return pageToAdd.AnimateIn();
    }

    public T GetFirstPageOfType<T>() where T : Page
    {
        foreach (Page p in stack.Keys)
        {
            if (p is T)
                return p as T;
        }

        return null;
    }

    #endregion

    #region Private Functions

    private void MaxPageLimitReached()
    {
        //TODO: Add a cap to the number of pages that can be added to the stack
    }

    #endregion
}
