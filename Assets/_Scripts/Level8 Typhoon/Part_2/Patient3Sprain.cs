using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class Patient3Sprain : MonoBehaviour, IDropHandler
{
    [Header("UI Layers")]
    public TMP_Text statusText;
    public GameObject elasticBandageApplied; // Lilitaw pag nilagay ang Elastic Bandage
    public GameObject armSlingApplied;       // Lilitaw pag nilagay ang Towel

    private int currentStep = 0; // 0=IcePack, 1=ElasticBandage, 2=Towel

    void Start()
    {
        UpdateStatusText();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (RationDragItem.currentlyDraggedItem != null)
        {
            RationDragItem droppedItem = RationDragItem.currentlyDraggedItem;
            string itemType = droppedItem.itemType;

            // STEP 1: ICE PACK (May 3 seconds waiting time)
            if (currentStep == 0 && itemType == "IcePack")
            {
                droppedItem.isLocked = true;
                droppedItem.transform.position = transform.position; // Pwesto sa wrist ng pasyente
                StartCoroutine(IcePackRoutine(droppedItem));
            }
            // STEP 2: ELASTIC BANDAGE
            else if (currentStep == 1 && itemType == "ElasticBandage")
            {
                elasticBandageApplied.SetActive(true); // Palitawin ang balot sa braso
                CompleteStep();
                Destroy(droppedItem.gameObject); 
            }
            // STEP 3: TOWEL (Arm Sling)
            else if (currentStep == 2 && itemType == "Towel")
            {
                armSlingApplied.SetActive(true); // Palitawin ang naka-sling na braso
                CompleteStep();
                Destroy(droppedItem.gameObject);
                
                Debug.Log("PATIENT 3 CURED! LEVEL COMPLETE!");
                Invoke("CallNext", 1.5f); // Tawagin ang end level animation
            }
            else
            {
                Debug.Log("MALI! Hindi pa yan ang kailangan.");
                if(Level8Part2Manager.instance != null) Level8Part2Manager.instance.WrongItem();
            }
        }
    }

    // --- ANIMATION NG PAGBABABAD NG YELO ---
    IEnumerator IcePackRoutine(RationDragItem iceItem)
    {
        statusText.text = "Applying Ice...";
        statusText.color = Color.blue;

        yield return new WaitForSeconds(3f); // Maghintay ng 3 seconds

        iceItem.ReturnToTable(); // Pabalikin sa lamesa

        if(Level8Part2Manager.instance != null) Level8Part2Manager.instance.CorrectStep();
        currentStep = 1;
        UpdateStatusText();
        
        Destroy(iceItem); // Burahin ang script para maging display item na lang
    }

    void CompleteStep()
    {
        if(Level8Part2Manager.instance != null) Level8Part2Manager.instance.CorrectStep();
        currentStep++;
        UpdateStatusText();
    }

    void UpdateStatusText()
    {
        if(statusText == null) return;

        if(currentStep == 0) { statusText.text = "Need: Ice Pack"; statusText.color = Color.white; }
        else if(currentStep == 1) { statusText.text = "Need: Elastic Bandage"; statusText.color = Color.white; }
        else if(currentStep == 2) { statusText.text = "Need: Towel (Sling)"; statusText.color = Color.white; }
        else if(currentStep == 3) { statusText.text = "Patient Cured!"; statusText.color = Color.green; }
    }

    void CallNext()
    {
        // Ito ang magti-trigger ng katapusan ng laro natin!
        if(Level8Part2Manager.instance != null) Level8Part2Manager.instance.NextPatient();
    }
}