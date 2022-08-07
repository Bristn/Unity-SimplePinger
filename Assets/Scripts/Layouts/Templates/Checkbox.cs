using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Checkbox
{
    private VisualElement root;
    private Toggle rootToggle;
    private VisualElement parent;

    private Label labelText;

    private VisualElement checkbox;

    private VisualElement indicatorParent;
    private VisualElement indicatorBackground;
    private VisualElement indicatorDot;

    private Action<bool> onSelect = null;


    public Checkbox(VisualElement pRoot, bool pSelected, string pText, Action<bool> pOnSelect)
    {
        // Assign variables
        onSelect = pOnSelect;

        // Assign UI elements
        root = pRoot.name.Equals("root") ? pRoot : root = pRoot.Q<VisualElement>("root");
        rootToggle = (Toggle)root;
        parent = root.Children().First();
        checkbox = parent.Children().First();
        labelText = (Label)parent.Children().Last();

        // Assign button actions
        rootToggle.RegisterValueChangedCallback(value => ChangedValue(value.newValue));

        // Apply style (not possible in editor GUI to style children of unity prefabs)
        checkbox.style.display = DisplayStyle.None;

        indicatorParent = new VisualElement();
        indicatorParent.styleSheets.Add(Application.StyleCheckbox);
        indicatorParent.AddToClassList("checkbox-parent");
        parent.Insert(0, indicatorParent);

        indicatorBackground = new VisualElement();
        indicatorBackground.styleSheets.Add(Application.StyleCheckbox);
        indicatorBackground.AddToClassList("checkbox-background");
        indicatorBackground.AddToClassList("checkbox-background--selected");
        indicatorParent.Add(indicatorBackground);

        indicatorDot = new VisualElement();
        indicatorDot.styleSheets.Add(Application.StyleCheckbox);
        indicatorDot.AddToClassList("checkbox-dot");
        indicatorDot.AddToClassList("checkbox-dot--selected");
        indicatorParent.Add(indicatorDot);

        labelText.style.marginLeft = 10;
        labelText.SetForegroundClass(StyleClasses.Foreground.ON_NEUTRAL);
        labelText.AddToClassList("text-regular");

        // Update visuals
        Selected = pSelected;
        Text = pText;
    }

    private void ChangedValue(bool pNew)
    {
        onSelect?.Invoke(pNew);

        indicatorBackground.EnableInClassList("checkbox-background--selected", pNew);
        indicatorDot.EnableInClassList("checkbox-dot--selected", pNew);
    }

    public VisualElement Root => root;

    public bool Selected
    {
        get => rootToggle.value;
        set
        {
            rootToggle.value = value;
        }
    }

    public string Text
    {
        get => labelText.text;
        set
        {
            labelText.text = value;
        }
    }
}
