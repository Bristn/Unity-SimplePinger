using UnityEngine;
using UnityEngine.UIElements;

public class UiStyleSheets : MonoBehaviour
{
    private static UiStyleSheets instance;

    void Awake()
    {
        instance = this;
    }

    [Header("Gneral")]

    [SerializeField] private StyleSheet colors;

    [SerializeField] private StyleSheet radius;

    [SerializeField] private StyleSheet buttons;

    [SerializeField] private StyleSheet borders;

    [SerializeField] private StyleSheet texts;

    public static StyleSheet Colors => instance.colors;

    public static StyleSheet Radius => instance.radius;

    public static StyleSheet Buttons => instance.buttons;

    public static StyleSheet Borders => instance.borders;

    public static StyleSheet Texts => instance.texts;


    [Header("Unique")]

    [SerializeField] private StyleSheet menuItem;

    [SerializeField] private StyleSheet checkbox;

    public static StyleSheet MenuItem => instance.menuItem;

    public static StyleSheet Checkbox => instance.checkbox;
}
