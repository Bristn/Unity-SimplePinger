using PlayerLoopProfiles;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class LongClickElement
{
    private VisualElement button;

    private Action onClick;
    private Action onLongClick;
    private float longClickDuration;

    private bool pressed;
    private float timePassed;

    public LongClickElement(VisualElement pButton, Action pOnClick, Action pOnLongClick = null, float pLongClickDuration = 0.5f)
    {
        button = pButton;
        onClick = pOnClick;
        onLongClick = pOnLongClick;
        longClickDuration = pLongClickDuration;

        // Register callbacks
        button.RegisterCallback<PointerDownEvent>(PointerDown, TrickleDown.TrickleDown);
        button.RegisterCallback<PointerUpEvent>(PointerUp, TrickleDown.TrickleDown);
        button.RegisterCallback<PointerLeaveEvent>(PointerLeave, TrickleDown.NoTrickleDown);

        // Add to Manager 
        LongClickManager.AddElement(this);
    }

    private void PointerDown(PointerDownEvent pEvent)
    {
        if (pEvent.button != 0)
        {
            return;
        }

        pressed = true;
        timePassed = 0;
    }

    private void PointerUp(PointerUpEvent pEvent)
    {
        if (!pressed || pEvent.button != 0)
        {
            return;
        }

        Main.RunAsync(InvokeOnclick());
        Reset();
    }

    // Invoke action in coroutine. Causes Nullpointerexception in Unity's core Clickable code (Clickable.ContainsPointer)
    private IEnumerator InvokeOnclick()
    {
        yield return null;
        onClick?.Invoke();
    }

    private void PointerLeave(PointerLeaveEvent pEvent) => Reset();

    public void Update()
    {
        if (!pressed)
        {
            return;
        }

        if (timePassed >= longClickDuration)
        {
            PlayerLoopTimeout.AddInteraction(Profiles.actionNames[(int)Profiles.InteractionType.CLICK]);
            onLongClick?.Invoke();
            Reset();
        }

        timePassed += Time.deltaTime;
    }

    public void Reset() => pressed = false;
}
