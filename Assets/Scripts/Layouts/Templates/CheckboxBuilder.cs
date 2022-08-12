using System;
using UnityEngine;
using UnityEngine.UIElements;

public class CheckboxBuilder : MonoBehaviour
{
    // Singleton
    private static CheckboxBuilder instance;

    [SerializeField] private VisualTreeAsset prefab;

    private void Awake()
    {
        instance = this;
    }

    public static VisualTreeAsset Prefab => instance.prefab;


    // Actual builder pattern
    private VisualElement root = null;

    private bool selected;
    private string text;

    private Action<bool> onChange = null;


    public CheckboxBuilder Root(VisualElement pRoot)
    {
        root = pRoot;
        return this;
    }

    public CheckboxBuilder Selected(bool pSelected)
    {
        selected = pSelected;
        return this;
    }

    public CheckboxBuilder Text(string pText)
    {
        text = pText;
        return this;
    }

    public CheckboxBuilder Onchange(Action<bool> pOnChange)
    {
        onChange = pOnChange;
        return this;
    }


    // Build
    public Checkbox Build()
    {
        // If no root is specified instantiate the prefab
        if (root == null)
        {
            root = Prefab.Instantiate().Q<VisualElement>("root");
        }

        return new Checkbox(root, selected, text, onChange);
    }
}
