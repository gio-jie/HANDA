using UnityEngine;
using UnityEngine.EventSystems;

public class FlashlightFollow : MonoBehaviour
{
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        // Kapag naka-click ang mouse o nakadiin ang daliri sa screen
        if (Input.GetMouseButton(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved))
        {
            // Sundan ang position ng mouse/daliri
            transform.position = Input.mousePosition;
        }
    }
}