using UnityEngine;
using UnityEngine.UI;

public class EarthquakeResult : MonoBehaviour
{
    [Header("UI")]
    public Image answerImage;
    public GameObject checkMark;
    public GameObject crossMark;

    private PlayerAnswerData data;

    // =========================
    // SETUP ITEM
    // =========================
    public void Setup(PlayerAnswerData answerData)
    {
        data = answerData;

        answerImage.sprite = answerData.chosenImage;

        checkMark.SetActive(answerData.isCorrect);
        crossMark.SetActive(!answerData.isCorrect);
    }

    // =========================
    // CLICK
    // =========================
    public void OnClick()
    {
        ResultDetails.Instance.Show(data);
    }
}