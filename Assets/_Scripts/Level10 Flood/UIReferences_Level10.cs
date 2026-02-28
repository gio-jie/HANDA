using UnityEngine;
using UnityEngine.UI;

public class UIReferences_Level10 : MonoBehaviour
{
    public static UIReferences_Level10 Instance;

    [Header("UI References")]
    public Transform trashIcon;

    [Header("Main Canvas")]
    public Canvas mainCanvas; // <-- Add this line

    private void Awake()
    {
        Instance = this;
    }
}