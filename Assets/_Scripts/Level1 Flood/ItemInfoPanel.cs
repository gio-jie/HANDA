using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInfoPanel : MonoBehaviour
{
    public static ItemInfoPanel Instance;

    public GameObject panel;
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text descriptionText;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show(ItemData item)
    {
        panel.SetActive(true);

        icon.sprite = item.icon;
        nameText.text = item.itemName;
        descriptionText.text = item.description;
    }

    public void Close()
    {
        panel.SetActive(false);
    }
}