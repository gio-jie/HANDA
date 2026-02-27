using UnityEngine;
using UnityEngine.UI;

public class StarGradeTitleUI : MonoBehaviour
{
    [Header("Reference")]
    public Image titleImage;

    [Header("Grade Sprites")]
    public Sprite oneStarSprite;
    public Sprite twoStarSprite;
    public Sprite threeStarSprite;

    void OnEnable()
    {
        StarManager starManager = FindObjectOfType<StarManager>();

        if (starManager != null)
        {
            int currentStars = starManager.GetCurrentStars();
            UpdateTitleSprite(currentStars);
        }
    }

    void UpdateTitleSprite(int starCount)
    {
        if (titleImage == null) return;

        switch (starCount)
        {
            case 1:
                titleImage.sprite = oneStarSprite;
                break;
            case 2:
                titleImage.sprite = twoStarSprite;
                break;
            case 3:
                titleImage.sprite = threeStarSprite;
                break;
            default:
                titleImage.sprite = null;
                break;
        }
    }
}