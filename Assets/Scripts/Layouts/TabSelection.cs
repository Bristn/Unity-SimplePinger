using Assets.Scripts;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using static InputField;

public class TabSelection : UiScreen
{
    private VisualElement tabParent;
    private Button buttonAddTab;

    private List<string> tabNames;
    private List<EditableText> tabEntries = new List<EditableText>();
    private SelectionTracker selectionTracker;

    private MenuItem menuDelete;
    private MenuItem menuSelect;
    private MenuItem menuExport;
    private MenuItem menuSettings;

    public TabSelection()
    {
        document = Main.TabSelection;
        tabNames = TabPersistence.GetAllTabNames();
        SettingsData.Settings.LastTab = string.Empty;
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

        menuExport = new MenuItemBuilder()
            .Text("Export selected")
            .OnClick(PressedExport)
            .Build();

        menuSettings = new MenuItemBuilder()
            .Text("Settings")
            .OnClick(PressedMenu)
            .Build();

        MenuBuilder builder = new MenuBuilder()
            .MenuItems(menuDelete)
            .MenuItems(menuSelect)
            .MenuItems(menuExport)
            .MenuItems(menuSettings)
            .Text("Tab selection")
            .SelectionTracker(selectionTracker)
            .ShowBackButton(false);
        base.CreateMenu(builder);
    }

    public override void Open()
    {
        CreateMenu();
        base.Open();

        // Assign UI elements
        VisualElement root = document.rootVisualElement;
        tabParent = root.Q("tab-parent");
        buttonAddTab = root.Q<Button>("button-add-tab");

        // Assign UI Actions
        buttonAddTab.clicked += PressedAddTab;
        root.Add(menu.Root);

        // Draw the tab entries
        foreach (string name in tabNames)
        {
            EditableText entry = GetNewTabEntry(name);
            tabEntries.Add(entry);
            selectionTracker.AddElement(entry);
        }
        OnSelectionChange();
    }

    private void PressedAddTab() 
    {
        InputField input = new InputFieldBuilder()
            .SaveButtonVisiblity(ValidType.VALID, true)
            .DiscardButtonVisiblity(ValidType.VALID, true)
            .DiscardButtonVisiblity(ValidType.INVALID, true)
            .OnSave(OnSaveNewTab)
            .OnDiscard(OnDiscardNewTab)
            .InvalidValues(tabNames)
            .Value(string.Empty)
            .Placeholder("Enter tab name ...")
            .Hint("Tab name")
            .Build();

        tabParent.Add(input.Root);
        buttonAddTab.style.display = DisplayStyle.None;
    }

    private EditableText GetNewTabEntry(string pName)
    {
        EditableText entry = new EditableTextBuilder()
            .Value(pName)
            .OnClick(PressedTab)
            .OnLongClick(LongPressedTab)
            .Build();

        InputField input = new InputFieldBuilder()
            .SaveButtonVisiblity(ValidType.VALID, true)
            .DiscardButtonVisiblity(ValidType.VALID, true)
            .DiscardButtonVisiblity(ValidType.INVALID, true)
            .OnSave(OnChangeTab)
            .InvalidValues(tabNames)
            .Placeholder("Enter tab name ...")
            .Hint("Tab name")
            .Build();

        entry.SetConnectedInput(input);
        input.SetConnectedText(entry);
        tabParent.Add(entry.Root);

        return entry;
    }

    private void OnSaveNewTab(InputField pEntry, string pFrom, string pTo)
    {
        if (!pTo.Equals(string.Empty))
        {
            EditableText entry = GetNewTabEntry(pTo);
            tabEntries.Add(entry);
            selectionTracker.AddElement(entry);
            tabNames.Add(pTo);
            Persistence.SaveObjectToJson("", Persistence.TABS_FOLDER, pTo);
        }

        OnDiscardNewTab(pEntry);
    }

    private void OnDiscardNewTab(InputField pEntry)
    {
        tabParent.Remove(pEntry.Root);
        buttonAddTab.style.display = DisplayStyle.Flex;
    }

    private void OnChangeTab(InputField pEntry, string pFrom, string pTo)
    {
        if (!pFrom.Equals(pTo))
        {
            tabNames.Remove(pFrom);
            tabNames.Add(pTo);
            TabPersistence.RenameTab(pFrom, pTo);
        }
    }

    private void PressedMenu() => OpenOtherScreen(new ApplicationSettings());

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
            EditableText entry = (EditableText)element;
            tabNames.Remove(entry.Value);
            tabEntries.Remove(entry);
            selectionTracker.RemoveElement(entry);
            tabParent.Remove(entry.Root);
            TabPersistence.DeleteTab(entry.Value);
        }
    }

    private void PressedExport()
    {
        List<string> tabNames = new List<string>();
        foreach (SelectableElement element in selectionTracker.Selection.ToList())
        {
            EditableText entry = (EditableText)element;
            tabNames.Add(entry.Value);
        }

        TabPersistence.ExportTabs(ExportCallback, tabNames.ToArray());
    }

    private void ExportCallback(NativeShare.ShareResult pResult, string pShareTarget)
    {
        selectionTracker.SetAllSelected(false);
        menu.CloseMenu();
    }

    private void PressedTab(EditableText pEntry)
    {
        if (!selectionTracker.HasSelection) {
            OpenOtherScreen(new EntrySelection(pEntry.Value));
        }
        else
        {
            selectionTracker.SelectElement(pEntry, !pEntry.Selected);
        }
    }

    private void LongPressedTab(EditableText pEntry) => selectionTracker.SelectElement(pEntry, !pEntry.Selected);
    
    private void OnSelectionChange()
    {
        buttonAddTab.style.display = selectionTracker.HasSelection ? DisplayStyle.None : DisplayStyle.Flex;
        menu.SetMenuItemVisible(menuDelete, selectionTracker.HasSelection);
        menu.SetMenuItemVisible(menuSelect, !selectionTracker.FullySelected);
        menu.SetMenuItemVisible(menuExport, selectionTracker.HasSelection);
    }

    public override void HandleBackButtonPress() => UnityEngine.Application.Quit();
}
