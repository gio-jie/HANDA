using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableTrash : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 startPos;
    private Vector3 startScale;

    public WasteTask wasteTask;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        startPos = rectTransform.anchoredPosition;
        startScale = transform.localScale;
    }

    public void OnBeginDrag(PointerEventData eventData) { }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition +=
            eventData.delta / canvas.scaleFactor;

        wasteTask.CheckHover(rectTransform);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (wasteTask.IsInsideTrash(rectTransform))
        {
            wasteTask.StartCoroutine(wasteTask.AbsorbTrash(this));
        }
        else
        {
            rectTransform.anchoredPosition = startPos;
            wasteTask.CheckHover(rectTransform); // snap back trash can
        }
    }

    public void ResetTrash()
    {
        gameObject.SetActive(true);
        rectTransform.anchoredPosition = startPos;
        transform.localScale = startScale;
    }
}