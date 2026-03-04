using UnityEngine;

public class CertificateButtonVisibility : MonoBehaviour
{
    [Header("Certificate UI")]
    public GameObject certificateButton;

    void Start()
    {
        int isStage1Done = PlayerPrefs.GetInt("Flood_Unlocked", 0);

        if (certificateButton != null)
        {
            if (isStage1Done == 1)
            {
                certificateButton.SetActive(true);
            }
            else
            {
                certificateButton.SetActive(false);
            }
        }
    }
}