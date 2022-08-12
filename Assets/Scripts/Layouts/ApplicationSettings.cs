using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ApplicationSettings : UiScreen
{
    private Checkbox toggleCode;
    private Checkbox toggleVibrationSuccess;
    private Checkbox toggleVibrationFailure;
    private Checkbox toggleSaveTab;
    private InputField inputTimeout;

    public ApplicationSettings()
    {
        document = Application.Settings;
    }

    public override void CreateMenu()
    {
        MenuBuilder builder = new MenuBuilder()
            .OnClickedBack(HandleBackButtonPress)
            .ShowBackButton(true)
            .Text("Settings");
        base.CreateMenu(builder);
    }

    public override void Open()
    {
        CreateMenu();
        base.Open();

        // Assign UI elements
        VisualElement root = document.rootVisualElement;

        toggleCode = new CheckboxBuilder()
            .Root(root.Q("toggle-code"))
            .Text("Show response code")
            .Selected(SettingsData.Settings.ShowCode)
            .Onchange((selected) => SettingsData.Settings.ShowCode = selected)
            .Build();

        toggleVibrationSuccess = new CheckboxBuilder()
            .Root(root.Q("toggle-vibration-success"))
            .Text("Vibrate on success")
            .Selected(SettingsData.Settings.VibrateOnSuccess)
            .Onchange((selected) => SettingsData.Settings.VibrateOnSuccess = selected)
            .Build();

        toggleVibrationFailure = new CheckboxBuilder()
            .Root(root.Q("toggle-vibration-failure"))
            .Text("Vibrate on faiure")
            .Selected(SettingsData.Settings.VibrateOnFailure)
            .Onchange((selected) => SettingsData.Settings.VibrateOnFailure = selected)
            .Build();

        toggleSaveTab = new CheckboxBuilder()
            .Root(root.Q("toggle-save-tab"))
            .Text("Save opened tab")
            .Selected(SettingsData.Settings.ReopenTab)
            .Onchange((selected) => SettingsData.Settings.ReopenTab = selected)
            .Build();

        inputTimeout = new InputFieldBuilder()
            .Root(root.Q("input-timeout"))
            .Hint("Timeout in seconds")
            .Validator(new DecimalValidator(1, 10, true))
            .Value(SettingsData.Settings.Timeout + "")
            .OnChange(InputTimeoutChanged)
            .Build();

        root.Add(menu.Root);
    }

    private void InputTimeoutChanged(string pValue, bool pIsValid)
    {
        if (pIsValid)
        {
            SettingsData.Settings.Timeout = int.Parse(pValue);
        }
    }

    public override void HandleBackButtonPress()
    {
        Persistence.SaveObjectToJson(SettingsData.Settings, "", Persistence.SETTING_FILE);
        OpenOtherScreen(new TabSelection());
    }
}
