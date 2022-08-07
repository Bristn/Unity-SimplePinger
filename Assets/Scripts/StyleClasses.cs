using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class StyleClasses 
{
    public enum Foreground
    {
        POSITIVE,
        NEGATIVE,
        ON_POSITIVE,
        ON_NEGATIVE,
        ON_NEUTRAL,
    }

    private static string[] foregroundClass = new string[]
    {
        "foreground-positive",
        "foreground-negative",
        "foreground-on-positive",
        "foreground-on-negative",
        "foreground-on-neutral"
    };

    public static void SetForegroundClass(this VisualElement pElement, Foreground pClass)
    {
        foreach (string style in foregroundClass)
        {
            pElement.RemoveFromClassList(style);
        }

        pElement.AddToClassList(foregroundClass[(int)pClass]);
    }

    public static void SetForegroundClass(Foreground pClass, params VisualElement[] pElements)
    {
        foreach (VisualElement element in pElements)
        {
            element.SetForegroundClass(pClass);
        }
    }

    public enum Border
    {
        POSITIVE,
        NEGATIVE,
    }

    private static string[] borderClass = new string[]
    {
        "border-positive",
        "border-negative",
    };

    public static void SetBorderClass(this VisualElement pElement, Border pClass)
    {
        foreach (string style in borderClass)
        {
            pElement.RemoveFromClassList(style);
        }

        pElement.AddToClassList(borderClass[(int)pClass]);
    }
}
