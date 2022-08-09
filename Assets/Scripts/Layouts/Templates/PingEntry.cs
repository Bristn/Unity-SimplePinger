using Assets.Scripts;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PingEntry : SelectableElement
{
    private VisualElement root;
    private Button rootButton;

    private Label labelTitle;
    private Label labelStatus;
    private Button buttonEdit;
    private Button buttonDelete;
    private VisualElement selectionIndicator;

    private EntryData data;
    private Action<PingEntry, EntryData> onEdit;
    private Action<PingEntry, EntryData> onDelete;
    private Action<PingEntry, EntryData> onClick;
    private Action<PingEntry, EntryData> onLongClick;
    private PingStatus status;
    private float longClickDuration;

    private LongClickElement longClickElement;
    private SelectElement selectElement;

    public enum PingStatus
    {
        INACTIVE,
        CONNECTING,
        SUCCESS,
        FAILURE
    }

    public PingEntry
        (
            VisualElement pRoot,
            EntryData pData,
            Action<PingEntry, EntryData> pOnEdit, 
            Action<PingEntry, EntryData> pOnDelete,
            Action<PingEntry, EntryData> pOnClick,
            Action<PingEntry, EntryData> pOnLongClick,
            PingStatus pStatus,
            float pLongClickDuration
        )
    {
        // Assign variables
        root = pRoot.name.Equals("root") ? pRoot : root = pRoot.Q<VisualElement>("root");
        rootButton = (Button)root;
        data = pData;
        onEdit = pOnEdit;
        onDelete = pOnDelete;
        onClick = pOnClick;
        onLongClick = pOnLongClick;
        status = pStatus;
        longClickDuration = pLongClickDuration;

        // Assign UI elements
        labelTitle = root.Q<Label>("label-title");
        labelStatus = root.Q<Label>("label-status");
        buttonEdit = root.Q<Button>("button-edit");
        buttonDelete = root.Q<Button>("button-delete");
        selectionIndicator = root.Q<VisualElement>("icon-selected");
        Title = pData.Name;
        Status = pStatus;

        // Assign button actions
        // Assign button actions
        longClickElement = new LongClickElement(root, PressedItem, PressedItemLong, longClickDuration);
        buttonEdit.clicked += PressedEdit;
        buttonEdit.clicked += longClickElement.Reset;
        buttonDelete.clicked += PressedDelete;
        buttonDelete.clicked += longClickElement.Reset;

        // Add style sheet & classes
        root.AddAllStyleSheets();

        // Selection
        selectElement = new SelectElement(selectionIndicator);
        UpdateButtonvisiblity();
    }

    private void PressedEdit() => onEdit?.Invoke(this, data);

    private void PressedDelete() => onDelete?.Invoke(this, data);

    private void PressedItem()
    {
        if (status == PingStatus.INACTIVE)
        {
            onClick?.Invoke(this, data);
        }
    }

    private void PressedItemLong()
    {
        if (status == PingStatus.INACTIVE)
        {
            onLongClick?.Invoke(this, data);
        }
    }

    public VisualElement Root => root;

    public string Title
    {
        get => labelTitle.text;
        set
        {
            labelTitle.text = value;
        }
    }

    public string FullUrl
    {
        get
        {
            if (data.Host != null && data.Suffix != null && !data.Host.Equals(string.Empty))
            {
                return data.Host + "/" + data.Suffix;
            }
            return string.Empty;
        }
    }

    public long StatusCode { get; set; }

    public PingStatus Status
    {
        get => status;
        set
        {
            status = value;
            string codeSuffix = SettingsData.Settings.ShowCode ? (" (" + StatusCode + ")") : "";

            if (status == PingStatus.INACTIVE)
            {
                labelStatus.text = "Incative";
                rootButton.SetBackgroundClass(StyleClasses.Background.NEUTRAL);
                StyleClasses.SetForegroundClass(StyleClasses.Foreground.ON_NEUTRAL, foreground);
            }
            else if (status == PingStatus.CONNECTING)
            {
                labelStatus.text = "Connecting...";
                rootButton.SetBackgroundClass(StyleClasses.Background.NEUTRAL);
                StyleClasses.SetForegroundClass(StyleClasses.Foreground.ON_NEUTRAL, foreground);
            }
            else if (status == PingStatus.SUCCESS)
            {
                labelStatus.text = "Success" + codeSuffix;
                rootButton.SetBackgroundClass(StyleClasses.Background.POSITIVE);
                StyleClasses.SetForegroundClass(StyleClasses.Foreground.ON_POSITIVE, foreground);
            }
            else if (status == PingStatus.FAILURE)
            {
                labelStatus.text = "Failed" + codeSuffix;
                rootButton.SetBackgroundClass(StyleClasses.Background.NEGATIVE);
                StyleClasses.SetForegroundClass(StyleClasses.Foreground.ON_NEGATIVE, foreground);
            }
        }
    }

    private VisualElement[] foreground 
    {
        get => new VisualElement[] { labelTitle, labelStatus, buttonDelete.Children().First(), buttonEdit.Children().First() };
    }

    public bool Selected
    {
        get => selectElement.Selected;
        set => selectElement.Selected = value;
    }

    public bool ShowIndicator
    {
        get => selectElement.ShowIndicator;
        set
        {
            selectElement.ShowIndicator = value;
            UpdateButtonvisiblity(selectElement.ShowIndicator);
        }
    }

    private void UpdateButtonvisiblity(bool pShowIndicator = false)
    {
        buttonDelete.style.display = (onDelete == null || pShowIndicator) ? DisplayStyle.None : DisplayStyle.Flex;
        buttonEdit.style.display = (onEdit == null || pShowIndicator) ? DisplayStyle.None : DisplayStyle.Flex;
    }
}
