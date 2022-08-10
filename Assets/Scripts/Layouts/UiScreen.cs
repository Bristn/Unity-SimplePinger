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
        ApplicationBack.AddCallback(PressBackButton);
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
    

    public void Close()
    {
        ApplicationBack.RemoveCallback(PressBackButton);
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
