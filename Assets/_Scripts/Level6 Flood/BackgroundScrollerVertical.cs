using UnityEngine;

public class BackgroundScrollerVertical : MonoBehaviour
{
    public float skyStartHeight = 20f;
    public GameObject skyBackground;

    void Update()
    {
        if (transform.position.y > skyStartHeight)
        {
            skyBackground.SetActive(true);
        }
        else
        {
            skyBackground.SetActive(false);
        }
    }
}
