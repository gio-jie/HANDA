using UnityEngine;
using UnityEngine.UI;

public class RepairTask : MonoBehaviour
{
    [Header("Requirements")]
    public string requiredTool; // "Hammer" or "Tape"
    public int hitsNeeded = 3;
    
    [Header("Visuals")]
    public Image taskImage; 

    private int currentHits = 0;
    private bool isFinished = false;
    private Level2Manager manager;

    void Start()
    {
        manager = FindFirstObjectByType<Level2Manager>();
        
        // Setup initial transparency for Tape
        if (requiredTool == "Tape")
        {
            var col = taskImage.color;
            col.a = 0f; 
            taskImage.color = col;
        }
    }

    public void OnClickObject()
    {
        if (isFinished) return; 

        // CHECK 1: May hawak bang tool ang player?
        if (manager.currentSelectedTool == "None")
        {
            Debug.Log("Pumili ka muna ng gamit sa baba!");
            return;
        }

        // CHECK 2: Tama ba ang tool?
        if (manager.currentSelectedTool == requiredTool)
        {
            // TAMA: Proceed sa action
            PerformAction();
        }
        else
        {
            // MALI: Penalty
            manager.ApplyPenalty();
        }
    }

    void PerformAction()
    {
        currentHits++;

        if (requiredTool == "Hammer")
        {
            transform.localScale -= new Vector3(0, 0.2f, 0); // Hammer Effect
        }
        else if (requiredTool == "Tape")
        {
            var col = taskImage.color;
            col.a = 1f; // Show Tape
            taskImage.color = col;
        }

        if (currentHits >= hitsNeeded)
        {
            isFinished = true;
            GetComponent<Button>().interactable = false;
            manager.TaskCompleted();
        }
    }
}