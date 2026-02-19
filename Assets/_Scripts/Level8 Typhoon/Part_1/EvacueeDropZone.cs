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

    [Header("Thought Bubble UI")]
    public GameObject thoughtBubble; 
    public Image[] requestIcons;     // Ang 3 slots ng picture
    public GameObject[] checkmarks;  // Ang 3 checkmarks
    public TMP_Text thankYouText;    // Ang magpapakita ng "Thank you!"

    private List<string> possibleItems = new List<string> { "Water", "Biscuit", "Soap", "Sanitizer", "Towel" };
    private List<string> currentRequests = new List<string>();
    private List<bool> requestStatus = new List<bool>();

    void Start()
    {
        GenerateNewRequest();
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Kung tapos na ang laro, wag na tumanggap ng items
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
        // 1. Magdagdag ng score sa Manager
        if (Level8Part1Manager.instance != null) Level8Part1Manager.instance.AddScore();

        // 2. Itago ang mga pictures at checkmarks
        foreach (Image icon in requestIcons) icon.gameObject.SetActive(false);
        foreach (GameObject check in checkmarks) check.SetActive(false);
        
        // 3. Ilabas ang Thank You!
        if(thankYouText != null) 
        {
            thankYouText.gameObject.SetActive(true);
            thankYouText.text = "Thank you!";
        }

        // 4. Maghintay ng 1.5 seconds 
        yield return new WaitForSeconds(1.5f); 

        // --- DITO ANG FIX NATIN ---
        // Bago mag-generate ng bago, itanong muna kung ACTIVE pa ang laro
        if (Level8Part1Manager.instance != null && Level8Part1Manager.instance.isGameActive)
        {
            GenerateNewRequest(); // Tuloy ang ligaya
        }
        else
        {
            // KUNG TAPOS NA ANG LARO (Win or Lose):
            Debug.Log("Game Over or Win! Stopping requests.");
            
            // Itago na nang tuluyan ang buong bubble at text
            if(thoughtBubble != null) thoughtBubble.SetActive(false);
            if(thankYouText != null) thankYouText.gameObject.SetActive(false);
            
            // Dito pwede nating tawagin ang transition papuntang Phase 2 sa susunod!
        }
        // --------------------------
    }

    void GenerateNewRequest()
    {
        currentRequests.Clear();
        requestStatus.Clear();

        if(thankYouText != null) thankYouText.gameObject.SetActive(false);
        foreach (Image icon in requestIcons) icon.gameObject.SetActive(false);
        foreach (GameObject check in checkmarks) check.SetActive(false);
        
        // Siguraduhing nakalitaw ang bubble
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
            default: return null;
        }
    }
}