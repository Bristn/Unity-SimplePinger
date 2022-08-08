using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts
{
    public static class Extensions
    {
        // https://stackoverflow.com/questions/1290603/how-to-get-the-index-of-an-element-in-an-ienumerable
        /// <summary>
        /// Returns the index of the element. Similar to List.IndexOf(). Custom implementation needed, as the Visualements' children are stored in an IEnumerable
        /// </summary>
        /// <returns>The index of the element or -1 if it's not present</returns>
        public static int IndexOf<T>(this IEnumerable<T> source, T value)
        {
            int index = 0;
            var comparer = EqualityComparer<T>.Default;
            foreach (T item in source)
            {
                if (comparer.Equals(item, value)) return index;
                index++;
            }
            return -1;
        }

        /// <summary>
        /// Replaces the given component/element with a new component/element
        /// </summary>
        public static void ReplaceWith(this VisualElement pParent, VisualElement pOld, VisualElement pNew)
        {
            int index = pParent.IndexOf(pOld);
            if (index == -1)
            {
                return;
            }

            pParent.RemoveAt(index);
            pParent.Insert(index, pNew);
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
    }
}