using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject mapPanel;  // Ito yung malaking panel na may drawing ng buong Map
    public GameObject mapButton; // Ito yung maliit na button sa gilid ng screen

    public void OpenMap()
    {
        mapPanel.SetActive(true);  // Ipakita ang Map
        mapButton.SetActive(false); // Itago muna ang Map Button (Optional: para malinis tignan)
        
        // PAUSE GAME
        Time.timeScale = 0; 
    }

    public void CloseMap()
    {
        mapPanel.SetActive(false); // Isara ang Map
        mapButton.SetActive(true); // Ibalik ang Map Button
        
        // RESUME GAME
        Time.timeScale = 1;
    }
}