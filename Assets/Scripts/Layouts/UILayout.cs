using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class UILayout 
{
    protected UIDocument document;

    public virtual void Show()
    {

    }

    public void Hide()
    {
        if (document != null)
        {
            document.enabled = false;
        }
    }

    public void ShowOtherLayout(UILayout pLayout)
    {
        Hide();
        pLayout.Show();
    }
}
