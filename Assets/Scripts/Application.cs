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

    [SerializeField] private StyleSheet styleCheckbox;
    public static StyleSheet StyleCheckbox => Instance.styleCheckbox;

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
        TabPersistence.ImportTab();

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

    private void Update()
    {
        Debug.Log(UnityEngine.InputSystem.Keyboard.current.escapeKey.isPressed);
        if (UnityEngine.InputSystem.Keyboard.current.escapeKey.isPressed)
        {
            UnityEngine.Application.Quit();
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

    public enum Profile
    {
        IDLE,
        NORMAL,
    }

    private void SetupLowPower()
    {
        QualitySettings.vSyncCount = 0;
        UnityEngine.Application.targetFrameRate = 30;

        List<Type> idleFilter = new List<Type>(new Type[]
        {
                // Keep, as causes higher CPU usage when removed
                typeof(TimeUpdate),

#if !UNITY_ANDROID || UNITY_EDITOR
                // Causese: GfxDeviceD3D11Base::WaitForLastPresentationAndGetTimestamp() was called multiple times in a row without calling GfxDeviceD3D11Base::PresentFrame(). This may result in a deadlock.
                typeof(PresentAfterDraw),
#endif

                // Keep Profiler for debugging
                typeof(ProfilerStartFrame),
                typeof(ProfilerSynchronizeStats),
                typeof(ProfilerEndFrame),

                // input Test
                typeof(SynchronizeInputs),
                typeof(EarlyUpdate.UpdateInputManager),
                typeof(EarlyUpdate.ProcessRemoteInput),
                typeof(NewInputFixedUpdate),
                typeof(CheckTexFieldInput),
                typeof(NewInputUpdate),
                typeof(InputEndFrame),
                typeof(ResetInputAxis),
        });

        PlayerLoopProfile idle = new PlayerLoopProfileBuilder()
            .FilterSystems(idleFilter)
            .FilterType(FilterType.KEEP)
            .AdditionalSystems(LongClickManager.System)
            .InteractionCallback(InteractionActionIdle)
            .Build();

        PlayerLoopProfile normal = new PlayerLoopProfileBuilder()
            .TimeoutCallback(TimeoutActionActive)
            .TimeoutDuration(0.1f)
            .AdditionalSystems(LongClickManager.System)
            .UI(typeof(TextField), (Focusable element) => true)
            .Build();

        PlayerLoopManager.AddProfile(Profile.IDLE, idle);
        PlayerLoopManager.AddProfile(Profile.NORMAL, normal);

        PlayerLoopManager.SetActiveProfile(Profile.IDLE);
    }

    private void InteractionActionIdle(InteractionType pType)
    {
        PlayerLoopManager.SetActiveProfile(Profile.NORMAL);
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
        PlayerLoopManager.SetActiveProfile(Profile.NORMAL);
        Persistence.SaveObjectToJson(SettingsData.Settings, "", Persistence.SETTING_FILE);
    }

    private void OnApplicationQuit()
    {
        Persistence.SaveObjectToJson(SettingsData.Settings, "", Persistence.SETTING_FILE);
    }
}