using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class EntrySelection : UILayout
{
    private VisualElement entryParent;
    private Button buttonAddEntry;

    private string tabName;
    private TabData tabData;
    private List<PingEntry> entries = new List<PingEntry>();

    public EntrySelection(string pTabName)
    {
        tabName = pTabName;
    }

    public override void Show()
    {
        document = Application.EntrySelection;
        document.enabled = true;

        // Assign variables
        tabData = (TabData)Persistence.LoadObjectFromJson(typeof(TabData), Persistence.TABS_FOLDER, tabName);

        // Assign UI elements
        VisualElement root = document.rootVisualElement;
        entryParent = root.Q("entry-parent");
        buttonAddEntry = root.Q<Button>("button-add-entry");

        // Assign UI Actions
        buttonAddEntry.clicked += PressedAddEntry;

        // Draw the ping entries
        foreach (EntryData data in tabData.Entries)
        {
            entries.Add(GetNewPingEntry(data));
        }

        // Create menu
        MenuItem itemBack = new MenuItemBuilder()
           .Icon(MenuItemBuilder.IconBack)
           .OnClick(PressedBack)
           .Build();

        Menu menu = new MenuBuilder()
            .MenuItems(itemBack, true)
            .Text(tabName)
            .Build();

        root.Add(menu.Root);

        // Save opened tab to settings
        SettingsData.Settings.LastTab = tabName;
    }

    private PingEntry GetNewPingEntry(EntryData pData)
    {
        PingEntry entry = new PingEntryBuilder()
            .Data(pData)
            .Status(PingEntry.PingStatus.INACTIVE)
            .OnClick(PressedEntry)
            .OnEdit(PressedEditEntry)
            .OnDelete(PressedDeleteEntry)
            .Build();
        entryParent.Insert(entryParent.childCount - 1, entry.Root);

        return entry;
    }

    private void PressedEntry(PingEntry pEntry, EntryData pData)
    {
        Application.RunAsync(pEntry.ActivateEntry());
    }

    private void PressedEditEntry(PingEntry pEntry, EntryData pData)
    {
        Hide();

        EntryEditor entryEditor = new EntryEditor();
        entryEditor.Show(pData, OnChangeEntry, OnDiscardEntryChange);
    }


    private void OnChangeEntry(EntryData pOld, EntryData pNew)
    {
        tabData.RemoveEntry(pOld);
        tabData.AddEntry(pNew);
        // entries.Add(GetNewPingEntry(pData));
        Persistence.SaveObjectToJson(tabData, Persistence.TABS_FOLDER, tabName);
        ShowOtherLayout(new EntrySelection(tabName));
    }


    private void OnDiscardEntryChange()
    {
        ShowOtherLayout(new EntrySelection(tabName));
    }

    private void PressedDeleteEntry(PingEntry pEntry, EntryData pData)
    {
        tabData.RemoveEntry(pData);
        entries.Remove(pEntry);
        entryParent.Remove(pEntry.Root);
    }

    private void PressedAddEntry()
    {
        Hide();

        EntryEditor entryEditor = new EntryEditor();
        entryEditor.Show(new EntryData(), OnSaveNewEntry, OnDiscardEntryChange);
    }

    private void OnSaveNewEntry(EntryData pOld, EntryData pNew)
    {
        tabData.AddEntry(pNew);
        entries.Add(GetNewPingEntry(pNew));
        Persistence.SaveObjectToJson(tabData, Persistence.TABS_FOLDER, tabName);
        ShowOtherLayout(new EntrySelection(tabName));
    }


    private void PressedBack()
    {
        Hide();

        TabSelection tabSelection = new TabSelection();
        tabSelection.Show();
    }
}