using Assets.Scripts;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuItem
{
    private VisualElement root;
    private Button rootButton;
    private VisualElement icon;
    private Label labelText;

    private Action onClick;
    private Vector2 iconSize;
    private Vector2 iconButtonSize;

    public MenuItem
        (
            VisualElement pRoot,
            string pText,
            VectorImage pIcon,
            Vector2 pIconSize,
            Vector2 pIconButtonSize,
            Action pOnClick,
            bool pIsBackButton
        )
    {
        // Assign variables
        onClick = pOnClick;

        // Assign UI elements
        root = pRoot.name.Equals("root") ? pRoot : root = pRoot.Q<VisualElement>("root");
        rootButton = (Button)root;
        icon = root.Q<VisualElement>("icon");
        labelText = root.Q<Label>("label-text");

        // Assign button actions
        if (!pIsBackButton)
        {
            rootButton.clicked += PressedItem;
        }

        // Add style sheet & classes
        root.AddAllStyleSheets();

        // Update visuals
        Text = pText;
        Icon = pIcon;
        IconSize = pIconSize;
        IconButtonSize = pIconButtonSize;
    }

    public VisualElement Root => root;

    public bool HasIcon => Icon != null;

    public Action OnClick
    {
        get => onClick;
        set => onClick = value;
    }

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

    public Vector2 IconSize
    {
        get => iconSize;
        set 
        {
            iconSize = value;
            if (Icon == null)
            {
                return;
            }

            if (iconSize.x != -1)
            {
                icon.style.width = iconSize.x;
            }

            if (iconSize.y != -1)
            {
                icon.style.height = iconSize.y;
            }
        }
    }

    public Vector2 IconButtonSize
    {
        get => iconButtonSize;
        set
        {
            iconButtonSize = value;
            if (Icon == null)
            {
                return;
            }

            if (iconButtonSize.x != -1)
            {
                root.style.width = iconButtonSize.x;
            }

            if (iconButtonSize.y != -1)
            {
                root.style.height = iconButtonSize.y;
            }
        }
    }

    public void PressedItem() => onClick?.Invoke();
}
