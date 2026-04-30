using UnityEngine;

public class Level7ManagerEQ : MonoBehaviour
{
    public static Level7ManagerEQ instance;

    [Header("Game Status")]
    public bool isGameActive = true;
    public GameObject currentObstacleNearPlayer; 

    [Header("UI Elements (I-drag mo dito yung tools)")]
    public GameObject keyUITool; 
    public GameObject extinguisherUITool; 

    void Awake()
    {
        instance = this;
    }

    public void SetCurrentObstacle(GameObject obstacle)
    {
        currentObstacleNearPlayer = obstacle;
    }

    public void ClearCurrentObstacle()
    {
        currentObstacleNearPlayer = null;
    }

    // --- LOGIC PARA SA PAGPULOT ---
    public void PickUpKey()
    {
        Debug.Log("NAKUHA ANG SUSI!");
        if (keyUITool != null)
        {
            keyUITool.SetActive(true); // Papalitawin ang susi sa Toolbelt panel
        }
    }

    public void PickUpExtinguisher()
    {
        Debug.Log("NAKUHA ANG FIRE EXTINGUISHER!");
        if (extinguisherUITool != null)
        {
            extinguisherUITool.SetActive(true); // Papalitawin ang extinguisher sa Toolbelt panel
        }
    }

    // --- LOGIC PARA SA PAG-DRAG AND DROP NG TOOLS ---
    public void UseTool(string toolNameUsed)
    {
        if (currentObstacleNearPlayer != null)
        {
            ObstacleData obsData = currentObstacleNearPlayer.GetComponent<ObstacleData>();
            
            if (obsData != null && obsData.correctTool == toolNameUsed)
            {
                Debug.Log("TAMA ANG TOOL! Nalinis ang kalat o nawala ang apoy!");
                
                // Burahin ang kalat/apoy/pinto sa screen
                Destroy(currentObstacleNearPlayer);
                ClearCurrentObstacle(); 
                
                // Kung ang ginamit ay Susi, itago na ulit yung Susi sa UI kasi nagamit na
                if (toolNameUsed == "Key" && keyUITool != null)
                {
                    keyUITool.SetActive(false); 
                }
                // Kung ang ginamit ay Extinguisher, itago na ulit yung Extinguisher sa UI kasi nagamit na
                else if (toolNameUsed == "Extinguisher" && extinguisherUITool != null)
                {
                    extinguisherUITool.SetActive(false); 
                }
            }
            else
            {
                Debug.Log("MALI ANG TOOL!");
            }
        }
        else
        {
            Debug.Log("Walang kalat sa harap ni Jobert!");
        }
    }
}