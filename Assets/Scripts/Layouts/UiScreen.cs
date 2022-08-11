using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class UiScreen 
{
    protected UIDocument document;

    protected Menu menu;

    public virtual void Open()
    {
        document.enabled = true;
        document.rootVisualElement.style.opacity = 1;
        ApplicationBack.AddCallback(PressBackButton); 
    }

    public void Close()
    {
        ApplicationBack.RemoveCallback(PressBackButton);
        if (document != null)
        {
            document.enabled = false;
        }
    }

    public void Show()
    {
        if (document != null)
        {
            document.rootVisualElement.style.opacity = 1;
        }
        ApplicationBack.AddCallback(PressBackButton);
    }

    public void Hide()
    {
        ApplicationBack.RemoveCallback(PressBackButton);
        if (document != null)
        {
            document.rootVisualElement.style.opacity = 0;
        }
    }

    public void OpenOtherScreen(UiScreen pLayout, bool pHideCurrent = false)
    {
        if (pHideCurrent)
        {
            Hide();
        }
        else
        {
            Close();
        }
        pLayout.Open();
    }

    public virtual void CreateMenu()
    {

    }

    public void CreateMenu(MenuBuilder pBuilder)
    {
        pBuilder.OnClickedBack(HandleBackButtonPress);
        menu = pBuilder.Build();
    }

    public void PressBackButton() => menu.PressedBack();

    public abstract void HandleBackButtonPress();
    

 
}
