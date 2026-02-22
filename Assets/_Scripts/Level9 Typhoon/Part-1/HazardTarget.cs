using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections; // Kailangan para sa timer ng X mark

public class HazardTarget : MonoBehaviour, IDropHandler
{
    [Header("Hazard Settings")]
    public string hazardName; 
    public string requiredTool; 

    [Header("Feedback Icons")]
    public GameObject checkmarkIcon; // Ang lilitaw pag tama at maiiwan
    public GameObject xMarkIcon;     // Ang lilitaw saglit pag mali

    private bool isResolved = false; // Taga-tanda kung naayos na ba ito

    void Start()
    {
        // Siguraduhing nakatago ang mga icons sa simula ng laro
        if (checkmarkIcon != null) checkmarkIcon.SetActive(false);
        if (xMarkIcon != null) xMarkIcon.SetActive(false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Kung na-solve na ang hazard na ito, wag na pansinin ang susunod na drop
        if (isResolved) return; 

        if (eventData.pointerDrag != null)
        {
            ToolDragItem droppedTool = eventData.pointerDrag.GetComponent<ToolDragItem>();
            if (droppedTool != null)
            {
                CheckTool(droppedTool.toolType);
            }
        }
    }

    void CheckTool(string toolUsed)
    {
        if (toolUsed == requiredTool)
        {
            Debug.Log("TAMA! Ligtas na ang: " + hazardName);
            isResolved = true; 
            
            if (checkmarkIcon != null) checkmarkIcon.SetActive(true);

            // --- TAWAGIN ANG MANAGER PARA SA SCORE ---
            if (Level9Part1Manager.instance != null) Level9Part1Manager.instance.AddScore();
        }
        else
        {
            Debug.Log("MALI! Bawal gamitin ang " + toolUsed + " sa " + hazardName + "!");
            
            StartCoroutine(ShowWrongFeedback());

            // --- TAWAGIN ANG MANAGER PARA SA PENALTY ---
            if (Level9Part1Manager.instance != null) Level9Part1Manager.instance.WrongItem();
        }
    }

    // --- ANIMATION PARA SA X MARK (1 Second) ---
    IEnumerator ShowWrongFeedback()
    {
        if (xMarkIcon != null)
        {
            xMarkIcon.SetActive(true);
            yield return new WaitForSeconds(1f); // Maghintay ng 1 second
            xMarkIcon.SetActive(false);
        }
    }
}