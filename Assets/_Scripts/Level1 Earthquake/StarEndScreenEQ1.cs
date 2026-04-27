using UnityEngine;
using UnityEngine.UI;

public class StarEndScreenEQ1 : MonoBehaviour
{
    public Image[] stars;
    public Color activeColor = Color.white;
    public Color inactiveColor = Color.gray;

    void OnEnable()
    {
        StarManagerEarthquake1 starManagerEarthquake1 = FindObjectOfType<StarManagerEarthquake1>();

        if (starManagerEarthquake1 != null)
        {
            int currentStars = starManagerEarthquake1.GetCurrentStars();
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