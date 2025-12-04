using UnityEngine;
using UnityEngine.UI;

public class RepairTask : MonoBehaviour
{
    [Header("Requirements")]
    public string requiredTool; // "Hammer" or "Tape"
    public int hitsNeeded = 3;
    
    [Header("Visuals")]
    public Image taskImage; // Ang image na papalitan
    public Sprite finalSprite; // Dito ilalagay ang "Ulo ng Pako" image
    public Vector2 finalSize = new Vector2(40, 40); // Size ng ulo ng pako

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

        if (manager.currentSelectedTool == "None")
        {
            Debug.Log("Pumili ka muna ng gamit sa baba!");
            return;
        }

        if (manager.currentSelectedTool == requiredTool)
        {
            PerformAction();
        }
        else
        {
            manager.ApplyPenalty();
        }
    }

    void PerformAction()
    {
        currentHits++;

        if (requiredTool == "Hammer")
        {
            // MOVEMENT LOGIC:
            // Bawat palo, bumababa nang konti para kunwari bumabaon
            transform.localPosition -= new Vector3(0, 20f, 0); 
            Debug.Log("Pok!");
        }
        else if (requiredTool == "Tape")
        {
            var col = taskImage.color;
            col.a = 1f; 
            taskImage.color = col;
        }

        // CHECK IF FINAL HIT
        if (currentHits >= hitsNeeded)
        {
            isFinished = true;
            GetComponent<Button>().interactable = false;
            
            // --- SPRITE SWAP LOGIC (BAGO!) ---
            if (requiredTool == "Hammer" && finalSprite != null)
            {
                // 1. Palitan ang itsura (Maging bilog)
                taskImage.sprite = finalSprite;
                
                // 2. Palitan ang size (Para maging maliit na bilog, hindi mahabang pako)
                taskImage.rectTransform.sizeDelta = finalSize;
                
                // 3. I-center ang pwesto (Optional adjustment)
                // transform.localPosition += new Vector3(0, 10f, 0); 
            }
            // ---------------------------------

            manager.TaskCompleted();
        }
    }
}