using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections; // Kailangan para sa timer ng Check/X marks

public class CleaningTarget : MonoBehaviour, IDropHandler
{
    [Header("Cleaning Settings")]
    public string targetName; 
    public string requiredTool; 
    
    [Header("Safety Rule")]
    public bool requiresBoots = true; 
    public bool isPlayerTarget = false; 
    public bool isFinalDisinfect = false; 

    [Header("Feedback Icons")]
    public GameObject checkmarkIcon; // Lilitaw pag tama
    public GameObject xMarkIcon;     // Lilitaw pag mali o delikado

    private bool isCleaned = false;

    void Start()
    {
        // Siguraduhing nakatago ang marks sa simula
        if (checkmarkIcon != null) checkmarkIcon.SetActive(false);
        if (xMarkIcon != null) xMarkIcon.SetActive(false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (isCleaned) return;

        if (eventData.pointerDrag != null)
        {
            ToolDragItem droppedTool = eventData.pointerDrag.GetComponent<ToolDragItem>();
            if (droppedTool != null) ProcessTool(droppedTool.toolType);
        }
    }

    void ProcessTool(string toolUsed)
    {
        // SPECIAL CASE: Kung nagsusuot ng Bota
        if (isPlayerTarget && toolUsed == "Boots")
        {
            if (Level9Part2Manager.instance != null) Level9Part2Manager.instance.WearBoots();
            isCleaned = true; 
            StartCoroutine(ShowCorrectFeedback(false)); // False: Wag burahin si Jobert
            return;
        }

        // KUNG TAMA ANG GAMIT
        if (toolUsed == requiredTool)
        {
            // CHECK 1: Naka-bota ba?
            if (requiresBoots && Level9Part2Manager.instance != null && !Level9Part2Manager.instance.isWearingBoots)
            {
                Level9Part2Manager.instance.AddInfection("DELIKADO! Magsuot muna ng bota bago maglinis!");
                StartCoroutine(ShowWrongFeedback());
                return; 
            }

            // CHECK 2: Final Disinfect ba?
            if (isFinalDisinfect && Level9Part2Manager.instance != null && Level9Part2Manager.instance.clearedHazards < 2)
            {
                Level9Part2Manager.instance.AddInfection("MALI! Alisin muna ang putik at tubig bago mag-disinfect!");
                StartCoroutine(ShowWrongFeedback());
                return;
            }

            // SUCCESS!
            Debug.Log("Nalinis ang: " + targetName);
            isCleaned = true;
            
            if (Level9Part2Manager.instance != null)
            {
                Level9Part2Manager.instance.statusText.text = "Nalinis ang " + targetName + "!";
                Level9Part2Manager.instance.HazardCleaned(); 
            }

            StartCoroutine(ShowCorrectFeedback(true)); // True: Burahin ang kalat pagkatapos ng 1 sec
        }
        else
        {
            // MALI ANG GAMIT
            if (Level9Part2Manager.instance != null)
            {
                Level9Part2Manager.instance.AddInfection("MALI! Hindi " + toolUsed + " ang panlinis sa " + targetName + "!");
            }
            StartCoroutine(ShowWrongFeedback());
        }
    }

    // --- ANIMATIONS ---
    IEnumerator ShowCorrectFeedback(bool hideTargetAfter)
    {
        if (checkmarkIcon != null) checkmarkIcon.SetActive(true);
        yield return new WaitForSeconds(1f); // Maghintay ng 1 second
        if (checkmarkIcon != null) checkmarkIcon.SetActive(false);

        // Kung kalat ito (Putik/Tubig), burahin na pagkatapos lumitaw ang checkmark
        if (hideTargetAfter)
        {
            gameObject.SetActive(false); 
        }
    }

    IEnumerator ShowWrongFeedback()
    {
        if (xMarkIcon != null) xMarkIcon.SetActive(true);
        yield return new WaitForSeconds(1f); // Maghintay ng 1 second
        if (xMarkIcon != null) xMarkIcon.SetActive(false);
    }
}