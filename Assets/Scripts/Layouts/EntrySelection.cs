using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class EntrySelection : UiScreen
{
    private VisualElement entryParent;
    private Button buttonAddEntry;

    private string tabName;
    private TabData tabData;
    private SelectionTracker selectionTracker;
    private Dictionary<EntryData, PingEntry> entries = new Dictionary<EntryData, PingEntry>();

    private MenuItem menuDelete;
    private MenuItem menuSelect;

    public EntrySelection(string pTabName)
    {
        document = Application.EntrySelection;
        tabName = pTabName;
    }

    public override void CreateMenu()
    {
        selectionTracker = new SelectionTracker(OnSelectionChange);
        menuDelete = new MenuItemBuilder()
            .Icon(UiIcons.MenuDelete)
            .OnClick(PressedDelete)
            .Build();

        menuSelect = new MenuItemBuilder()
            .Text("Select all")
            .OnClick(PressedSelectAll)
            .Build();

        MenuBuilder builder = new MenuBuilder()
            .OnClickedBack(HandleBackButtonPress)
            .ShowBackButton(true)
            .MenuItems(menuDelete)
            .MenuItems(menuSelect)
            .Text(tabName)
            .SelectionTracker(selectionTracker);
        base.CreateMenu(builder);
    }

    public override void Open()
    {
        CreateMenu();
        base.Open();

        // Assign variables
        tabData = (TabData)Persistence.LoadObjectFromJson(typeof(TabData), Persistence.TABS_FOLDER, tabName);

        // Assign UI elements
        VisualElement root = document.rootVisualElement;
        entryParent = root.Q("entry-parent");
        buttonAddEntry = root.Q<Button>("button-add-entry");

        // Assign UI Actions
        buttonAddEntry.clicked += PressedAddEntry;
        root.Add(menu.Root);

        // Draw the ping entries
        foreach (EntryData data in tabData.Entries)
        {
            PingEntry entry = GetPingEntry(data);
            selectionTracker.AddElement(entry);
            entryParent.Add(entry.Root);
        }
        OnSelectionChange();

        // Save opened tab to settings
        SettingsData.Settings.LastTab = tabName;
    }

    private PingEntry GetPingEntry(EntryData pData)
    {
        if (entries.TryGetValue(pData, out PingEntry result))
        {
            return result;
        }

        PingEntry entry = new PingEntryBuilder()
            .Data(pData)
            .Status(PingEntry.PingStatus.INACTIVE)
            .OnClick(PressedEntry)
            .OnLongClick(LongPressedEntry)
            .OnEdit(PressedEditEntry)
            .Build();
        entries.Add(pData, entry);

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

    private void PressedEditEntry(PingEntry pEntry, EntryData pData) => OpenOtherScreen(new EntryEditor(pData, OnChangeEntry, OnDiscardEntryChange), true);
    
    private void OnChangeEntry(EntryData pOld, EntryData pNew)
    {
        PingEntry entryOld = GetPingEntry(pOld);
        PingEntry entryNew = GetPingEntry(pNew);
        selectionTracker.ReplaceElement(entryOld, entryNew);
        entryParent.ReplaceWith(entryOld.Root, entryNew.Root);

        tabData.ReplaceEntry(pOld, pNew);
        Persistence.SaveObjectToJson(tabData, Persistence.TABS_FOLDER, tabName);
        Show();
    }

    private void OnDiscardEntryChange() => Show();

    private void PressedAddEntry() => OpenOtherScreen(new EntryEditor(new EntryData(), OnSaveNewEntry, OnDiscardEntryChange), true);

    private void OnSaveNewEntry(EntryData pOld, EntryData pNew)
    {
        PingEntry entry = GetPingEntry(pNew);
        selectionTracker.AddElement(entry);
        entryParent.Add(entry.Root);

        tabData.AddEntry(pNew);
        Persistence.SaveObjectToJson(tabData, Persistence.TABS_FOLDER, tabName);
        Show();
    }

    public override void HandleBackButtonPress() => OpenOtherScreen(new TabSelection());

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
            selectionTracker.RemoveElement(entry);
            entryParent.Remove(entry.Root);

            EntryData data = entries.FirstOrDefault(x => x.Value == entry).Key;
            tabData.RemoveEntry(data);
            entries.Remove(data);
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