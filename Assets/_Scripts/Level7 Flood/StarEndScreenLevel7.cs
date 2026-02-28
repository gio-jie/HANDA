using UnityEngine;
using UnityEngine.UI;

public class StarEndScreenLevel7 : MonoBehaviour
{
    public Image[] stars;
    public Color activeColor = Color.white;
    public Color inactiveColor = Color.gray;

    void OnEnable()
    {
        StarManagerLevel7 starManagerLevel7 = FindObjectOfType<StarManagerLevel7>();

        if (starManagerLevel7 != null)
        {
            int currentStars = starManagerLevel7.GetCurrentStars();
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