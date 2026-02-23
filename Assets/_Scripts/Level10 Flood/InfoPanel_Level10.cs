using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoPanel_Level10 : MonoBehaviour
{
    public static InfoPanel_Level10 Instance;

    public GameObject panel;
    public TMP_Text nameText;
    public Image icon;
    public TMP_Text descriptionText;

    void Awake()
    {
        Instance = this;
    }

    public void ShowInfo(string name, Sprite sprite, string desc)
    {
        panel.SetActive(true);
        nameText.text = name;
        icon.sprite = sprite;
        descriptionText.text = desc;
    }

    public void ClosePanel()
    {
        panel.SetActive(false);
    }
}