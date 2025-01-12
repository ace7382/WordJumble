using UnityEngine;
using UnityEngine.UIElements;

public class QuickButton : PointerManipulator
{
    private VisualElement display;
    private Color neutralStateColor;
    private Color highlightColor        = new Color(.886f, .886f, .886f);

    public QuickButton(VisualElement display, Color neutral)
    {
        this.display = display;
        this.neutralStateColor = neutral;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerOverEvent>(OnPointerOver);
        target.RegisterCallback<PointerOutEvent>(OnPointerOut);
        target.RegisterCallback<PointerDownEvent>(OnPointerDown);
        target.RegisterCallback<PointerUpEvent>(OnPointerUp);
        target.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerOverEvent>(OnPointerOver);
        target.UnregisterCallback<PointerOutEvent>(OnPointerOut);
        target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
        target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        target.UnregisterCallback<PointerLeaveEvent>(OnPointerLeave);
    }

    protected void OnPointerOver(PointerOverEvent e)
    {
        display.style.backgroundColor = neutralStateColor * highlightColor;
    }

    protected void OnPointerOut(PointerOutEvent e)
    {
        display.style.backgroundColor = neutralStateColor;
    }

    protected void OnPointerDown(PointerDownEvent e)
    {
        display.transform.scale = new Vector3(.95f, .95f, 1f);
    }

    protected void OnPointerUp(PointerUpEvent e)
    {
        display.transform.scale = Vector3.one;
    }

    protected void OnPointerLeave(PointerLeaveEvent e)
    {
        display.transform.scale = Vector3.one;
    }
}