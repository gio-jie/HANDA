using UnityEngine;

public class BackgroundScrollerVerticalBG1 : MonoBehaviour
{
    [Header("Settings")]
    public float bgHeight = 10f;

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        if (mainCam == null) return;

        float camBottomY = mainCam.transform.position.y - mainCam.orthographicSize;

        if (transform.position.y + bgHeight / 2f < camBottomY)
        {
            Destroy(gameObject);
        }
    }
}