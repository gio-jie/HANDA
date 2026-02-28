using UnityEngine;
using UnityEngine.UI;

public class StarEndScreenLevel3 : MonoBehaviour
{
    public Image[] stars;
    public Color activeColor = Color.white;
    public Color inactiveColor = Color.gray;

    void OnEnable()
    {
        StarManagerLevel3 starManagerLevel3 = FindObjectOfType<StarManagerLevel3>();

        if (starManagerLevel3 != null)
        {
            int currentStars = starManagerLevel3.GetCurrentStars();
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