using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class WaterResultCardUI : MonoBehaviour, IPointerClickHandler
{
    public Image image;
    public Image background;
    public Image checkMark;
    public Image crossMark;

    private WaterResult result;

    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;

    public void Setup(WaterResult res)
    {
        result = res;
        image.sprite = res.cardData.image;

        background.color = res.playerSwipedRight ? Color.red : Color.green;

        checkMark.gameObject.SetActive(res.isCorrect);
        crossMark.gameObject.SetActive(!res.isCorrect);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        WaterInfoPanelUI.Instance.Show(result.cardData);
    }
}