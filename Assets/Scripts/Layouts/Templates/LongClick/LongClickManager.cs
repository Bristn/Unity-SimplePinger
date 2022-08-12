using System.Collections.Generic;
using UnityEngine.LowLevel;

public static class LongClickManager 
{
    public static PlayerLoopSystem System { get; private set; } = new PlayerLoopSystem()
    {
        type = typeof(LongClickManager),
        updateDelegate = Update
    };

    private static HashSet<LongClickElement> elements = new HashSet<LongClickElement>();

    private static void Update()
    {
        foreach (LongClickElement element in elements)
        {
            element.Update();
        }
    }

    public static void AddElement(LongClickElement pElement) => elements.Add(pElement);

    public static void removeElement(LongClickElement pElement) => elements.Remove(pElement);
}
