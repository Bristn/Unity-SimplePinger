using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntryData
{
    [SerializeField] private string name = "";
    [SerializeField] private string host = "";
    [SerializeField] private string suffix = "";

    public string Name
    {
        get => name;
        set => name = value;
    }

    public string Host
    {
        get => host;
        set => host = value;
    }

    public string Suffix
    {
        get => suffix;
        set => suffix = value;
    }
}
