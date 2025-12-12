using UnityEngine;
using TMPro; // Kailangan para sa Text Mesh Pro
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI dialogueText; // Kung saan lalabas ang salita
    public GameObject dialoguePanel;     // Ang buong panel ni Jobert
    public Button nextButton;            // Ang button na pipindutin
    public TextMeshProUGUI buttonText;   // Text ng button (Next -> Go)

    [Header("Jobert's Script")]
    [TextArea(3, 10)] // Para malaki ang box sa Inspector
    public string[] sentences; // Dito natin ita-type yung script

    private int index = 0; // Pang-ilang sentence na tayo?

    void Start()
    {
        // CHECKING: Kung "1" ang value nito, ibig sabihin nakita na. ITAGO ang panel.
        if (PlayerPrefs.GetInt("JobertTriviaSeen", 0) == 1)
        {
            dialoguePanel.SetActive(false);
            return; // Tigil na dito, wag na ituloy ang iba.
        }

        // Kung "0", tuloy ang script...
        index = 0;
        dialoguePanel.SetActive(true);
        UpdateDialogue();
    }

    public void NextSentence()
    {
        // Kung hindi pa tapos, next line
        if (index < sentences.Length - 1)
        {
            index++;
            UpdateDialogue();
        }
        else
        {
            // Kung tapos na, isara ang panel
            EndDialogue();
        }
    }

    void UpdateDialogue()
    {
        // Update text
        dialogueText.text = sentences[index];

        // Check kung last sentence na (Change button text)
        if (index == sentences.Length - 1)
        {
            if(buttonText != null) buttonText.text = "LET'S GO!";
        }
        else
        {
            if(buttonText != null) buttonText.text = "NEXT";
        }
    }

    void EndDialogue()
    {
        // MARKAHAN: Tapos na magsalita, i-save na natin na "Seen" na siya.
        PlayerPrefs.SetInt("JobertTriviaSeen", 1);
        PlayerPrefs.Save();

        dialoguePanel.SetActive(false);
    }
}