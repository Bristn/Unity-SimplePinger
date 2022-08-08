using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuItemBuilder : MonoBehaviour
{
    // Singleton
    private static MenuItemBuilder instance;

    [SerializeField] private VisualTreeAsset prefab;
    private void Awake()
    {
        instance = this;
    }

    public static VisualTreeAsset Prefab => instance.prefab;


    // Actual builder pattern
    private VisualElement root = null;

    private string text = string.Empty;

    private VectorImage icon = null;
    private Vector2 iconSize = new Vector2(20, 20);
    private Vector2 iconButtonSize = new Vector2(50, 60);

    private Action onClick = null;
    private bool isBackButton;


    public MenuItemBuilder Root(VisualElement pRoot)
    {
        root = pRoot;
        return this;
    }

    public MenuItemBuilder Text(string pText)
    {
        text = pText;
        return this;
    }

    public MenuItemBuilder Icon(VectorImage pIcon)
    {
        icon = pIcon;
        return this;
    }

    public MenuItemBuilder OnClick(Action pOnClick)
    {
        onClick = pOnClick;
        return this;
    }

    public MenuItemBuilder IconSize(float pWidth = -1, float pHeight = -1)
    {
        iconSize = new Vector2(pWidth, pHeight);
        return this;
    }

    public MenuItemBuilder IconButtonSize(float pWidth = -1, float pHeight = -1)
    {
        iconButtonSize = new Vector2(pWidth, pHeight);
        return this;
    }

    public MenuItemBuilder IsBackButton(bool pIsBack)
    {
        isBackButton = pIsBack;
        return this;
    }


    // Build
    public MenuItem Build()
    {
        // If no root is specified instantiate the prefab
        if (root == null)
        {
            root = Prefab.Instantiate().Q<VisualElement>("root");
        }

        return new MenuItem(root, text, icon, iconSize, iconButtonSize, onClick, isBackButton);
    }
}
