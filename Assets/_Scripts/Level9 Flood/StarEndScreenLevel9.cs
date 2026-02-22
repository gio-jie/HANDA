using UnityEngine;
using UnityEngine.UI;

public class StarEndScreenLevel9 : MonoBehaviour
{
    public Image[] stars;
    public Color activeColor = Color.white;
    public Color inactiveColor = Color.gray;

    void OnEnable()
    {
        StarManagerLevel9 starManagerLevel9 = FindObjectOfType<StarManagerLevel9>();

        if (starManagerLevel9 != null)
        {
            int currentStars = starManagerLevel9.GetCurrentStars();
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