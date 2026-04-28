using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class ResultsUI : MonoBehaviour
{
    public static ResultsUI Instance;

    [Header("UI References")]
    public Transform gridParent;
    public GameObject resultCardPrefab;
    public GameObject endPanel;

    [Header("Description Popup")]
    public GameObject descriptionPanel;
    public Image descImage;
    public TextMeshProUGUI descName;
    public TextMeshProUGUI descText;

    private bool hasDisplayedResults = false;

    void Awake()
    {
        // Safe singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (endPanel != null)
            endPanel.SetActive(false);

        if (descriptionPanel != null)
            descriptionPanel.SetActive(false);
    }

    public void ShowResults(List<SortingResult> results)
    {
        if (hasDisplayedResults)
        {
            return;
        }

        hasDisplayedResults = true;

        if (endPanel != null)
            endPanel.SetActive(true);

        if (gridParent == null || resultCardPrefab == null)
        {
            Debug.LogError("Missing gridParent or resultCardPrefab!");
            return;
        }

        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        foreach (SortingResult result in results)
        {
            GameObject obj = Instantiate(resultCardPrefab, gridParent);
            ResultCardUI slot = obj.GetComponent<ResultCardUI>();

            if (slot == null) continue;

            // Icon
            if (slot.icon != null)
                slot.icon.sprite = result.itemData.itemSprite;

            // Background color
            if (slot.background != null)
                slot.background.color = result.playerChoseSave ? Color.green : Color.red;

            // Correct check
            bool correct = result.playerChoseSave == result.itemData.shouldSave;

            if (slot.checkIcon != null)
                slot.checkIcon.SetActive(correct);

            if (slot.crossIcon != null)
                slot.crossIcon.SetActive(!correct);

            // Safe listener
            SortingItemData capturedData = result.itemData;

            if (slot.button != null)
            {
                slot.button.onClick.RemoveAllListeners();
                slot.button.onClick.AddListener(() =>
                {
                    ShowDescription(capturedData);
                });
            }
        }
    }

    public void ShowDescription(SortingItemData data)
    {
        if (data == null) return;

        if (descriptionPanel != null)
            descriptionPanel.SetActive(true);

        if (descImage != null)
            descImage.sprite = data.itemSprite;

        if (descName != null)
            descName.text = data.itemName;

        if (descText != null)
            descText.text = data.description;
    }

    public void CloseDescription()
    {
        if (descriptionPanel != null)
            descriptionPanel.SetActive(false);
    }

    public void ResetResultsUI()
    {
        hasDisplayedResults = false;

        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        if (endPanel != null)
            endPanel.SetActive(false);
    }
}