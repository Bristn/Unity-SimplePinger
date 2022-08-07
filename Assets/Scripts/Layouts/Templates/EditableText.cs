using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EditableText : SelectableElement
{
    private VisualElement root;

    private Label labelValue;
    private Button buttonEdit;
    private Button buttonDelete;
    private VisualElement selectionIndicator;

    private Action<EditableText> onEdit;
    private Action<EditableText> onDelete;
    private Action<EditableText> onClick;
    private Action<EditableText> onLongClick;
    private float longClickDuration;

    private InputField connectedInput;
    private LongClickElement longClickElement;
    private SelectElement selectElement;

    public EditableText
        (
            VisualElement pRoot,
            string pValue,
            Action<EditableText> pOnEdit,
            Action<EditableText> pOnDelete,
            Action<EditableText> pOnClick,
            Action<EditableText> pOnLongClick,
            float pLongClickDuration
        )
    {
        // Assign variables
        onEdit = pOnEdit;
        onDelete = pOnDelete;
        onClick = pOnClick;
        onLongClick = pOnLongClick;
        longClickDuration = pLongClickDuration;

        // Assign UI elements
        root = pRoot.name.Equals("root") ? pRoot : root = pRoot.Q<VisualElement>("root");
        labelValue = root.Q<Label>("label-value");
        buttonEdit = root.Q<Button>("button-edit");
        buttonDelete = root.Q<Button>("button-delete");
        selectionIndicator = root.Q<VisualElement>("icon-selected");
        Value = pValue;

        // Assign button actions
        // longClickElement = new LongClickElement(root, PressedItem, PressedItemLong, longClickDuration);
        ((Button)root).clicked += PressedItem;

        buttonEdit.clicked += PressedEdit;
        // buttonEdit.clicked += longClickElement.Reset;
        buttonDelete.clicked += PressedDelete;
        // buttonDelete.clicked += longClickElement.Reset;

        // Selection
        selectElement = new SelectElement(selectionIndicator);
        UpdateButtonvisiblity();
    }

    private void PressedEdit()
    {
        if (connectedInput != null)
        {
            connectedInput.Value = Value;
            Root.parent.ReplaceWith(Root, connectedInput.Root);
        }
        onEdit?.Invoke(this);
    }

    private void PressedDelete() => onDelete?.Invoke(this);
    
    private void PressedItem() => onClick?.Invoke(this); 
    
    private void PressedItemLong() => onLongClick?.Invoke(this);

    public string Value
    {
        get => labelValue.text;
        set
        {
            labelValue.text = value;
        }
    }

    public VisualElement Root => root;


    public void SetConnectedInput(InputField pInput)
    {
        connectedInput = pInput;
        UpdateButtonvisiblity();
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

            if (selectElement.ShowIndicator)
            {
                buttonDelete.style.display = DisplayStyle.None;
                buttonEdit.style.display = DisplayStyle.None;
            }
            else
            {
                UpdateButtonvisiblity();
            }
        }
    }

    private void UpdateButtonvisiblity()
    {
        buttonDelete.style.display = onDelete == null ? DisplayStyle.None : DisplayStyle.Flex;
        buttonEdit.style.display = (onEdit == null && connectedInput == null) ? DisplayStyle.None : DisplayStyle.Flex;
    }
}
