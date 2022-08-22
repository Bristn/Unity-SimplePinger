using PlayerLoopProfiles;
using UnityEngine.UIElements;
using static Main;

public static class ProfileNormal
{
    public static PlayerLoopProfile GetProfile()
    {
        PlayerLoopProfile profile = new PlayerLoopProfileBuilder()
            .TimeoutCallback(Timeout)
            .TimeoutDuration(0.1f)
            .AdditionalSystems(LongClickManager.System, ApplicationBack.System)
            .ActiveUiEvaluation(typeof(TextField), EvaluateTextField)
            .Build();
        return profile;
    }

    private static bool EvaluateTextField(Focusable pTextField)
    {
        return true;
    }

    private static void Timeout()
    {
        PlayerLoopManager.SetActiveProfile(Profile.IDLE);
    }
}
