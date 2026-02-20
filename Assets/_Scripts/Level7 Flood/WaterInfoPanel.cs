using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaterInfoPanelUI : MonoBehaviour
{
    public static WaterInfoPanelUI Instance;

    public GameObject panel;
    public Image image;
    public TMP_Text title;
    public TMP_Text description;

    void Awake() { Instance = this; }

    public void Show(WaterCardData data)
    {
        panel.SetActive(true);
        image.sprite = data.image;
        title.text = data.waterName;
        description.text = data.description;
    }

    public void Hide() => panel.SetActive(false);
}