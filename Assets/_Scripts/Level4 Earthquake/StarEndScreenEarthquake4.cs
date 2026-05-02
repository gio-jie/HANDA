using UnityEngine;
using UnityEngine.UI;

public class StarEndScreenEarthquake4 : MonoBehaviour
{
    public Image[] stars;
    public Color activeColor = Color.white;
    public Color inactiveColor = Color.gray;

    void OnEnable()
    {
        StarManagerEarthquake4 starManagerEarthquake4 = FindObjectOfType<StarManagerEarthquake4>();

        if (starManagerEarthquake4 != null)
        {
            int currentStars = starManagerEarthquake4.GetCurrentStars();
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
