using UnityEngine;
using UnityEngine.UI;

public class RepairTask : MonoBehaviour
{
    [Header("Task Settings")]
    public bool isNail; // Check ito kung Pako, Uncheck kung Tape
    public int hitsNeeded = 3; // Ilang pukpok para bumaon? (3 sa pako, 1 sa tape)
    
    [Header("Visuals")]
    public Image taskImage; // Ang itsura ng object

    private int currentHits = 0;
    private bool isFinished = false;
    
    // Reference sa Manager (Gagawa tayo nito sa next step)
    private Level2Manager manager;

    void Start()
    {
        // Hanapin ang manager sa scene
        manager = FindFirstObjectByType<Level2Manager>();
        
        // Setup initial look
        if (isNail)
        {
            // Kung pako, siguraduhing kulay gray
            taskImage.color = Color.gray; 
        }
        else
        {
            // Kung tape, siguraduhing invisible sa simula (transparent)
            var col = taskImage.color;
            col.a = 0f; // Invisible
            taskImage.color = col;
        }
    }

    public void OnClickObject()
    {
        if (isFinished) return; // Pag tapos na, wag na gumalaw

        currentHits++;

        if (isNail)
        {
            // LOGIC PARA SA PAKO (Hammer Effect)
            // Bawat click, lumiliit/lumulubog ang pako
            transform.localScale -= new Vector3(0, 0.2f, 0); 
            
            // Pwede magdagdag ng sound effect dito later
            Debug.Log("Pok!");
        }
        else
        {
            // LOGIC PARA SA TAPE
            // Pag click, lilitaw ang tape
            var col = taskImage.color;
            col.a = 1f; // Visible na
            taskImage.color = col;
        }

        // Check kung tapos na trabaho
        if (currentHits >= hitsNeeded)
        {
            isFinished = true;
            Debug.Log("Task Done!");
            
            // Disable button para di na mapindot
            GetComponent<Button>().interactable = false;

            // Report sa Manager
            if(manager != null)
            {
                manager.TaskCompleted();
            }
        }
    }
}