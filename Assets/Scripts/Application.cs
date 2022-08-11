using Assets.Scripts.PlayerLoop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static Assets.Scripts.PlayerLoop.PlayerLoopInteraction;
using static Assets.Scripts.PlayerLoop.PlayerLoopProfile;
using static UnityEngine.PlayerLoop.FixedUpdate;
using static UnityEngine.PlayerLoop.Initialization;
using static UnityEngine.PlayerLoop.PostLateUpdate;
using static UnityEngine.PlayerLoop.PreUpdate;

public class Application : MonoBehaviour
{
    public static Application Instance;

    [SerializeField] private UIDocument tabSelection;
    [SerializeField] private UIDocument entrySelection;
    [SerializeField] private UIDocument entryEditor;
    [SerializeField] private UIDocument settings;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    public static void UpdateStatusBar()
    {
        ApplicationChrome.dimmed = false;
        ApplicationChrome.statusBarState = ApplicationChrome.States.TranslucentOverContent;
        ApplicationChrome.navigationBarState = ApplicationChrome.States.Visible;
        ApplicationChrome.statusBarColor = 0x00000000;
    }

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

    public static void RunAsync(IEnumerator pRoutine)
    {
        Instance.StartCoroutine(pRoutine);
    }

    public static void StopAsync(IEnumerator pRoutine)
    {
        Instance.StopCoroutine(pRoutine);
    }

    public enum Profile
    {
        IDLE,
        NORMAL,
    }

    private void SetupLowPower()
    {
        QualitySettings.vSyncCount = 0;
        UnityEngine.Application.targetFrameRate = 30;

        PlayerLoopProfile normal = new PlayerLoopProfileBuilder()
            .TimeoutCallback(TimeoutActionActive)
            .TimeoutDuration(0.1f)
            .AdditionalSystems(LongClickManager.System, ApplicationBack.System)
            .UI(typeof(TextField), (Focusable element) => true)
            .Build();

        PlayerLoopManager.AddProfile(Profile.IDLE, ProfileIdle.GetProfile());
        PlayerLoopManager.AddProfile(Profile.NORMAL, normal);

        PlayerLoopManager.SetActiveProfile(Profile.IDLE);
    }

    private void TimeoutActionActive()
    {
        PlayerLoopManager.SetActiveProfile(Profile.IDLE);
    }

    private void OnApplicationFocus(bool pFocused)
    {
        PlayerLoopManager.SetActiveProfile(Profile.NORMAL);
    }

    private void OnApplicationPause(bool pPaused)
    {
        // TODO Fix: Once a tab file is clicked the app is trying to import the same file on each resume, even ig the app hasn't been started with an intent
        // Only occurs when user "switches tabs", once the "desktop" is opened the intent is correctly processed
        // -> Detect when the same uri is imported twice -> But check if the user has done anything which would change the outcome of importing
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