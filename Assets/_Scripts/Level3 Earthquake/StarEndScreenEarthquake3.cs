using UnityEngine;
using UnityEngine.UI;

public class StarEndScreenEarthquake3 : MonoBehaviour
{
    public Image[] stars;
    public Color activeColor = Color.white;
    public Color inactiveColor = Color.gray;

    void OnEnable()
    {
        StarManagerEarthquake3 starManagerEarthquake3 = FindObjectOfType<StarManagerEarthquake3>();

        if (starManagerEarthquake3 != null)
        {
            int currentStars = starManagerEarthquake3.GetCurrentStars();
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
