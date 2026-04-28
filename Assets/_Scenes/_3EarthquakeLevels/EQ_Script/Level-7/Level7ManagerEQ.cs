using UnityEngine;

public class Level7ManagerEQ : MonoBehaviour
{
    public static Level7ManagerEQ instance;

    [Header("Game Status")]
    public bool isGameActive = true;
    public GameObject currentObstacleNearPlayer; 

    [Header("UI Elements")]
    public GameObject keyUITool; // DITO NATIN ILALAGAY YUNG UI SUSI SA TOOLBELT

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

    // --- NA-UPDATE NA LOGIC PARA SA SUSI ---
    public void PickUpKey()
    {
        Debug.Log("NAKUHA ANG SUSI!");
        if (keyUITool != null)
        {
            keyUITool.SetActive(true); // Papalitawin ang susi sa Toolbelt panel!
            if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);
        }
    }

    public void UseTool(string toolNameUsed)
    {
        if (currentObstacleNearPlayer != null)
        {
            ObstacleData obsData = currentObstacleNearPlayer.GetComponent<ObstacleData>();
            
            if (obsData != null && obsData.correctTool == toolNameUsed)
            {
                Debug.Log("TAMA ANG TOOL! Nalinis ang kalat!");
                
                // Burahin ang kalat/pinto sa screen
                Destroy(currentObstacleNearPlayer);
                ClearCurrentObstacle(); 
                
                // Kung ang ginamit ay Susi, itago na ulit yung Susi sa UI kasi nagamit na
                if (toolNameUsed == "Key" && keyUITool != null)
                {
                    keyUITool.SetActive(false); 
                }

                if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);
            }
            else
            {
                Debug.Log("MALI ANG TOOL!");
                if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.warningSound);
            }
        }
        else
        {
            Debug.Log("Walang kalat sa harap ni Jobert!");
        }
    }
}