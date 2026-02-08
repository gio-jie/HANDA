using UnityEngine;
using UnityEngine.UI; // Need ito para sa Text at Buttons

public class Level9Manager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject questionPanel; // Yung Panel na ginawa natin
    public Text questionText;
    public Text option1Text; // Text sa loob ng Button 1
    public Text option2Text; // Text sa loob ng Button 2

    [Header("Game State")]
    public bool isPanelActive = false;
    private HazardData currentHazard; // Sino ang hazard na active ngayon?

    void Start()
    {
        questionPanel.SetActive(false); // Siguraduhing nakasara sa simula
    }

    // TATAWAGIN NG HAZARD PAG CLINICK
    public void ShowQuestion(HazardData hazard)
    {
        currentHazard = hazard; // Tandaan kung sinong hazard ang nagtanong
        
        // I-set ang text sa UI
        questionText.text = hazard.questionText;
        option1Text.text = hazard.option1Text; // Note: Drag text object inside button
        option2Text.text = hazard.option2Text;

        // Buksan ang panel
        questionPanel.SetActive(true);
        isPanelActive = true;
    }

    // TATAWAGIN NG BUTTONS (1 or 2)
    public void SelectAnswer(int playerChoice)
    {
        if (currentHazard == null) return;

        if (playerChoice == currentHazard.correctOption)
        {
            Debug.Log("CORRECT! Hazard Solved.");
            // Wasakin ang hazard
            Destroy(currentHazard.gameObject);
        }
        else
        {
            Debug.Log("WRONG! Bawas Buhay!");
            // Dito pwede maglagay ng damage logic later
        }

        // Isara ang panel
        ClosePanel();
    }

    void ClosePanel()
    {
        questionPanel.SetActive(false);
        isPanelActive = false;
        currentHazard = null;
    }
}