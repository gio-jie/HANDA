using UnityEngine;
using UnityEngine.UI;

public class StarGradeTitleUILevel3 : MonoBehaviour
{
    [Header("Reference")]
    public Image titleImage;

    [Header("Grade Sprites")]
    public Sprite oneStarSprite;
    public Sprite twoStarSprite;
    public Sprite threeStarSprite;

    void OnEnable()
    {
        StarManagerLevel3 starManagerlevel3 = FindObjectOfType<StarManagerLevel3>();

        if (starManagerlevel3 != null)
        {
            int currentStars = starManagerlevel3.GetCurrentStars();
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