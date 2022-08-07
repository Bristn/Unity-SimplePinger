using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SelectElement 
{
    private VisualElement indicator;
    private bool selected;
    private bool showIndicator;

    public SelectElement(VisualElement pIndicator, bool pSelected = false, bool pShowIndicator = false)
    {
        indicator = pIndicator;
        selected = pSelected;
        showIndicator = pShowIndicator;
        UpdateIndicator();
    }

    public bool Selected
    {
        get => selected;
        set
        {
            selected = value;
            UpdateIndicator();
        }
    }

    public bool ShowIndicator
    {
        get => showIndicator;
        set
        {
            showIndicator = value;
            UpdateIndicator();
        }
    }

    private void UpdateIndicator()
    {
        indicator.style.display = showIndicator ? DisplayStyle.Flex : DisplayStyle.None;
        indicator.style.backgroundImage = new StyleBackground(selected ? UiIcons.SelectionActive : UiIcons.SelectionInactive);
    }
}
