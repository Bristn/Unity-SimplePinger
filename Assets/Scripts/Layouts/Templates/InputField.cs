using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class InputField
{
    private VisualElement root;
    private CustomValidator validator;
    private List<string> invalidValues;

    private string lastValidValue = "";
    private string fromValue;

    private Dictionary<ValidType, bool> validIconVisiblity = new Dictionary<ValidType, bool>();
    private Dictionary<ValidType, bool> saveButtonVisiblity = new Dictionary<ValidType, bool>();
    private Dictionary<ValidType, bool> discardButtonVisiblity = new Dictionary<ValidType, bool>();

    private TextField textField;
    private VisualElement iconValid;
    private Button buttonSave;
    private Button buttonDiscard;

    private Action<InputField, string, string> onSave = null;
    private Action<InputField> onDiscard = null;
    private Action<string, bool> onChange = null;

    private EditableText connectedText;

    public enum ValidType
    {
        VALID,
        INVALID
    }

    public InputField
        (
            VisualElement pRoot,
            CustomValidator pValidator,
            List<string> pInvalidValues,
            string pValue,
            Dictionary<ValidType, bool> pValidIconVis,
            Dictionary<ValidType, bool> pSaveButtonVis,
            Dictionary<ValidType, bool> pDiscardButtonVis,
            Action<InputField, string, string> pOnSave,
            Action<InputField> pOnDiscard,
            Action<string, bool> pOnChange
        )
    {
        // Assign variables
        validator = pValidator;
        invalidValues = pInvalidValues;
        validIconVisiblity = pValidIconVis;
        saveButtonVisiblity = pSaveButtonVis;
        discardButtonVisiblity = pDiscardButtonVis;
        onSave = pOnSave;
        onDiscard = pOnDiscard;
        fromValue = pValue;
        onChange = pOnChange;

        // Assign UI elements
        root = pRoot.name.Equals("root") ? pRoot : root = pRoot.Q<VisualElement>("root");
        iconValid = root.Q<VisualElement>("icon-valid");
        textField = root.Q<TextField>("text-field");
        buttonSave = root.Q<Button>("button-save");
        buttonDiscard = root.Q<Button>("button-discard");
        Value = pValue;

        // Assign button actions
        buttonSave.clicked += PressedSave;
        buttonDiscard.clicked += PressedDisacrd;
        textField.RegisterValueChangedCallback(value => ChangedValue(value.newValue));

        // Apply style (not possible in editor GUI to style children of unity prefabs)
        textField.Children().First().style.marginLeft = 0;
        textField.Children().First().style.backgroundColor = new StyleColor(new Color(1, 1, 1, 0));
        textField.Children().First().SetBorderWidth(0);
        textField.Children().First().SetForegroundClass(StyleClasses.Foreground.ON_NEUTRAL);
        textField.Children().First().AddToClassList("text-regular");
    }

    private void PressedSave()
    {
        string valueBefore = fromValue;
        if (connectedText != null)
        {
            valueBefore = connectedText.Value;
            connectedText.Value = lastValidValue;
            Root.parent.ReplaceWith(Root, connectedText.Root);
        }
        onSave?.Invoke(this, valueBefore, lastValidValue);
    }

    private void PressedDisacrd() 
    {
        if (connectedText != null)
        {
            Root.parent.ReplaceWith(Root, connectedText.Root);
        }
        onDiscard?.Invoke(this);
    }

    private void ChangedValue(string pValue)
    {
        bool isValid = GetIsValid(pValue);
        if (isValid)
        {
            lastValidValue = pValue;
        }
        ValidType validType = isValid ? ValidType.VALID : ValidType.INVALID;


        // Update visuals
        root.SetBorderClass(isValid ? StyleClasses.Border.POSITIVE : StyleClasses.Border.NEGATIVE);

        if (validIconVisiblity.TryGetValue(validType, out bool iconValidVis))
        {
            iconValid.style.display = iconValidVis ? DisplayStyle.Flex : DisplayStyle.None;
            if (iconValidVis)
            {
                iconValid.style.backgroundImage = new StyleBackground(isValid ? UiIcons.InputFieldValid : UiIcons.InputFieldInvalid);
                iconValid.SetForegroundClass(isValid ? StyleClasses.Foreground.POSITIVE : StyleClasses.Foreground.NEGATIVE);
            }
        }

        if (saveButtonVisiblity.TryGetValue(validType, out bool buttonSaveVis))
        {
            buttonSave.style.display = buttonSaveVis ? DisplayStyle.Flex : DisplayStyle.None;
        }

        if (discardButtonVisiblity.TryGetValue(validType, out bool buttondiscardVis))
        {
            buttonDiscard.style.display = buttondiscardVis ? DisplayStyle.Flex : DisplayStyle.None;
        }

        onChange?.Invoke(pValue, isValid);
    }

    private bool GetIsValid(string pValue)
    {
        bool isValid = !invalidValues.Contains(pValue) || pValue.Equals(fromValue);
        if (isValid && validator != null)
        {
            isValid = validator.IsValidValue(pValue);
        }
        return isValid;
    }

    public string Value
    {
        get => lastValidValue;
        set
        {
            textField.value = value;
            ChangedValue(value);
        }
    }

    public VisualElement Root => root;

    public void SetConnectedText(EditableText pConnectedText)
    {
        connectedText = pConnectedText;
    }
}
