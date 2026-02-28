using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class ResultsUI : MonoBehaviour
{
    public static ResultsUI Instance;

    public Transform gridParent;
    public GameObject resultCardPrefab;
    public GameObject endPanel;

    [Header("Description Popup")]
    public GameObject descriptionPanel;
    public Image descImage;
    public TextMeshProUGUI descName;
    public TextMeshProUGUI descText;

    void Awake()
    {
        Instance = this;
    }

    public void ShowResults(List<SortingResult> results)
    {
        endPanel.SetActive(true);

        foreach (SortingResult result in results)
        {
            GameObject obj = Instantiate(resultCardPrefab, gridParent);
            ResultCardUI slot = obj.GetComponent<ResultCardUI>();

            slot.icon.sprite = result.itemData.itemSprite;

            if (result.playerChoseSave)
            {
                slot.background.color = Color.green;
            }
            else
            {
                slot.background.color = Color.red;
            }

            bool correct = result.playerChoseSave == result.itemData.shouldSave;

            slot.checkIcon.SetActive(correct);
            slot.crossIcon.SetActive(!correct);

            slot.button.onClick.AddListener(() =>
            {
                ShowDescription(result.itemData);
            });
        }
    }

    public void ShowDescription(SortingItemData data)
    {
        descriptionPanel.SetActive(true);
        descImage.sprite = data.itemSprite;
        descName.text = data.itemName;
        descText.text = data.description;
    }

    public void CloseDescription()
    {
        descriptionPanel.SetActive(false);
    }
}