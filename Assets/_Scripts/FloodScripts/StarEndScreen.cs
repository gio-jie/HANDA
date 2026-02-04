using UnityEngine; 
using UnityEngine.UI;

public class StarEndScreen : MonoBehaviour
{
    public int levelIndex;
    public Image[] stars;

    public Color activeColor = Color.white;
    public Color inactiveColor = Color.gray;

    void Start()
    {
        int savedStars = PlayerPrefs.GetInt("Level_" + levelIndex, 0);

        for (int i = 0; i < stars.Length; i++)
            stars[i].color = i < savedStars ? activeColor : inactiveColor;
    }
}