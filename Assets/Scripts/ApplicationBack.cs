using System;
using System.Collections.Generic;
using UnityEngine.LowLevel;

public static class ApplicationBack 
{
    public static PlayerLoopSystem System = new PlayerLoopSystem()
    {
        type = typeof(ApplicationBack),
        updateDelegate = Update,
    };

    private static Dictionary<Action, bool> onBack = new Dictionary<Action, bool>();

    private static bool isPressed;

    public static bool AddCallback(Action pAction, bool pRemoveAfterInvoke = false) => onBack.TryAdd(pAction, pRemoveAfterInvoke);
    
    public static bool RemoveCallback(Action pAction) => onBack.Remove(pAction);

    public static void ClearCallbacks()
    {
        onBack.Clear();
    }

    private static void InvokeCallback()
    {
        // Use two separate lists for iterating
        // - First remove all items of the set which are flagged with the bool
        // - After that invoke the actions 
        // -> Prevents infinite loop + Modification whils enumerating if a action ends up calling this code again
        List<Action> remove = new List<Action>();
        List<Action> invoke = new List<Action>();
        foreach (var pair in onBack)
        {
            if (pair.Value)
            {
                remove.Add(pair.Key);
            }
            invoke.Add(pair.Key);
        }

        foreach (Action element in remove)
        {
            onBack.Remove(element);
        }

        foreach (Action element in invoke)
        {
            element.Invoke();
        }
    }

    public static void Update()
    {
        bool pressedBefore = isPressed;
        isPressed = UnityEngine.InputSystem.Keyboard.current.escapeKey.isPressed;
        if (!pressedBefore && isPressed)
        {
            InvokeCallback();
        }
    }

}
