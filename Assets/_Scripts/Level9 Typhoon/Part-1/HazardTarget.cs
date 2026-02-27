using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections; 
using System.Collections.Generic; // DAGDAG: Kailangan ito para makagamit tayo ng "List"

public class HazardTarget : MonoBehaviour, IDropHandler
{
    [Header("Hazard Settings")]
    public string hazardName; 
    
    // --- ITO ANG BINAGO ---
    // Ginawa nating List para pwede kang mag-add ng dalawa o higit pang tools sa Inspector
    public List<string> acceptedTools = new List<string>(); 

    [Header("Feedback Icons")]
    public GameObject checkmarkIcon; 
    public GameObject xMarkIcon;     

    private bool isResolved = false; 

    void Start()
    {
        if (checkmarkIcon != null) checkmarkIcon.SetActive(false);
        if (xMarkIcon != null) xMarkIcon.SetActive(false);
    }

    public void OnDrop(PointerEventData eventData)
    {
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
        // --- ITO ANG BINAGO ---
        // Iche-check niya ngayon kung yung ginamit na tool ay nasa loob ng listahan mo
        if (acceptedTools.Contains(toolUsed))
        {
            Debug.Log("TAMA! Ligtas na ang: " + hazardName);
            isResolved = true; 
            
            if (checkmarkIcon != null) checkmarkIcon.SetActive(true);

            if (Level9Part1Manager.instance != null) Level9Part1Manager.instance.AddScore();
        }
        else
        {
            Debug.Log("MALI! Bawal gamitin ang " + toolUsed + " sa " + hazardName + "!");
            
            StartCoroutine(ShowWrongFeedback());

            if (Level9Part1Manager.instance != null) Level9Part1Manager.instance.WrongItem();
        }
    }

    IEnumerator ShowWrongFeedback()
    {
        if (xMarkIcon != null)
        {
            xMarkIcon.SetActive(true);
            yield return new WaitForSeconds(1f); 
            xMarkIcon.SetActive(false);
        }
    }
}