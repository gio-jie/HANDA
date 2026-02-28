using UnityEngine;
using UnityEngine.UI;

public class StarEndScreen : MonoBehaviour
{
    public Image[] stars;
    public Color activeColor = Color.white;
    public Color inactiveColor = Color.gray;

    void OnEnable()
    {
        StarManager starManager = FindObjectOfType<StarManager>();

        if (starManager != null)
        {
            int currentStars = starManager.GetCurrentStars();
            DisplayStars(currentStars);
        }
    }

    void DisplayStars(int starCount)
    {
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].color = i < starCount ? activeColor : inactiveColor;
        }
    }
}