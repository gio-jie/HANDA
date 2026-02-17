using UnityEngine;
using UnityEngine.UI;

public class StarEndScreenLevel2 : MonoBehaviour
{
    public Image[] stars;
    public Color activeColor = Color.white;
    public Color inactiveColor = Color.gray;

    void OnEnable()
    {
        StarManagerLevel2 starManagerLevel2 = FindObjectOfType<StarManagerLevel2>();

        if (starManagerLevel2 != null)
        {
            int currentStars = starManagerLevel2.GetCurrentStars();
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