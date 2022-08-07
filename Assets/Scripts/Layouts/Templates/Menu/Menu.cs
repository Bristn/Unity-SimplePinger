using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Menu : UILayout
{
    private VisualElement root;
    private VisualElement parentIcon;
    private VisualElement parentText;
    private VisualElement background;
    private Label labelText;

    private MenuItem itemShowMenu;
    private HashSet<MenuItem> menuItems = new HashSet<MenuItem>();

    public Menu(VisualElement pRoot, string pText, Dictionary<MenuItem, bool> pItems)
    {
        // Assign UI elements
        root = pRoot.name.Equals("root") ? pRoot : root = pRoot.Q<VisualElement>("root");
        parentIcon = root.Q<VisualElement>("parent-icon");
        parentText = root.Q<VisualElement>("parent-text");
        labelText = root.Q<Label>("label-text");
        background = root.Q<VisualElement>("background");

        // Assign UI Actions
        background.RegisterCallback<ClickEvent>((value) => PressedBackground());
        PressedBackground();

        // Add items
        foreach (KeyValuePair<MenuItem, bool> pair in pItems)
        {
            AddMenuItem(pair.Key, pair.Value);
        }

        // Update visuals
        Text = pText;
    }

    public VisualElement Root => root;

    public string Text
    {
        get => labelText.text;
        set
        {
            labelText.text = value;
        }
    }

    public void AddMenuItem(MenuItem pItem, bool pIsLeft = false)
    {
        if (!menuItems.Add(pItem))
        {
            return;
        }

        if (pItem.HasIcon)
        {
            if (pIsLeft)
            {
                parentIcon.Insert(0, pItem.Root);
            }
            else
            {
                if (itemShowMenu == null)
                {
                    parentIcon.Add(pItem.Root);
                }
                else
                {
                    parentIcon.Insert(parentIcon.childCount - 1, pItem.Root);
                }
            }
        }
        else
        {
            parentText.Add(pItem.Root);
            if (itemShowMenu == null)
            {
                itemShowMenu = new MenuItemBuilder()
                    .Icon(MenuItemBuilder.IconMore)
                    .OnClick(PressedShowMore)
                    .Build();

                parentIcon.Add(itemShowMenu.Root);
            }
        }
    }

    public void RemoveMenuItem(MenuItem pItem)
    {
        if (!menuItems.Remove(pItem))
        {
            return;
        }

        if (pItem.HasIcon)
        {
            parentIcon.Remove(pItem.Root);
        }
        else
        {
            parentText.Remove(pItem.Root);
            if (parentText.childCount == 0)
            {
                itemShowMenu.Root.style.display = DisplayStyle.None;
            }
        }
    }

    public void SetMenuItemVisible(MenuItem pItem, bool pVisible)
    {
        pItem.Root.style.display = pVisible ? DisplayStyle.Flex : DisplayStyle.None;
        foreach (VisualElement child in parentText.Children())
        {
            if (child.style.display != DisplayStyle.None)
            {
                itemShowMenu.Root.style.display = DisplayStyle.Flex;
                return;
            }
        }

        itemShowMenu.Root.style.display = DisplayStyle.None;
    }

    private void PressedShowMore(MenuItem pItem)
    {
        background.style.display = DisplayStyle.Flex;
        parentText.style.display = DisplayStyle.Flex;
    }

    private void PressedBackground() => CloseMenu();

    public void CloseMenu()
    {
        background.style.display = DisplayStyle.None;
        parentText.style.display = DisplayStyle.None;
    }
}
