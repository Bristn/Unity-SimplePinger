using PlayerLoopProfiles;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public enum InteractionType
    {
        NAVIGATE,
        POINT,
        RIGHT_CLICK,
        MIDDLE_CLICK,
        CLICK,
        SCROLL_WHEEL,
        SUBMIT,
        CANCEL,
        TRACKED_DEVICE_POSITION,
        TRACKED_DEVICE_ORIENTATION,
    }

    public static List<string> actionNames = new string[]
    {
        "Navigate",
        "Point",
        "RightClick",
        "MiddleClick",
        "Click",
        "ScrollWheel",
        "Submit",
        "Cancel",
        "TrackedDevicePosition",
        "TrackedDeviceOrientation"
    }.ToList();

    void Start()
    {
        Screen.fullScreen = false;

        Instance = this;
        tabSelection.enabled = false;
        entrySelection.enabled = false;
        entryEditor.enabled = false;
        settings.enabled = false;

        SetupLowPower();

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
    
    public enum Profile
    {
        IDLE,
        NORMAL,
    }

    private void SetupLowPower()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        PlayerLoopInteraction.AddActionMap(inputAsset.FindActionMap("UI"));
        PlayerLoopManager.AddProfile(Profile.IDLE, ProfileIdle.GetProfile());
        PlayerLoopManager.AddProfile(Profile.NORMAL, ProfileNormal.GetProfile());

        PlayerLoopManager.SetActiveProfile(Profile.IDLE);
    }

    private void OnApplicationFocus(bool pFocused)
    {
        PlayerLoopManager.SetActiveProfile(Profile.NORMAL);
    }

    private void OnApplicationPause(bool pPaused)
    {
        if (!pPaused)
        {
            PlayerLoopManager.SetActiveProfile(Profile.NORMAL);
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