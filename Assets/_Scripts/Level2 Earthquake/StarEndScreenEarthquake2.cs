using UnityEngine;
using UnityEngine.UI;

public class StarEndScreenEarthquake2 : MonoBehaviour
{
    public Image[] stars;
    public Color activeColor = Color.white;
    public Color inactiveColor = Color.gray;

    void OnEnable()
    {
        StarManagerEarthquake2 starManagerEarthquake2 = FindObjectOfType<StarManagerEarthquake2>();

        if (starManagerEarthquake2 != null)
        {
            int currentStars = starManagerEarthquake2.GetCurrentStars();
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
