using UnityEngine;
using UnityEngine.UI;

public class StarGradeTitleUILevel7 : MonoBehaviour
{
    [Header("Reference")]
    public Image titleImage;

    [Header("Grade Sprites")]
    public Sprite oneStarSprite;
    public Sprite twoStarSprite;
    public Sprite threeStarSprite;

    void OnEnable()
    {
        StarManagerLevel7 starManagerlevel7 = FindObjectOfType<StarManagerLevel7>();

        if (starManagerlevel7 != null)
        {
            int currentStars = starManagerlevel7.GetCurrentStars();
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