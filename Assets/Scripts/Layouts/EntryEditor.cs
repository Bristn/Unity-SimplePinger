using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static InputField;

public class EntryEditor : UiScreen
{
    private Button buttonSaveEntry;
    private Label labelStatus;
    private Label labelAddress;

    private InputField inputIp;
    private InputField inputName;
    private InputField inputSuffix;

    private Action<EntryData, EntryData> onSave;
    private Action onBack;

    private EntryData dataBefore;
    private ConnectionStatus status;

    public enum ConnectionStatus
    {
        INVALID,
        SUCCESS,
        FAILURE
    }

    public EntryEditor(EntryData pData, Action<EntryData, EntryData> pOnSave, Action pOnBack)
    {
        document = Application.EntryEditor;
        onSave = pOnSave;
        onBack = pOnBack;
        dataBefore = pData;
    }

    public override void CreateMenu()
    {
        MenuBuilder builder = new MenuBuilder()
            .Text("Entry editor")
            .ShowBackButton(true);
        base.CreateMenu(builder);
    }

    public override void Open()
    {
        CreateMenu();
        base.Open();

        // Assign UI elements
        VisualElement root = document.rootVisualElement;
        buttonSaveEntry = root.Q<Button>("button-save-entry");
        labelStatus = root.Q<Label>("label-status");
        labelAddress = root.Q<Label>("label-address");

        inputIp = new InputFieldBuilder()
            .Root(root.Q<VisualElement>("input-ip"))
            .Validator(new AddressValidator())
            .Value(dataBefore.Host)
            .ValidIconVisiblity(ValidType.INVALID, true)
            .OnChange(InputIpChanged)
            .Placeholder("Enter host address ...")
            .Build();

        inputName = new InputFieldBuilder()
            .Root(root.Q<VisualElement>("input-name"))
            .Value(dataBefore.Name)
            .ValidIconVisiblity(ValidType.INVALID, true)
            .Placeholder("Enter entry name ...")
            .Build();

        inputSuffix = new InputFieldBuilder()
            .Root(root.Q<VisualElement>("input-suffix"))
            .Value(dataBefore.Suffix)
            .ValidIconVisiblity(ValidType.INVALID, true)
            .OnChange(InputSuffixChanged)
            .Placeholder("Enter suffix ...")
            .Build();

        // Assign UI Actions
        buttonSaveEntry.clicked += PressedSaveEntry;
        root.Add(menu.Root);
    }

    private void InputIpChanged(string pValue, bool pIsValid)
    {
        if (!pIsValid)
        {
            Status = ConnectionStatus.INVALID;
        }
        else
        {
            Application.RunAsync(this.SetConnectionStatus(pValue));
            labelAddress.text = FullUrl;
        }
    }

    public ConnectionStatus Status
    {
        get => status;
        set
        {
            status = value;

            if (status == ConnectionStatus.INVALID)
            {
                labelStatus.text = "Invalid address";
                labelStatus.SetForegroundClass(StyleClasses.Foreground.NEGATIVE);
            }
            else if (status == ConnectionStatus.SUCCESS)
            {
                labelStatus.text = "Able to connect";
                labelStatus.SetForegroundClass(StyleClasses.Foreground.POSITIVE);
            }
            else if (status == ConnectionStatus.FAILURE)
            {
                labelStatus.text = "Unable to connect";
                labelStatus.SetForegroundClass(StyleClasses.Foreground.NEGATIVE);
            }
        }
    }

    public string FullUrl 
    {
        get
        {
            if (inputIp != null && inputSuffix != null && !inputIp.Value.Equals(string.Empty))
            {
                return inputIp.Value + "/" + inputSuffix.Value;
            }
            return string.Empty;
        }
    }

    private void InputSuffixChanged(string pValue, bool pIsValid)
    {
        labelAddress.text = FullUrl;
    }

    private void PressedSaveEntry() 
    {
        Close();
        onSave?.Invoke(dataBefore, GetEntry());
    }

    public override void HandleBackButtonPress()
    {
        Close();
        onBack?.Invoke();
    }

    private EntryData GetEntry()
    {
        EntryData data = new EntryData();
        data.Host = inputIp.Value;
        data.Name = inputName.Value;
        data.Suffix = inputSuffix.Value;
        return data;
    }
}
