using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject tutorialPanel; // Ang Panel na may instructions
    public GameObject openButton;    // Yung Book Button

    void Start()
    {
        // Pag-start ng level, automatic bukas ang Tutorial
        OpenTutorial();
    }

    public void OpenTutorial()
    {
        tutorialPanel.SetActive(true);
        openButton.SetActive(false); // Tago muna ang book button pag nakabukas ang panel
        
        // PAUSE GAME
        Time.timeScale = 0; 
    }

    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
        openButton.SetActive(true); // Labas ulit ang book button
        
        // RESUME GAME
        Time.timeScale = 1;
    }
}