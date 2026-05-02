using UnityEngine;
using UnityEngine.UI;

public class StarEndScreenEarthquake5 : MonoBehaviour
{
    public Image[] stars;
    public Color activeColor = Color.white;
    public Color inactiveColor = Color.gray;

    void OnEnable()
    {
        StarManagerEarthquake5 starManagerEarthquake5 = FindObjectOfType<StarManagerEarthquake5>();

        if (starManagerEarthquake5 != null)
        {
            int currentStars = starManagerEarthquake5.GetCurrentStars();
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
