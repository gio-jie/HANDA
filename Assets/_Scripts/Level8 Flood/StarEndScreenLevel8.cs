using UnityEngine;
using UnityEngine.UI;

public class StarEndScreenLevel8 : MonoBehaviour
{
    public Image[] stars;
    public Color activeColor = Color.white;
    public Color inactiveColor = Color.gray;

    void OnEnable()
    {
        StarManagerLevel8 starManagerLevel8 = FindObjectOfType<StarManagerLevel8>();

        if (starManagerLevel8 != null)
        {
            int currentStars = starManagerLevel8.GetCurrentStars();
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