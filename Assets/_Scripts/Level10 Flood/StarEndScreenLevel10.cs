using UnityEngine;
using UnityEngine.UI;

public class StarEndScreenLevel10 : MonoBehaviour
{
    public Image[] stars;
    public Color activeColor = Color.white;
    public Color inactiveColor = Color.gray;

    void OnEnable()
    {
        StarManagerLevel10 starManagerLevel10 = FindObjectOfType<StarManagerLevel10>();

        if (starManagerLevel10 != null)
        {
            int currentStars = starManagerLevel10.GetCurrentStars();
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