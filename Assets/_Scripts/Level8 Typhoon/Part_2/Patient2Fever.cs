using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class Patient2Fever : MonoBehaviour, IDropHandler
{
    [Header("UI Layers")]
    public TMP_Text statusText;
    public GameObject coolingPadApplied; 
    public TMP_Text thermometerText; 

    private int currentStep = 0; 

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

            // STEP 1: THERMOMETER (May Animation)
            if (currentStep == 0 && itemType == "Thermometer")
            {
                // 1. I-lock yung thermometer para wag bumalik agad sa lamesa
                droppedItem.isLocked = true;
                
                // 2. I-pwesto sa gitna ng pasyente
                droppedItem.transform.position = transform.position; 
                
                // 3. Simulan ang pag-akyat ng number
                StartCoroutine(ThermometerRoutine(droppedItem));
            }
            // STEP 2: GAMOT
            else if (currentStep == 1 && itemType == "Medicine")
            {
                CompleteStep();
                Destroy(droppedItem.gameObject); 
            }
            // STEP 3: TUBIG
            else if (currentStep == 2 && itemType == "Water")
            {
                CompleteStep();
                Destroy(droppedItem.gameObject); 
            }
            // STEP 4: COOLING PAD
            else if (currentStep == 3 && itemType == "CoolingPad")
            {
                coolingPadApplied.SetActive(true); 
                Invoke("CallNext", 1.5f);
                CompleteStep();
                Destroy(droppedItem.gameObject);
            }
            else
            {
                Debug.Log("MALI! Hindi pa yan ang kailangan.");
                if(Level8Part2Manager.instance != null) Level8Part2Manager.instance.WrongItem();
            }
        }
    }

    // --- ANG UPDATED ANIMATION NG THERMOMETER ---
    IEnumerator ThermometerRoutine(RationDragItem thermoItem)
    {
        float temp = 0f;
        float targetTemp = 39f;
        float duration = 2f; 
        float elapsed = 0f;

        // Loop para sa pag-akyat ng number habang nasa pasyente
        while(elapsed < duration)
        {
            elapsed += Time.deltaTime;
            temp = Mathf.Lerp(0f, targetTemp, elapsed / duration);
            
            if(thermometerText != null)
            {
                thermometerText.text = Mathf.RoundToInt(temp) + "°C";
                if (temp > 37f) thermometerText.color = Color.red;
            }
            yield return null;
        }

        if(thermometerText != null) thermometerText.text = "39°C";
        
        // --- HINTAY NG 1 SECOND PARA MAKITA NG PLAYER ---
        yield return new WaitForSeconds(1f);

        // --- IBALIK SA LAMESA MANUALLY ---
        thermoItem.ReturnToTable();

        // Maglabas ng checkmark at usad sa next step
        if(Level8Part2Manager.instance != null) Level8Part2Manager.instance.CorrectStep();
        currentStep = 1;
        UpdateStatusText();
        
        // Burahin ang script para maging display na lang sa lamesa
        Destroy(thermoItem); 
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
        
        if(currentStep == 0) statusText.text = "Need: Thermometer";
        else if(currentStep == 1) statusText.text = "Need: Medicine";
        else if(currentStep == 2) statusText.text = "Need: Water";
        else if(currentStep == 3) statusText.text = "Need: Cooling Pad";
        else if(currentStep == 4) 
        {
            statusText.text = "Patient Cured!";
            statusText.color = Color.green;
        }
    }
    void CallNext() 
    { 
        if(Level8Part2Manager.instance != null) Level8Part2Manager.instance.NextPatient(); 
    }
    
}