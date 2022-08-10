using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class UiScreen 
{
    protected UIDocument document;

    public virtual void Open()
    {

    }

    public void Close()
    {
        if (document != null)
        {
            document.enabled = false;
        }
    }

    public void OpenOtherScreen(UiScreen pLayout)
    {
        Close();
        pLayout.Open();
    }
}
