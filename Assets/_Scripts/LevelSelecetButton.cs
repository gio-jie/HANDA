using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    [Header("Settings")]
    public int levelNumber = 1; // Anong level ito? (1, 2, 3...)

    [Header("UI Elements")]
    public GameObject lockIcon; // Yung padlock image
    public GameObject[] stars;  // Array para sa 3 stars (Left, Middle, Right)
    public Button myButton;     // Ang button component mismo

    void Start()
    {
        UpdateButtonStatus();
    }

    void UpdateButtonStatus()
    {
        // 1. CHECK IF UNLOCKED
        // Ang Level 1 ay laging bukas (default 1). Ang iba, default 0 (locked).
        int isUnlocked = 0;
        
        if (levelNumber == 1) 
        {
            isUnlocked = 1; // Always open ang Level 1
        }
        else
        {
            // Check sa memory kung nabuksan na ng previous level
            isUnlocked = PlayerPrefs.GetInt("Level" + levelNumber + "_Unlocked", 0);
        }

        // 2. APPLY VISUALS
        if (isUnlocked == 1)
        {
            // UNLOCKED STATE
            if(myButton) myButton.interactable = true;
            if(lockIcon) lockIcon.SetActive(false); // Tago ang padlock

            // Show Stars based on record
            int starsCount = PlayerPrefs.GetInt("Level" + levelNumber + "_Stars", 0);
            
            // Loop para buksan ang tamang dami ng stars
            for (int i = 0; i < stars.Length; i++)
            {
                if (i < starsCount) 
                    stars[i].SetActive(true);  // Kulay Yellow
                else 
                    stars[i].SetActive(false); // Tago or Gray
            }
        }
        else
        {
            // LOCKED STATE
            if(myButton) myButton.interactable = false; // Di mapipindot
            if(lockIcon) lockIcon.SetActive(true); // Labas ang padlock
            
            // Tago lahat ng stars pag locked
            foreach (GameObject star in stars) star.SetActive(false);
        }
    }
    
    // Pang-reset lang (For testing purposes, pwede mong tawagin to)
    public void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
    }
}