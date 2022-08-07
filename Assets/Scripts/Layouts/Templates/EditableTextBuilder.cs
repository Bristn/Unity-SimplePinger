using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EditableTextBuilder : MonoBehaviour
{
    // Singleton
    private static EditableTextBuilder instance;

    [SerializeField] private VisualTreeAsset prefab;

    private void Awake()
    {
        instance = this;
    }

    public static VisualTreeAsset Prefab => instance.prefab;


    // Actual builder pattern
    private VisualElement root = null;
    private string value = "";
    private Action<EditableText> onEdit = null;
    private Action<EditableText> onDelete = null;
    private Action<EditableText> onClick = null;
    private Action<EditableText> onLongClick = null;
    private float longClickDuration = 0.5f;

    public EditableTextBuilder Root(VisualElement pRoot)
    {
        root = pRoot;
        return this;
    }

    public EditableTextBuilder Value(string pValue)
    {
        value = pValue;
        return this;
    }

    public EditableTextBuilder OnEdit(Action<EditableText> pOnEdit)
    {
        onEdit = pOnEdit;
        return this;
    }

    public EditableTextBuilder OnDelete(Action<EditableText> pOnDelete)
    {
        onDelete = pOnDelete;
        return this;
    }

    public EditableTextBuilder OnClick(Action<EditableText> pOnclick)
    {
        onClick = pOnclick;
        return this;
    }

    public EditableTextBuilder OnLongClick(Action<EditableText> pOnLongClick)
    {
        onLongClick = pOnLongClick;
        return this;
    }

    public EditableTextBuilder LongClickduration(float pLongclickDuration)
    {
        longClickDuration = pLongclickDuration;
        return this;
    }


    // Build
    public EditableText Build()
    {
        // If no root is specified instantiate the prefab
        if (root == null)
        {
            root = Prefab.Instantiate().Q<VisualElement>("root");
        }

        return new EditableText
            (
                root,
                value,
                onEdit,
                onDelete,
                onClick,
                onLongClick,
                longClickDuration
            );
    }
}
