using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Level8ResultsUI : MonoBehaviour
{
    public static Level8ResultsUI Instance;

    [Header("End Panel Grid")]
    public Transform gridParent;
    public GameObject resultSlotPrefab;
    public GameObject endPanel;

    [Header("Correct Answer Panel")]
    public GameObject correctAnswerPanel;
    public Image correctItemImage;
    public TMPro.TMP_Text correctItemName;

    void Awake() => Instance = this;

    public void ShowResults(List<Level8AnswerRecord> answers)
    {
        endPanel.SetActive(true);

        foreach (Transform child in gridParent)
            Destroy(child.gameObject);

        foreach (var record in answers)
        {
            GameObject obj = Instantiate(resultSlotPrefab, gridParent);
            EndAnswerSlotUI slot = obj.GetComponent<EndAnswerSlotUI>();

            slot.Setup(
                record.scenarioImage,
                record.playerItemSprite,
                record.isCorrect,
                record.correctItemSprite,
                record.correctItemName
            );
        }
    }

    public void ShowCorrectAnswer(Sprite correctSprite, string itemName)
    {
        correctAnswerPanel.SetActive(true);
        correctItemImage.sprite = correctSprite;
        correctItemName.text = "The correct answer is " + itemName;
    }

    public void CloseCorrectAnswer()
    {
        correctAnswerPanel.SetActive(false);
    }
}