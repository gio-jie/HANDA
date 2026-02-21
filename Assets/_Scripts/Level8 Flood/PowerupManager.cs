using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Powerup
{
    public string name;
    public Sprite icon;
}

public class PowerupManager : MonoBehaviour
{
    public static PowerupManager Instance;

    [Header("Assign in Inspector")]
    public Transform itemsParent;
    public GameObject powerupPanel;
    public Image powerupImage;
    public TMPro.TMP_Text powerupNameText;
    public float panelShowDuration = 1.5f;
    public AudioClip powerUpSfx;
    public AudioSource audioSource;

    private bool isShowingPowerup = false;

    [Header("Optional Message Text")]
    public TMPro.TMP_Text extraMessageText;

    [Header("All available powerups")]
    public List<Powerup> powerups;

    private List<Level8DragItem> highlightedItems = new List<Level8DragItem>();
    private Powerup pendingPowerup = null;

    void Awake() => Instance = this;

    public void QueueRandomPowerup()
    {
        if (powerups.Count == 0) return;

        pendingPowerup = powerups[Random.Range(0, powerups.Count)];
        ShowPowerupPanel(pendingPowerup);
    }

    public void ApplyPendingPowerup(string correctIDForNewCard)
    {
        if (pendingPowerup == null)
            return;

        ClearHighlights();

        switch (pendingPowerup.name)
        {
            case "Help is on the way!":
                HighlightFiveIncludingCorrect(correctIDForNewCard);
                break;

            case "Enlightenment!":
                HighlightCorrectItem(correctIDForNewCard);
                break;

            case "Danger Reveal!":
                HighlightWrongItems(correctIDForNewCard);
                break;
        }

        pendingPowerup = null;
    }

    private void HighlightFiveIncludingCorrect(string correctID)
    {
        var allItems = GetAllItems();
        Level8DragItem correctItem = allItems
            .FirstOrDefault(x => x.itemID.Equals(correctID, System.StringComparison.OrdinalIgnoreCase));

        List<Level8DragItem> selected = new List<Level8DragItem>();

        if (correctItem != null)
        {
            var pool = allItems
                .Where(x => !x.itemID.Equals(correctID, System.StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => Random.value)
                .Take(4)
                .ToList();

            selected.AddRange(pool);
            selected.Add(correctItem);
        }
        else
        {
            selected = allItems
                .OrderBy(x => Random.value)
                .Take(4)
                .ToList();

            ShowNoCorrectText();
        }

        foreach (var item in selected)
        {
            item.PlayPowerupScaleOnly();
            highlightedItems.Add(item);
        }
    }

    private void HighlightCorrectItem(string correctID)
    {
        var correctItem = GetAllItems()
            .FirstOrDefault(x => x.itemID.Equals(correctID, System.StringComparison.OrdinalIgnoreCase));

        if (correctItem != null)
        {
            correctItem.PlayPowerupCheck();
            highlightedItems.Add(correctItem);
        }
        else
        {
            ShowNoCorrectText();
        }
    }

    private void HighlightWrongItems(string correctID)
    {
        var wrongItems = GetAllItems()
            .Where(x => !x.itemID.Equals(correctID, System.StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => Random.value)
            .Take(5)
            .ToList();

        foreach (var item in wrongItems)
        {
            item.PlayPowerupCross();
            highlightedItems.Add(item);
        }
    }

    public void ClearHighlights()
    {
        foreach (var item in highlightedItems)
            item.ClearHighlight();

        highlightedItems.Clear();
    }

    private List<Level8DragItem> GetAllItems()
    {
        return itemsParent.GetComponentsInChildren<Level8DragItem>().ToList();
    }

    private void ShowNoCorrectText()
    {
        if (extraMessageText == null) return;

        extraMessageText.text = "No correct answer in your items!";
        extraMessageText.gameObject.SetActive(true);
        StarManagerLevel8.Instance.PauseTimer();
        StartCoroutine(HideExtraMessage());
    }

    private IEnumerator HideExtraMessage()
    {
        yield return new WaitForSeconds(3f);
        extraMessageText.gameObject.SetActive(false);
        StarManagerLevel8.Instance.ResumeTimer();
    }

    private void ShowPowerupPanel(Powerup powerup)
    {
        if (powerupPanel == null) return;

        Debug.Log("Powerup panel shown");
        if (isShowingPowerup) return;
        isShowingPowerup = true;

        powerupPanel.SetActive(true);
        audioSource.PlayOneShot(powerUpSfx);
        powerupImage.sprite = powerup.icon;
        powerupNameText.text = powerup.name;

        StartCoroutine(HidePanelAfterDelay(panelShowDuration));
    }

    private IEnumerator HidePanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        powerupPanel.SetActive(false);
        isShowingPowerup = false;
    }
}