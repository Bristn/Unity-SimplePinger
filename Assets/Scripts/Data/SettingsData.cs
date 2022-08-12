using UnityEngine;

[System.Serializable]
public class SettingsData
{
    private static SettingsData settings;

    public static SettingsData Settings
    {
        get
        {
            if (settings == null)
            {
                settings = (SettingsData)Persistence.LoadObjectFromJson(typeof(SettingsData), "", Persistence.SETTING_FILE);
                if (settings == null)
                {
                    settings = new SettingsData();
                }
            }
            return settings;
        }
    }


    [SerializeField] private bool showCode;
    [SerializeField] private bool vibrateOnSuccess;
    [SerializeField] private bool vibrateOnFailure;
    [SerializeField] private bool reopenTab;
    [SerializeField] private string lastTab = string.Empty;
    [SerializeField] private int timeout = 1;

    public bool ShowCode
    {
        get => showCode;
        set => showCode = value;
    }

    public bool VibrateOnSuccess
    {
        get => vibrateOnSuccess;
        set => vibrateOnSuccess = value;
    }

    public bool VibrateOnFailure
    {
        get => vibrateOnFailure;
        set => vibrateOnFailure = value;
    }

    public bool ReopenTab
    {
        get => reopenTab;
        set => reopenTab = value;
    }

    public string LastTab
    {
        get => lastTab;
        set => lastTab = value;
    }

    public int Timeout
    {
        get => timeout;
        set => timeout = value;
    }
}
