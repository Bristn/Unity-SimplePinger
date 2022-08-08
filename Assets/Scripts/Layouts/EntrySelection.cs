using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private SelectionTracker selectionTracker;

    private Menu menu;
    private MenuItem menuDelete;
    private MenuItem menuSelect;
    private MenuItem menuBack;

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
        selectionTracker = new SelectionTracker(OnSelectionChange);

        // Assign UI elements
        VisualElement root = document.rootVisualElement;
        entryParent = root.Q("entry-parent");
        buttonAddEntry = root.Q<Button>("button-add-entry");

        // Assign UI Actions
        buttonAddEntry.clicked += PressedAddEntry;

        // Draw the ping entries
        foreach (EntryData data in tabData.Entries)
        {
            PingEntry entry = GetNewPingEntry(data);
            entries.Add(entry);
            selectionTracker.AddElement(entry);
        }

        // Create menu
        menuBack = new MenuItemBuilder()
            .Icon(UiIcons.MenuBack)
            .OnClick(PressedBack)
            .IsBackButton(true)
            .Build();

        menuDelete = new MenuItemBuilder()
            .Icon(UiIcons.MenuDelete)
            .OnClick(PressedDelete)
            .Build();

        menuSelect = new MenuItemBuilder()
            .Text("Select all")
            .OnClick(PressedSelectAll)
            .Build();

        menu = new MenuBuilder()
            .BackButton(menuBack)
            .MenuItems(menuDelete)
            .MenuItems(menuSelect)
            .Text(tabName)
            .SelectionTracker(selectionTracker)
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
            .OnLongClick(LongPressedEntry)
            .OnEdit(PressedEditEntry)
            .Build();
        entryParent.Add(entry.Root);

        return entry;
    }

    private void PressedEntry(PingEntry pEntry, EntryData pData)
    {
        if (!selectionTracker.HasSelection)
        {
            Application.RunAsync(pEntry.ActivateEntry());
        }
        else
        {
            selectionTracker.SelectElement(pEntry, !pEntry.Selected);
        }
    }

    private void LongPressedEntry(PingEntry pEntry, EntryData pData) => selectionTracker.SelectElement(pEntry, !pEntry.Selected);

    private void PressedEditEntry(PingEntry pEntry, EntryData pData) => ShowOtherLayout(new EntryEditor(pData, OnChangeEntry, OnDiscardEntryChange));
    
    private void OnChangeEntry(EntryData pOld, EntryData pNew)
    {
        tabData.RemoveEntry(pOld);
        tabData.AddEntry(pNew);
        Persistence.SaveObjectToJson(tabData, Persistence.TABS_FOLDER, tabName);
        ShowOtherLayout(new EntrySelection(tabName));
    }

    private void OnDiscardEntryChange() => ShowOtherLayout(new EntrySelection(tabName));

    private void PressedAddEntry() => ShowOtherLayout(new EntryEditor(new EntryData(), OnSaveNewEntry, OnDiscardEntryChange));

    private void OnSaveNewEntry(EntryData pOld, EntryData pNew)
    {
        tabData.AddEntry(pNew);
        entries.Add(GetNewPingEntry(pNew));
        Persistence.SaveObjectToJson(tabData, Persistence.TABS_FOLDER, tabName);
        ShowOtherLayout(new EntrySelection(tabName));
    }

    private void PressedBack() => ShowOtherLayout(new TabSelection());

    private void PressedSelectAll()
    {
        selectionTracker.SetAllSelected(true);
        menu.CloseMenu();
    }

    private void PressedDelete()
    {
        string title = "Delete entries?";
        string message = "This is going to delete the selected entries permanently. Do yo wish to continue?";
        string positive = "Delete";
        string negative = "Cancel";
        Extensions.ShowConfirmDialog(DeleteSelectedEntries, null, title, message, positive, negative);
    }

    private void DeleteSelectedEntries()
    {
        foreach (SelectableElement element in selectionTracker.Selection.ToList())
        {
            PingEntry entry = (PingEntry)element;
            entries.Remove(entry);
            selectionTracker.RemoveElement(entry);
            entryParent.Remove(entry.Root);
            // TODO: Delete entry from tab data
            Persistence.SaveObjectToJson(tabData, Persistence.TABS_FOLDER, tabName);
        }
    }

    private void OnSelectionChange()
    {
        buttonAddEntry.style.display = selectionTracker.HasSelection ? DisplayStyle.None : DisplayStyle.Flex;
        menu.SetMenuItemVisible(menuDelete, selectionTracker.HasSelection);
        menu.SetMenuItemVisible(menuSelect, !selectionTracker.FullySelected);
    }
}