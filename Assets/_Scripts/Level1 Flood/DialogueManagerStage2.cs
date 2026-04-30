using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManagerStage2 : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI dialogueText; 
    public GameObject dialoguePanel;    
    public Button nextButton;           
    public TextMeshProUGUI buttonText;   

    // --- BAGONG DAGDAG: Skip Button ---
    public Button skipButton;

    [Header("Jobert's Script & Audio")]
    [TextArea(3, 10)] 
    public string[] sentences; 
    
    // --- BAGONG DAGDAG: Para sa Boses ni Jobert ---
    public AudioClip[] voiceOvers;
    public AudioSource audioSource;

    private int index = 0;

    // --- FIX PARA SA SCRIPT NI DEV-NADINE (IntroVisualController) ---
    public int CurrentIndex 
    {
        get { return index; }
    }
    // ----------------------------------------------------------------

    void Start()
    {
        if (PlayerPrefs.GetInt("FloodIntroSeen", 0) == 1)
        {
            dialoguePanel.SetActive(false);
            return;
        }

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

    public void SkipDialogue()
    {
        EndDialogue();
    }

    void UpdateDialogue()
    {
        // Update text
        dialogueText.text = sentences[index];

        // Patugtugin ang voice over kung meron man tayong nilagay sa Inspector
        if (audioSource != null && voiceOvers.Length > index && voiceOvers[index] != null)
        {
            audioSource.Stop(); // Patayin muna yung nakaraang boses bago mag-play ng bago
            audioSource.clip = voiceOvers[index];
            audioSource.Play();
        }

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
        // Patayin ang tunog pagka-close ng panel
        if (audioSource != null) audioSource.Stop();

        // MARKAHAN: Tapos na magsalita, i-save na natin na "Seen" na siya.
        PlayerPrefs.SetInt("FloodIntroSeen", 1);
        PlayerPrefs.Save();

        dialoguePanel.SetActive(false);
    }
}