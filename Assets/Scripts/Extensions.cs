using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts
{
    public static class Extensions
    {
        public static bool ReplaceElement<T>(this List<T> pCollection, T pOld, T pNew)
        {
            int index = pCollection.IndexOf(pOld);
            if (index == -1)
            {
                return false;
            }

            pCollection.RemoveAt(index);
            pCollection.Insert(index, pNew);
            return true;
        }

        /// <summary>
        /// Replaces the given component/element with a new component/element
        /// </summary>
        public static bool ReplaceWith(this VisualElement pParent, VisualElement pOld, VisualElement pNew)
        {
            int index = pParent.IndexOf(pOld);
            if (index == -1)
            {
                return false;
            }

            pParent.RemoveAt(index);
            pParent.Insert(index, pNew);
            return true;
        }


        /// <summary>
        /// Sets all four borders to the same color. Reduces amount of similar lines of code
        /// </summary>
        public static void SetBorderColor(this VisualElement pElement, Color pColor)
        {
            StyleColor color = new StyleColor(pColor); ;
            pElement.style.borderRightColor = color;
            pElement.style.borderLeftColor = color;
            pElement.style.borderTopColor = color;
            pElement.style.borderBottomColor = color;
        }


        /// <summary>
        /// Sets all four borders to the same width. Reduces amount of similar lines of code
        /// </summary>
        public static void SetBorderWidth(this VisualElement pElement, int pWidth)
        {
            pElement.style.borderRightWidth = pWidth;
            pElement.style.borderLeftWidth = pWidth;
            pElement.style.borderTopWidth = pWidth;
            pElement.style.borderBottomWidth = pWidth;
        }

        /// <summary>
        /// Sets all four paddings to the same value. Reduces amount of similar lines of code
        /// </summary>
        public static void SetPadding(this VisualElement pElement, int pPadding)
        {
            pElement.style.paddingRight = pPadding;
            pElement.style.paddingLeft = pPadding;
            pElement.style.paddingTop = pPadding;
            pElement.style.paddingBottom = pPadding;
        }


        /// <summary>
        /// Adds all stylesheets
        /// </summary>
        public static void AddAllStyleSheets(this VisualElement pElement)
        {
            pElement.styleSheets.Add(UiStyleSheets.Colors);
            pElement.styleSheets.Add(UiStyleSheets.Radius);
            pElement.styleSheets.Add(UiStyleSheets.Buttons);
            pElement.styleSheets.Add(UiStyleSheets.Borders);
            pElement.styleSheets.Add(UiStyleSheets.MenuItem);
            pElement.styleSheets.Add(UiStyleSheets.Checkbox);
            pElement.styleSheets.Add(UiStyleSheets.Texts);
        }


        public static void ShowConfirmDialog(Action pOnPositive, Action pOnNegative, string pTitle, string pMessage, string pPositive, string pNegative)
        {
            if (SystemInfo.deviceType == DeviceType.Desktop)
            {
                pOnPositive.Invoke();
                return;
            }

            Action<bool> callback = (pResult) =>
            {
                if (pResult)
                {
                    pOnPositive?.Invoke();
                }
                else
                {
                    pOnNegative?.Invoke();
                }
            };
            NativeToolkit.ShowConfirm(pTitle, pMessage, callback, pPositive, pNegative);
        }
    }
}