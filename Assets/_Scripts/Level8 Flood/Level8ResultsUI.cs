using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
        string formattedName = AddSpacesToCamelCase(itemName);

        correctItemName.text = "The correct answer is " + formattedName;
    }

    public void CloseCorrectAnswer()
    {
        correctAnswerPanel.SetActive(false);
    }

    private string AddSpacesToCamelCase(string text)
    {
        return Regex.Replace(text, "(\\B[A-Z])", " $1");
    }
}