using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static InputField;

public class EntryEditor : UILayout
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

    public void Show(EntryData pData, Action<EntryData, EntryData> pOnSave, Action pOnBack)
    {
        document = Application.EntryEditor;
        document.enabled = true;

        // Assign variables
        onSave = pOnSave;
        onBack = pOnBack;
        dataBefore = pData;

        // Assign UI elements
        VisualElement root = document.rootVisualElement;
        buttonSaveEntry = root.Q<Button>("button-save-entry");
        labelStatus = root.Q<Label>("label-status");
        labelAddress = root.Q<Label>("label-address");

        inputIp = new InputFieldBuilder()
            .Root(root.Q<VisualElement>("input-ip"))
            .Validator(new AddressValidator())
            .Value(pData.Host)
            .ValidIconVisiblity(ValidType.INVALID, true)
            .OnChange(InputIpChanged)
            .Build();

        inputName = new InputFieldBuilder()
            .Root(root.Q<VisualElement>("input-name"))
            .Value(pData.Name)
            .ValidIconVisiblity(ValidType.INVALID, true)
            .Build();

        inputSuffix = new InputFieldBuilder()
            .Root(root.Q<VisualElement>("input-suffix"))
            .Value(pData.Suffix)
            .ValidIconVisiblity(ValidType.INVALID, true)
            .OnChange(InputSuffixChanged)
            .Build();

        // Assign UI Actions
        buttonSaveEntry.clicked += PressedSaveEntry;

        // Create menu
        MenuItem itemBack = new MenuItemBuilder()
           .Icon(MenuItemBuilder.IconBack)
           .OnClick(PressedBack)
           .Build();

        Menu menu = new MenuBuilder()
            .MenuItems(itemBack, true)
            .Text("Entry editor")
            .Build();

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
        Hide();
        onSave?.Invoke(dataBefore, GetEntry());
    }

    private void PressedBack()
    {
        Hide();
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
