using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionTracker 
{
    private List<SelectableElement> elements = new List<SelectableElement>();
    private HashSet<SelectableElement> selectedElements = new HashSet<SelectableElement>();
    private Action onSelectionChange;

    public SelectionTracker(Action pOnSelectionChange)
    {
        onSelectionChange = pOnSelectionChange;
    }

    public Action OnSelectionChange
    {
        get => onSelectionChange;
        set => onSelectionChange = value;
    }

    public void SelectElement(SelectableElement pElement, bool pSelected)
    {
        if (!elements.Contains(pElement))
        {
            return;
        }

        int prevCount = selectedElements.Count;

        if (pSelected)
        {
            selectedElements.Add(pElement);
            pElement.Selected = true;

            if (prevCount == 0 && selectedElements.Count > 0)
            {
                SetIndicatorsEnabled(true);
            }
        }
        else
        {
            selectedElements.Remove(pElement);
            pElement.Selected = false;

            if (prevCount > 0 && selectedElements.Count == 0)
            {
                SetIndicatorsEnabled(false);
            }
        }

        OnSelectionChange?.Invoke();
    }

    public void SetAllSelected(bool pSelected)
    {
        foreach (SelectableElement element in elements)
        {
            SelectElement(element, pSelected);
        }
    }

    public void AddElement(SelectableElement pElement) 
    { 
        elements.Add(pElement);
        pElement.ShowIndicator = HasSelection;
    }

    public void RemoveElement(SelectableElement pElement)
    {
        SelectElement(pElement, false);
        elements.Remove(pElement);
    }

    public void ReplaceElement(SelectableElement pOld, SelectableElement pNew) => elements.ReplaceElement(pOld, pNew);
    
    public IReadOnlyCollection<SelectableElement> Selection => selectedElements;

    public bool HasSelection => selectedElements.Count > 0;

    public bool FullySelected => selectedElements.Count == elements.Count;

    private void SetIndicatorsEnabled(bool pShowIndicator)
    {
        foreach (SelectableElement element in elements)
        {
            element.ShowIndicator = pShowIndicator;
        }
    }
}
