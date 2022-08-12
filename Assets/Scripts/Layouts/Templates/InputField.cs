using Assets.Scripts;
using System;
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
    private string placeholderValue;
    private string hintValue;
    private bool hasFocus;

    private Dictionary<ValidType, bool> validIconVisiblity = new Dictionary<ValidType, bool>();
    private Dictionary<ValidType, bool> saveButtonVisiblity = new Dictionary<ValidType, bool>();
    private Dictionary<ValidType, bool> discardButtonVisiblity = new Dictionary<ValidType, bool>();

    private TextField textField;
    private VisualElement iconValid;
    private Button buttonSave;
    private Button buttonDiscard;
    private Label textPlaceholder;
    private Label textHint;
    private VisualElement textInput;

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
            Action<string, bool> pOnChange,
            string pPlaceholder,
            string pHint
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
        textPlaceholder = root.Q<Label>("text-placeholder");
        textHint = root.Q<Label>("text-hint");

        // Apply style (not possible in editor GUI to style children of unity prefabs)
        textInput = textField.Children().First();
        textInput.style.marginLeft = 20;
        textInput.style.marginTop = 15;
        textInput.SetPadding(0);
        textInput.style.backgroundColor = new StyleColor(new Color(1, 1, 1, 0));
        textInput.SetBorderWidth(0);
        textInput.SetForegroundClass(StyleClasses.Foreground.ON_NEUTRAL);
        textInput.AddToClassList("text-size-regular");
        
        Value = pValue;
        PlaceholderValue = pPlaceholder;
        HintValue = pHint;

        // Assign button actions
        buttonSave.clicked += PressedSave;
        buttonDiscard.clicked += PressedDisacrd;
        textField.RegisterValueChangedCallback(value => ChangedValue(value.newValue));

        Action<bool> focusChanged = (value) =>
        {
            hasFocus = value;
            UpdatePlaceholder();
            UpdateHint();
        };
        textField.RegisterCallback<FocusInEvent>((e) => focusChanged.Invoke(true));
        textField.RegisterCallback<FocusOutEvent>((e) => focusChanged.Invoke(false));

        // Add style sheet & classes
        root.AddAllStyleSheets();
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

        UpdatePlaceholder();
        UpdateHint();
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


    public string PlaceholderValue
    {
        get => placeholderValue;
        set
        {
            placeholderValue = value;
            textPlaceholder.text = placeholderValue;
            UpdatePlaceholder();
        }
    }

    private void UpdatePlaceholder()
    {
        bool showPlaceholder = (textField.value == string.Empty || textField.value.Length == 0) && !hasFocus;
        textPlaceholder.style.display = showPlaceholder ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public string HintValue
    {
        get => hintValue;
        set
        {
            hintValue = value;
            textHint.text = hintValue;
            UpdateHint();
        }
    }

    private void UpdateHint()
    {
        bool showHint = (textField.value != string.Empty || hasFocus) && hintValue != string.Empty;
        textHint.style.display = showHint ? DisplayStyle.Flex : DisplayStyle.None;

        int placeholderPos = showHint ? 5 : 0;
        textPlaceholder.style.paddingTop = placeholderPos;

        int inputPos = showHint ? 15 : 0;
        textInput.style.marginTop = inputPos;
    }

    public VisualElement Root => root;

    public void SetConnectedText(EditableText pConnectedText) => connectedText = pConnectedText;
}
