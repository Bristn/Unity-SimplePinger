using Assets.Scripts;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuBackButton
{
    private Button root;
    private VisualElement icon;

    private Vector2 iconSize;
    private Vector2 iconButtonSize;

    public MenuBackButton(VisualElement pRoot, Vector2 pIconSize, Vector2 pIconButtonSize)
    {
        // Assign UI elements
        root = pRoot.name.Equals("root") ? (Button) pRoot : root = pRoot.Q<Button>("root");
        icon = root.Q<VisualElement>("icon");

        // Add style sheet & classes
        root.AddAllStyleSheets();

        // Update visuals
        icon.style.backgroundImage = new StyleBackground(UiIcons.MenuBack);
        IconSize = pIconSize;
        IconButtonSize = pIconButtonSize;
    }

    public Button Root => root;

    public Vector2 IconSize
    {
        get => iconSize;
        set
        {
            iconSize = value;
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
}
