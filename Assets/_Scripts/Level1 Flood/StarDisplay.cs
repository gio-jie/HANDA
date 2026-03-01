using UnityEngine;
using UnityEngine.UI;

public class StarDisplay : MonoBehaviour
{
    public string stagePrefix = "";
    public int levelIndex;
    public Image[] stars;

    public Color activeColor = Color.white;
    public Color inactiveColor = Color.gray;

    [Header("Lock Settings")]
    public Button levelButton;
    public GameObject lockIcon;

    void Start()
    {
        int savedStars = PlayerPrefs.GetInt(stagePrefix + "Level_" + levelIndex, 0);

        if (stars != null)
        {
            for (int i = 0; i < stars.Length; i++)
            {
                if (stars[i] != null) 
                {
                    stars[i].color = i < savedStars ? activeColor : inactiveColor;
                }
            }
        }

        bool isUnlocked = false;

        if (levelIndex == 1)
        {
            isUnlocked = true;
        }
        else
        {
            int prevLevelStars = PlayerPrefs.GetInt(stagePrefix + "Level_" + (levelIndex - 1), 0);
            isUnlocked = (prevLevelStars > 0);
        }

        if (levelButton != null)
        {
            levelButton.interactable = isUnlocked;
        }

        if (lockIcon != null)
        {
            lockIcon.SetActive(!isUnlocked);
        }
    }
}