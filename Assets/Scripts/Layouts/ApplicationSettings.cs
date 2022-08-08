using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ApplicationSettings : UILayout
{
    private Checkbox toggleCode;
    private Checkbox toggleVibrationSuccess;
    private Checkbox toggleVibrationFailure;
    private Checkbox toggleSaveTab;

    public override void Show()
    {
        document = Application.Settings;
        document.enabled = true;

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

        // Create menu
        MenuItem itemBack = new MenuItemBuilder()
            .Icon(UiIcons.MenuBack)
            .OnClick(PressedBack)
            .Build();

        Menu menu = new MenuBuilder()
            .MenuItems(itemBack, true)
            .Text("Settings")
            .Build();

        root.Add(menu.Root);
    }

    private void PressedBack()
    {
        Persistence.SaveObjectToJson(SettingsData.Settings, "", Persistence.SETTING_FILE);

        Hide();

        TabSelection tabselection = new TabSelection();
        tabselection.Show();
    }
}
