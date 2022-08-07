using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuItem
{
    private VisualElement root;
    private Button rootButton;
    private VisualElement icon;
    private Label labelText;

    private Action<MenuItem> onClick;
    private Action onClickNone;
    public float iconSize;
    public float iconButtonSize;

    public MenuItem
        (
            VisualElement pRoot,
            string pText,
            VectorImage pIcon,
            float pIconSize,
            float pIconButtonSize,
            Action<MenuItem> pOnClick,
            Action pOnClickNone
        )
    {
        // Assign variables
        onClick = pOnClick;
        onClickNone = pOnClickNone;

        // Assign UI elements
        root = pRoot.name.Equals("root") ? pRoot : root = pRoot.Q<VisualElement>("root");
        rootButton = (Button)root;
        icon = root.Q<VisualElement>("icon");
        labelText = root.Q<Label>("label-text");

        // Assign button actions
        rootButton.clicked += PressedItem;

        // Add style sheet & classes
        root.styleSheets.Add(MenuItemBuilder.StyleSheetMenu);
        root.AddToClassList("menu-item-text");
        root.AddToClassList("menu-item-icon");
        root.AddToClassList("menu-no-border");

        // Update visuals
        Text = pText;
        Icon = pIcon;
        IconSize = pIconSize;
        IconButtonSize = pIconButtonSize;
    }

    public VisualElement Root => root;

    public bool HasIcon => Icon != null;

    public string Text
    {
        get => labelText.text;
        set
        {
            bool isValid = !value.Equals(string.Empty);
            if (isValid)
            {
                labelText.text = value;
                root.EnableInClassList("menu-item-text", true);
                root.EnableInClassList("menu-item-icon", false);
            }

            labelText.style.display = isValid ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    public VectorImage Icon
    {
        get => icon.style.backgroundImage.value.vectorImage;
        set
        {
            bool isValid = value != null;
            if (isValid)
            {
                icon.style.backgroundImage = new StyleBackground(value);
                root.EnableInClassList("menu-item-text", false);
                root.EnableInClassList("menu-item-icon", true);
            }

            icon.style.display = isValid ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    public float IconSize
    {
        get => iconSize;
        set 
        {
            iconSize = value;
            if (Icon != null)
            {
                icon.style.width = iconSize;
                icon.style.height = iconSize;
            }
        }
    }

    public float IconButtonSize
    {
        get => iconButtonSize;
        set
        {
            iconButtonSize = value;
            if (Icon != null)
            {
                root.style.width = iconButtonSize;
                root.style.height = iconButtonSize;
            }
        }
    }

    private void PressedItem()
    {
        onClick?.Invoke(this);
        onClickNone?.Invoke();
    }
}
