using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class StyleClasses 
{
    //---------------------------------------------//---------------------------------------------//
    // Background
    //---------------------------------------------//---------------------------------------------//
    public enum Background
    {
        POSITIVE,
        NEGATIVE,
        NEUTRAL,
    }

    private static string[] backgroundClass = new string[]
    {
        "color-background-positive",
        "color-background-negative",
        "color-background-neutral"
    };

    public static void SetBackgroundClass(this VisualElement pElement, Background pClass)
    {
        foreach (string style in backgroundClass)
        {
            pElement.RemoveFromClassList(style);
        }

        pElement.AddToClassList(backgroundClass[(int)pClass]);
    }

    public static void SetBackgroundClass(Background pClass, params VisualElement[] pElements)
    {
        foreach (VisualElement element in pElements)
        {
            element.SetBackgroundClass(pClass);
        }
    }


    //---------------------------------------------//---------------------------------------------//
    // Foreground
    //---------------------------------------------//---------------------------------------------//
    public enum Foreground
    {
        POSITIVE,
        NEGATIVE,
        ON_POSITIVE,
        ON_NEGATIVE,
        ON_NEUTRAL,
        ON_DARK,
    }

    private static string[] foregroundClass = new string[]
    {
        "color-foreground-positive",
        "color-foreground-negative",
        "color-foreground-on-positive",
        "color-foreground-on-negative",
        "color-foreground-on-neutral",
        "color-foreground-on-dark"
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


    //---------------------------------------------//---------------------------------------------//
    // Borders
    //---------------------------------------------//---------------------------------------------//
    public enum Border
    {
        POSITIVE,
        NEGATIVE,
    }

    private static string[] borderClass = new string[]
    {
        "color-border-positive",
        "color-border-negative",
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
