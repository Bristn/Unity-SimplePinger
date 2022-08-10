using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuBackButtonBuilder : MonoBehaviour
{
    // Singleton
    private static MenuBackButtonBuilder instance;

    [SerializeField] private VisualTreeAsset prefab;
    private void Awake()
    {
        instance = this;
    }

    public static VisualTreeAsset Prefab => instance.prefab;


    // Actual builder pattern
    private VisualElement root = null;
    private Vector2 iconSize = new Vector2(20, 20);
    private Vector2 iconButtonSize = new Vector2(50, 60);

    public MenuBackButtonBuilder Root(VisualElement pRoot)
    {
        root = pRoot;
        return this;
    }

    public MenuBackButtonBuilder IconSize(float pWidth = -1, float pHeight = -1)
    {
        iconSize = new Vector2(pWidth, pHeight);
        return this;
    }

    public MenuBackButtonBuilder IconButtonSize(float pWidth = -1, float pHeight = -1)
    {
        iconButtonSize = new Vector2(pWidth, pHeight);
        return this;
    }


    // Build
    public MenuBackButton Build()
    {
        // If no root is specified instantiate the prefab
        if (root == null)
        {
            root = Prefab.Instantiate().Q<VisualElement>("root");
        }

        return new MenuBackButton(root, iconSize, iconButtonSize);
    }
}
