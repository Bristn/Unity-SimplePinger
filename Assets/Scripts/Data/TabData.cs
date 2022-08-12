using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TabData 
{
    [SerializeField] private List<EntryData> entries = new List<EntryData>();

    public void AddEntry(EntryData pEntry, int pIndex = -1)
    {
        if (pIndex == -1 || pIndex >= entries.Count)
        {
            entries.Add(pEntry);
        }
        else
        {
            entries.Insert(pIndex, pEntry);
        }
    }

    public void RemoveEntry(EntryData pEntry)
    {
        entries.Remove(pEntry);
    }

    public void ReplaceEntry(EntryData pOld, EntryData pNew) =>  entries.ReplaceElement(pOld, pNew);

    public IReadOnlyList<EntryData> Entries => entries;
}
