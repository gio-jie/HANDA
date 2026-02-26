using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class EvacueeDropZone : MonoBehaviour, IDropHandler
{
    [Header("Item Sprites (I-drag ang pictures dito)")]
    public Sprite waterSprite;
    public Sprite biscuitSprite;
    public Sprite soapSprite;
    public Sprite sanitizerSprite;
    public Sprite towelSprite;
    public Sprite betadineSprite;
    public Sprite cottonSprite;
    public Sprite medicineSprite;
    
    [Header("Thought Bubble UI")]
    public GameObject thoughtBubble; 
    public Image[] requestIcons;     // Ang 3 slots ng picture
    public GameObject[] checkmarks;  // Ang 3 checkmarks
    public TMP_Text thankYouText;    // Ang magpapakita ng "Thank you!"

    private List<string> possibleItems = new List<string> { "Water", "Biscuit", "Soap", "Sanitizer", "Towel", "Betadine", "Cotton", "Medicine" };
    private List<string> currentRequests = new List<string>();
    private List<bool> requestStatus = new List<bool>();

    void Start()
    {
        GenerateNewRequest();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (Level8Part1Manager.instance != null && !Level8Part1Manager.instance.isGameActive) return;

        if (eventData.pointerDrag != null)
        {
            RationDragItem droppedItem = eventData.pointerDrag.GetComponent<RationDragItem>();
            if (droppedItem != null)
            {
                ProcessItem(droppedItem.itemType);
            }
        }
    }

    void ProcessItem(string type)
    {
        bool matched = false;

        // --- DITO YUNG NAWALA KANINA! Binalik ko na para mag-match ulit ---
        for (int i = 0; i < currentRequests.Count; i++)
        {
            if (currentRequests[i] == type && requestStatus[i] == false)
            {
                requestStatus[i] = true;
                if(checkmarks[i] != null) checkmarks[i].SetActive(true); 
                matched = true;
                Debug.Log("Tama! Nabigay ang " + type);
                break; 
            }
        }
        // ------------------------------------------------------------------

        if (matched)
        {
            CheckIfComplete();
        }
        else
        {
            Debug.Log("Mali! Penalty!");
            if (Level8Part1Manager.instance != null)
            {
                Level8Part1Manager.instance.currentPower -= 10f;
                // TAWAGIN ANG BAGONG VISUAL FEEDBACK
                Level8Part1Manager.instance.TriggerPenaltyFeedback();
            }
        }
    }

    void CheckIfComplete()
    {
        bool allDone = true;
        foreach (bool status in requestStatus)
        {
            if (status == false) allDone = false;
        }

        if (allDone)
        {
            StartCoroutine(CompleteRequestRoutine());
        }
    }

    IEnumerator CompleteRequestRoutine()
    {
        if (Level8Part1Manager.instance != null) Level8Part1Manager.instance.AddScore();

        foreach (Image icon in requestIcons) icon.gameObject.SetActive(false);
        foreach (GameObject check in checkmarks) check.SetActive(false);
        
        if(thankYouText != null) 
        {
            thankYouText.gameObject.SetActive(true);
            thankYouText.text = "Thank you!";
        }

        yield return new WaitForSeconds(1.5f); 

        if (Level8Part1Manager.instance != null && Level8Part1Manager.instance.isGameActive)
        {
            GenerateNewRequest(); 
        }
        else
        {
            Debug.Log("Game Over or Win! Stopping requests.");
            if(thoughtBubble != null) thoughtBubble.SetActive(false);
            if(thankYouText != null) thankYouText.gameObject.SetActive(false);
        }
    }

    void GenerateNewRequest()
    {
        currentRequests.Clear();
        requestStatus.Clear();

        if(thankYouText != null) thankYouText.gameObject.SetActive(false);
        foreach (Image icon in requestIcons) icon.gameObject.SetActive(false);
        foreach (GameObject check in checkmarks) check.SetActive(false);
        
        if(thoughtBubble != null) thoughtBubble.SetActive(true);

        int itemsToRequest = Random.Range(1, 4); 

        for (int i = 0; i < itemsToRequest; i++)
        {
            string randomItem = possibleItems[Random.Range(0, possibleItems.Count)];
            currentRequests.Add(randomItem);
            requestStatus.Add(false); 

            if(i < requestIcons.Length)
            {
                requestIcons[i].gameObject.SetActive(true);
                requestIcons[i].sprite = GetSpriteFor(randomItem);
            }
        }
    }

    Sprite GetSpriteFor(string itemName)
    {
        switch (itemName)
        {
            case "Water": return waterSprite;
            case "Biscuit": return biscuitSprite;
            case "Soap": return soapSprite;
            case "Sanitizer": return sanitizerSprite;
            case "Towel": return towelSprite;
            case "Betadine": return betadineSprite;
            case "Cotton": return cottonSprite;
            case "Medicine": return medicineSprite;
            default: return null;
        }
    }
}