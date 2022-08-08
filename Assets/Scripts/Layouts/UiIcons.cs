using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UiIcons : MonoBehaviour
{
    private static UiIcons instance;

    void Awake()
    {
        instance = this;
    }


    [Header("Input Field")]

    [SerializeField] private VectorImage inputFieldValid;

    [SerializeField] private VectorImage inputFieldInvalid;

    public static VectorImage InputFieldValid => instance.inputFieldValid;

    public static VectorImage InputFieldInvalid => instance.inputFieldInvalid;


    [Header("Selction")]

    [SerializeField] private VectorImage selectionActive;

    [SerializeField] private VectorImage selectionInactive;

    public static VectorImage SelectionActive => instance.selectionActive;

    public static VectorImage SelectionInactive => instance.selectionInactive;


    [Header("Menu")]

    [SerializeField] private VectorImage menuMore;

    [SerializeField] private VectorImage menuBack;

    [SerializeField] private VectorImage menuDelete;

    public static VectorImage MenuMore => instance.menuMore;

    public static VectorImage MenuBack => instance.menuBack;

    public static VectorImage MenuDelete => instance.menuDelete;
}
