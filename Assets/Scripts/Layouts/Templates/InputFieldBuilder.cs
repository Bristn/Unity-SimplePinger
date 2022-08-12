using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class InputFieldBuilder : MonoBehaviour
{
    // Singleton
    private static InputFieldBuilder instance;

    [SerializeField] private VisualTreeAsset prefab;

    private void Awake()
    {
        instance = this;
    }

    public static VisualTreeAsset Prefab => instance.prefab;

    // Actual builder pattern
    private VisualElement root = null;
    private CustomValidator validator = null;
    private List<string> invalidValues = new List<string>();

    private string value = string.Empty;
    private string placeholder = string.Empty;
    private string hint = string.Empty;

    private Dictionary<InputField.ValidType, bool> validIconVisiblity = new Dictionary<InputField.ValidType, bool>();
    private Dictionary<InputField.ValidType, bool> saveButtonVisiblity = new Dictionary<InputField.ValidType, bool>();
    private Dictionary<InputField.ValidType, bool> discardButtonVisiblity = new Dictionary<InputField.ValidType, bool>();

    private Action<InputField, string, string> onSave = null;
    private Action<InputField> onDiscard = null;
    private Action<string, bool> onChange = null;


    public InputFieldBuilder Root(VisualElement pRoot)
    {
        root = pRoot;
        return this;
    }

    public InputFieldBuilder Validator(CustomValidator pValidator)
    {
        validator = pValidator;
        return this;
    }

    public InputFieldBuilder InvalidValues(List<string> pInvalidValues)
    {
        invalidValues = pInvalidValues;
        return this;
    }

    public InputFieldBuilder InvalidValues(params string[] pInvalidValues)
    {
        invalidValues = pInvalidValues.ToList();
        return this;
    }

    public InputFieldBuilder Value(string pValue)
    {
        value = pValue;
        return this;
    }

    public InputFieldBuilder ValidIconVisiblity(InputField.ValidType pValidType, bool pVisiblity)
    {
        validIconVisiblity.Add(pValidType, pVisiblity);
        return this;
    }

    public InputFieldBuilder SaveButtonVisiblity(InputField.ValidType pValidType, bool pVisiblity)
    {
        saveButtonVisiblity.Add(pValidType, pVisiblity);
        return this;
    }

    public InputFieldBuilder DiscardButtonVisiblity(InputField.ValidType pValidType, bool pVisiblity)
    {
        discardButtonVisiblity.Add(pValidType, pVisiblity);
        return this;
    }

    public InputFieldBuilder OnSave(Action<InputField, string, string> pOnSave)
    {
        onSave = pOnSave;
        return this;
    }

    public InputFieldBuilder OnDiscard(Action<InputField> pOnDiscard)
    {
        onDiscard = pOnDiscard;
        return this;
    }

    public InputFieldBuilder OnChange(Action<string, bool> pOnChange)
    {
        onChange = pOnChange;
        return this;
    }

    public InputFieldBuilder Placeholder(string pPlaceholder)
    {
        placeholder = pPlaceholder;
        return this;
    }

    public InputFieldBuilder Hint(string pHint)
    {
        hint = pHint;
        return this;
    }

    // Build
    public InputField Build()
    {
        // If no root is specified instantiate the prefab
        if (root == null)
        {
            root = Prefab.Instantiate().Q<VisualElement>("root");
        }

        // By default hide every indicator
        for (int i = 0; i < System.Enum.GetNames(typeof(InputField.ValidType)).Length; i++)
        {
            InputField.ValidType validType = (InputField.ValidType) i;
            validIconVisiblity.TryAdd(validType, false);
            saveButtonVisiblity.TryAdd(validType, false);
            discardButtonVisiblity.TryAdd(validType, false);
        }

        return new InputField
            (
                root, 
                validator,
                invalidValues,
                value, 
                validIconVisiblity, 
                saveButtonVisiblity,
                discardButtonVisiblity,
                onSave,
                onDiscard,
                onChange,
                placeholder,
                hint
            );
    }
}
