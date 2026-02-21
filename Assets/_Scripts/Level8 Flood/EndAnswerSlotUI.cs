using UnityEngine;
using UnityEngine.UI;

public class EndAnswerSlotUI : MonoBehaviour
{
    public Image scenarioImage;
    public Image playerItemImage;
    public Image checkIcon;
    public Image crossIcon;
    public Button button;

    private Sprite correctItemSprite;
    private string correctItemName;

    public void Setup(Sprite scenario, Sprite playerSprite, bool isCorrect, Sprite correctSprite, string correctName)
    {
        scenarioImage.sprite = scenario;
        playerItemImage.sprite = playerSprite;

        checkIcon.gameObject.SetActive(isCorrect);
        crossIcon.gameObject.SetActive(!isCorrect);

        correctItemSprite = correctSprite;
        correctItemName = correctName;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            Level8ResultsUI.Instance.ShowCorrectAnswer(correctItemSprite, correctItemName);
        });
    }
}