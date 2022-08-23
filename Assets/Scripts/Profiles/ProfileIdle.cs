using PlayerLoopProfiles;
using System;
using System.Collections.Generic;
using UnityEngine.PlayerLoop;
using static Main;
using static PlayerLoopProfiles.PlayerLoopProfile;
using static Profiles;
using static UnityEngine.PlayerLoop.FixedUpdate;
using static UnityEngine.PlayerLoop.Initialization;
using static UnityEngine.PlayerLoop.PostLateUpdate;
using static UnityEngine.PlayerLoop.PreUpdate;

public static class ProfileIdle
{
    public static PlayerLoopProfile GetProfile()
    {
        List<Type> filter = new List<Type>(new Type[]
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
        });

        PlayerLoopProfile profile = new PlayerLoopProfileBuilder()
            .FilterSystems(filter)
            .FilterType(FilterType.KEEP)
            .AdditionalSystems(LongClickManager.System, ApplicationBack.System)
            .KeepInteractionSystems(true)
            .InteractionCallback(Interaction)
            .Build();
        return profile;
    }

    private static void Interaction(string pType)
    {
        PlayerLoopManager.SetActiveProfile(ProfileType.NORMAL);
    }
}
