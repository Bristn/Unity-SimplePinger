using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PingEntryBuilder : MonoBehaviour
{
    // Singleton
    private static PingEntryBuilder instance;

    [SerializeField] private VisualTreeAsset prefab;

    private void Awake()
    {
        instance = this;
    }

    public static VisualTreeAsset Prefab => instance.prefab;


    // Actual builder pattern
    private VisualElement root;
    private EntryData data;
    private Action<PingEntry, EntryData> onEdit;
    private Action<PingEntry, EntryData> onDelete;
    private Action<PingEntry, EntryData> onClick;
    private Action<PingEntry, EntryData> onLongClick;
    private PingEntry.PingStatus status;
    private float longClickDuration = 0.5f;

    public PingEntryBuilder Root(VisualElement pRoot)
    {
        root = pRoot;
        return this;
    }

    public PingEntryBuilder Data(EntryData pData)
    {
        data = pData;
        return this;
    }

    public PingEntryBuilder OnEdit(Action<PingEntry, EntryData> pOnEdit)
    {
        onEdit = pOnEdit;
        return this;
    }

    public PingEntryBuilder OnDelete(Action<PingEntry, EntryData> pOnDelete)
    {
        onDelete = pOnDelete;
        return this;
    }

    public PingEntryBuilder OnClick(Action<PingEntry, EntryData> pOnClick)
    {
        onClick = pOnClick;
        return this;
    }

    public PingEntryBuilder OnLongClick(Action<PingEntry, EntryData> pOnLongClick)
    {
        onLongClick = pOnLongClick;
        return this;
    }

    public PingEntryBuilder Status(PingEntry.PingStatus pStatus)
    {
        status = pStatus;
        return this;
    }

    public PingEntryBuilder LongClickduration(float pLongclickDuration)
    {
        longClickDuration = pLongclickDuration;
        return this;
    }


    // Build
    public PingEntry Build()
    {
        // If no root is specified instantiate the prefab
        if (root == null)
        {
            root = Prefab.Instantiate().Q<VisualElement>("root");
        }

        return new PingEntry
            (
                root,
                data,
                onEdit,
                onDelete,
                onClick,
                onLongClick,
                status,
                longClickDuration
            );
    }
}
