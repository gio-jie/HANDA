using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TriagePatient : MonoBehaviour, IDropHandler
{
    [Header("First Aid Sequence (EXACT ORDER)")]
    // Dito natin ililista kung ano ang unang dapat i-drag hanggang huli
    public string[] requiredSequence = { "Cotton", "Betadine", "Bandage" };
    
    private int currentStepIndex = 0; // Taga-tanda kung nasa anong step na tayo

    [Header("UI Feedback")]
    public TMP_Text statusText;

    void Start()
    {
        UpdateStatusText();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            // Ginamit natin ulit yung RationDragItem para hindi ka na gagawa ng bagong drag script!
            RationDragItem droppedItem = eventData.pointerDrag.GetComponent<RationDragItem>();
            if (droppedItem != null)
            {
                CheckMedicalStep(droppedItem.itemType);
            }
        }
    }

    void CheckMedicalStep(string itemType)
    {
        // Kung hindi pa tapos ang gamutan
        if (currentStepIndex < requiredSequence.Length)
        {
            // KUNG TAMA ANG STEP NA BINIGAY MO
            if (itemType == requiredSequence[currentStepIndex])
            {
                Debug.Log("TAMA! Step " + (currentStepIndex + 1) + " completed.");
                currentStepIndex++; // Uusad na sa susunod na step!
                
                UpdateStatusText();

                // KUNG TAPOS NA LAHAT NG STEPS
                if (currentStepIndex >= requiredSequence.Length)
                {
                    Debug.Log("MAGALING! Patient Cured!");
                    statusText.text = "Patient Cured! Good job!";
                    statusText.color = Color.green;
                    // Dito natin ilalagay yung next logic (pagpapalit ng pasyente o Win Level)
                }
            }
            // KUNG MALI ANG SEQUENCE (Halimbawa: Bandage agad kahit di pa nalilinis)
            else
            {
                Debug.Log("MALI! Dapat ay " + requiredSequence[currentStepIndex] + " muna!");
                // Pwede tayong maglagay ng penalty sa oras o score dito mamaya
            }
        }
    }

    void UpdateStatusText()
    {
        if (currentStepIndex < requiredSequence.Length)
        {
            statusText.text = "Patient Needs:\n" + requiredSequence[currentStepIndex];
            statusText.color = Color.white;
        }
    }
}