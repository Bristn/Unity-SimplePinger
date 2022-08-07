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
    [SerializeField] private VectorImage iconMore;
    [SerializeField] private VectorImage iconBack;
    [SerializeField] private VectorImage iconDelete;
    [SerializeField] private StyleSheet styleSheetMenu;

    private void Awake()
    {
        instance = this;
    }

    public static VisualTreeAsset Prefab => instance.prefab;

    public static VectorImage IconMore => instance.iconMore;

    public static VectorImage IconBack => instance.iconBack;

    public static VectorImage IconDelete => instance.iconDelete;

    public static StyleSheet StyleSheetMenu => instance.styleSheetMenu;


    // Actual builder pattern
    private VisualElement root = null;

    private string text = string.Empty;

    private VectorImage icon = null;
    private float iconSize = 30;
    private float iconButtonSize = 60;

    private Action<MenuItem> onClick = null;
    private Action onClickNone = null;


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

    public MenuItemBuilder OnClick(Action<MenuItem> pOnClick)
    {
        onClick = pOnClick;
        return this;
    }

    public MenuItemBuilder OnClick(Action pOnClick)
    {
        onClickNone = pOnClick;
        return this;
    }

    public MenuItemBuilder IconSize(float pSize)
    {
        iconSize = pSize;
        return this;
    }

    public MenuItemBuilder IconButtonSize(float pSize)
    {
        iconButtonSize = pSize;
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

        return new MenuItem(root, text, icon, iconSize, iconButtonSize, onClick, onClickNone);
    }
}
