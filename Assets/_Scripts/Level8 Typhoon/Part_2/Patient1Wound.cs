using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Patient1Wound : MonoBehaviour, IDropHandler
{
    [Header("Wound UI Layers")]
    public Image dirtOverlay;         
    public Image betadineOverlay;     
    public GameObject bandageApplied; 

    [Header("UI Feedback")]
    public TMP_Text statusText; // DITO NATIN ILALAGAY YUNG BINALIK MONG TEXT

    [Header("Rubbing Settings")]
    public float rubSpeed = 0.5f;     
    public float rubRadius = 150f;    

    private int currentStep = 0; // 0=Alcohol, 1=Betadine, 2=Bandage
    private Vector3 lastMousePos;

    void Start()
    {
        // Pagka-start ng laro, i-update agad ang text sa Step 0
        UpdateStatusText();
    }

    void Update()
    {
        if (RationDragItem.currentlyDraggedItem != null)
        {
            string heldItem = RationDragItem.currentlyDraggedItem.itemType;
            float distanceToWound = Vector2.Distance(Input.mousePosition, transform.position);

            if (distanceToWound <= rubRadius)
            {
                float moveDelta = Vector3.Distance(Input.mousePosition, lastMousePos);
                if (moveDelta > 2f) 
                {
                    // STEP 1: KUSKOS ALCOHOL
                    if (currentStep == 0 && heldItem == "Alcohol")
                    {
                        Color c = dirtOverlay.color;
                        c.a -= rubSpeed * Time.deltaTime; 
                        dirtOverlay.color = c;

                        if (c.a <= 0)
                        {
                            c.a = 0; dirtOverlay.color = c;
                            currentStep = 1; 
                            
                            if(Level8Part2Manager.instance != null) Level8Part2Manager.instance.CorrectStep();
                            UpdateStatusText(); // PALITAN ANG TEXT!
                            Debug.Log("Malinis na! Next: Betadine");
                        }
                    }
                    // STEP 2: KUSKOS BETADINE
                    else if (currentStep == 1 && heldItem == "Betadine")
                    {
                        Color c = betadineOverlay.color;
                        c.a += rubSpeed * Time.deltaTime; 
                        betadineOverlay.color = c;

                        if (c.a >= 1)
                        {
                            c.a = 1; betadineOverlay.color = c;
                            currentStep = 2; 
                            
                            if(Level8Part2Manager.instance != null) Level8Part2Manager.instance.CorrectStep();
                            UpdateStatusText(); // PALITAN ANG TEXT!
                            Debug.Log("May gamot na! Next: Bandage");
                        }
                    }
                }
            }
        }
        lastMousePos = Input.mousePosition; 
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (RationDragItem.currentlyDraggedItem != null)
        {
            string droppedItem = RationDragItem.currentlyDraggedItem.itemType;

            // KUNG TAMA ANG BANDAGE SA HULI
            if (currentStep == 2 && droppedItem == "Bandage")
            {
                bandageApplied.SetActive(true); 
                
                if(Level8Part2Manager.instance != null) Level8Part2Manager.instance.CorrectStep();
                
                currentStep = 3; 
                UpdateStatusText(); // PALITAN ANG TEXT TO "CURED!"
                Debug.Log("PATIENT 1 CURED! TRANSITION NEXT!");
                Invoke("CallNext", 1.5f);
            }
            // KUNG MALI ANG ITEM
            else if ((currentStep == 0 && droppedItem != "Alcohol") ||
                     (currentStep == 1 && droppedItem != "Betadine") ||
                     (currentStep == 2 && droppedItem != "Bandage"))
            {
                Debug.Log("MALI! Hindi pa 'yan ang kailangan!");
                if(Level8Part2Manager.instance != null) Level8Part2Manager.instance.WrongItem();
            }
        }
    }

    // --- BAGONG FUNCTION: Taga-palit ng Text ---
    void UpdateStatusText()
    {
        if (statusText == null) return; // Iwas error kung walang text na nakakabit

        if (currentStep == 0)
        {
            statusText.text = "Need: Cotton w/ Alcohol";
            statusText.color = Color.white;
        }
        else if (currentStep == 1)
        {
            statusText.text = "Need: Betadine";
            statusText.color = Color.white;
        }
        else if (currentStep == 2)
        {
            statusText.text = "Need: Bandage";
            statusText.color = Color.white;
        }
        else if (currentStep == 3)
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