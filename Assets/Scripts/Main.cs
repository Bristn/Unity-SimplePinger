using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Main : MonoBehaviour
{
    public static Main Instance;

    [SerializeField] private InputActionAsset inputAsset;

    [Header("UI Documents")]
    [SerializeField] private UIDocument tabSelection;
    [SerializeField] private UIDocument entrySelection;
    [SerializeField] private UIDocument entryEditor;
    [SerializeField] private UIDocument settings;

    void Start()
    {
        Screen.fullScreen = false;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        Instance = this;
        tabSelection.enabled = false;
        entrySelection.enabled = false;
        entryEditor.enabled = false;
        settings.enabled = false;

        Profiles.SetupProfiles(inputAsset.FindActionMap("UI"));

        Persistence.CreateDirectories();

        // Open correct ui
        if (SettingsData.Settings.ReopenTab && !SettingsData.Settings.LastTab.Equals(string.Empty))
        {
            new EntrySelection(SettingsData.Settings.LastTab).Open();
        }
        else
        {
            new TabSelection().Open();
        }
    }

    public static UIDocument TabSelection => Instance.tabSelection;

    public static UIDocument EntrySelection => Instance.entrySelection;

    public static UIDocument EntryEditor => Instance.entryEditor;

    public static UIDocument Settings => Instance.settings;

    public static void RunAsync(IEnumerator pRoutine) => Instance.StartCoroutine(pRoutine);
    
    public static void StopAsync(IEnumerator pRoutine) => Instance.StopCoroutine(pRoutine);
    
    private void OnApplicationFocus(bool pFocused)
    {
        Profiles.OnApplicationFocus(pFocused);
    }

    private void OnApplicationPause(bool pPaused)
    {
        Profiles.OnApplicationPause(pPaused);
        if (!pPaused)
        {
            if (TabPersistence.ImportTab())
            {
                tabSelection.enabled = false;
                entrySelection.enabled = false;
                entryEditor.enabled = false;
                settings.enabled = false;
                new TabSelection().Open();
            }
        }
        else
        {
            Persistence.SaveObjectToJson(SettingsData.Settings, "", Persistence.SETTING_FILE);
        }
    }

    private void OnApplicationQuit()
    {
        Persistence.SaveObjectToJson(SettingsData.Settings, "", Persistence.SETTING_FILE);
    }
}