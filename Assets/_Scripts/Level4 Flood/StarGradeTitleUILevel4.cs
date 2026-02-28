using UnityEngine;
using UnityEngine.UI;

public class StarGradeTitleUILevel4 : MonoBehaviour
{
    [Header("Reference")]
    public Image titleImage;

    [Header("Grade Sprites")]
    public Sprite oneStarSprite;
    public Sprite twoStarSprite;
    public Sprite threeStarSprite;

    void OnEnable()
    {
        StarManagerLevel4 starManagerlevel4 = FindObjectOfType<StarManagerLevel4>();

        if (starManagerlevel4 != null)
        {
            int currentStars = starManagerlevel4.GetCurrentStars();
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