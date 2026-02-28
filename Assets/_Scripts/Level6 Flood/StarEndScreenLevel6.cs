using UnityEngine;
using UnityEngine.UI;

public class StarEndScreenLevel6 : MonoBehaviour
{
    public Image[] stars;
    public Color activeColor = Color.white;
    public Color inactiveColor = Color.gray;

    void OnEnable()
    {
        StarManagerLevel6 starManagerLevel6 = FindObjectOfType<StarManagerLevel6>();

        if (starManagerLevel6 != null)
        {
            int currentStars = starManagerLevel6.GetCurrentStars();
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