using PlayerLoopProfiles;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public static class Profiles 
{
    public enum ProfileType
    {
        IDLE,
        NORMAL,
    }

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

    public static void SetupProfiles(InputActionMap pMap)
    {
        PlayerLoopInteraction.AddActionMap(pMap);
        PlayerLoopManager.AddProfile(ProfileType.IDLE, ProfileIdle.GetProfile());
        PlayerLoopManager.AddProfile(ProfileType.NORMAL, ProfileNormal.GetProfile());
        PlayerLoopManager.SetActiveProfile(ProfileType.IDLE);
    }

    public static void OnApplicationFocus(bool pFocused)
    {
        PlayerLoopManager.SetActiveProfile(ProfileType.NORMAL);
    }

    public static void OnApplicationPause(bool pPaused)
    {
        if (!pPaused)
        {
            PlayerLoopManager.SetActiveProfile(ProfileType.NORMAL);
        }
    }
}
