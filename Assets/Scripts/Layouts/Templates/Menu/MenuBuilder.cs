using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuBuilder : MonoBehaviour
{
    // Singleton
    private static MenuBuilder instance;

    [SerializeField] private VisualTreeAsset prefab;

    private void Awake()
    {
        instance = this;
    }

    public static VisualTreeAsset Prefab => instance.prefab;


    // Actual builder pattern
    private VisualElement root = null;

    private string text = "";
    private Dictionary<MenuItem, bool> menuItems = new Dictionary<MenuItem, bool>();
    private SelectionTracker selectionTracker;
    private Action onBack;
    private bool showBackButton;

    public MenuBuilder Root(VisualElement pRoot)
    {
        root = pRoot;
        return this;
    }

    public MenuBuilder Text(string pText)
    {
        text = pText;
        return this;
    }

    public MenuBuilder MenuItems(Dictionary<MenuItem, bool> pMenuItems)
    {
        menuItems = pMenuItems;
        return this;
    }

    public MenuBuilder MenuItems(MenuItem pItem, bool pIsLeft = false)
    {
        menuItems.Add(pItem, pIsLeft);
        return this;
    }

    public MenuBuilder SelectionTracker(SelectionTracker pSelectionTracker)
    {
        selectionTracker = pSelectionTracker;
        return this;
    }

    public MenuBuilder OnClickedBack(Action pOnBack)
    {
        onBack = pOnBack;
        return this;
    }

    public MenuBuilder ShowBackButton(bool pShow)
    {
        showBackButton = pShow;
        return this;
    }


    // Build
    public Menu Build()
    {
        // If no root is specified instantiate the prefab
        if (root == null)
        {
            root = Prefab.Instantiate().Q<VisualElement>("root");
        }

        return new Menu(root, text, menuItems, selectionTracker, onBack, showBackButton);
    }
}
