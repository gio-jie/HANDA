using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // DAGDAG PARA SA SCENE CHECK

public class DialogueManager : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI dialogueText; 
    public GameObject dialoguePanel;    
    public Button nextButton;           
    public TextMeshProUGUI buttonText;   
    public Button skipButton;

    [Header("Jobert's Script & Audio")]
    [TextArea(3, 10)] 
    public string[] sentences; 
    public AudioClip[] voiceOvers;
    public AudioSource audioSource;

    private int index = 0;
    
    // --- BAGONG DAGDAG: DYNAMIC SAVE KEY ---
    private string saveKey = "JobertTriviaSeen"; // Default pang-Typhoon

    public int CurrentIndex 
    {
        get { return index; }
    }

    void Awake()
    {
        // AUTO-DETECT: Alamin kung nasaang stage tayo para hindi mag-conflict kay Nadine!
        if (SceneManager.GetActiveScene().name.Contains("Flood"))
        {
            saveKey = "FloodIntroSeen"; // Gamitin ang kay Nadine
        }
        else
        {
            saveKey = "JobertTriviaSeen"; // Gamitin ang sa'yo, Gio
        }
    }

    void Start()
    {
        // Gagamitin na niya ngayon kung anong saveKey ang nabasa niya sa Awake
        if (PlayerPrefs.GetInt(saveKey, 0) == 1)
        {
            if (dialoguePanel != null) dialoguePanel.SetActive(false);
            return;
        }

        index = 0;
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        UpdateDialogue();
    }

    public void NextSentence()
    {
        if (index < sentences.Length - 1)
        {
            index++;
            UpdateDialogue();
        }
        else
        {
            EndDialogue();
        }
    }

    public void SkipDialogue()
    {
        EndDialogue();
    }

    void UpdateDialogue()
    {
        if (dialogueText != null && sentences.Length > 0)
            dialogueText.text = sentences[index];

        if (audioSource != null && voiceOvers.Length > index && voiceOvers[index] != null)
        {
            audioSource.Stop(); 
            audioSource.clip = voiceOvers[index];
            audioSource.Play();
        }

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
        if (audioSource != null) audioSource.Stop();

        // I-SAVE ANG TAMANG KEY DEPENDE SA STAGE!
        PlayerPrefs.SetInt(saveKey, 1);
        PlayerPrefs.Save();

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }
}