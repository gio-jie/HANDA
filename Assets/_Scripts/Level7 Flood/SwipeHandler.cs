using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector2 startPos;
    private WaterCardUI cardUI;
    public float minSwipeDistance = 100f;

    void Awake()
    {
        cardUI = GetComponent<WaterCardUI>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        startPos = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        float deltaX = eventData.position.x - startPos.x;

        if (deltaX > minSwipeDistance)
            cardUI.SwipeRight();
        else if (deltaX < -minSwipeDistance)
            cardUI.SwipeLeft();
        else
            cardUI.ResetPosition();
    }
}