using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolItemLevel5 : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum ToolMode
    {
        Tutorial,
        Gameplay
    }

    public string toolID;
    public ToolMode toolMode = ToolMode.Gameplay;

    public RectTransform rect;
    public Canvas canvas;
    public CanvasGroup canvasGroup;

    public Vector3 originalScale;
    private Transform originalParent;

    private Vector2 pointerOffset;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        originalScale = transform.localScale;
        originalParent = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (toolMode == ToolMode.Tutorial)
        {
            FindObjectOfType<IntroManager>()?.ScalePlayerUp();
        }

        transform.SetAsLastSibling();
        transform.localScale = originalScale * 1.2f;

        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = false;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect,
            eventData.position,
            eventData.pressEventCamera,
            out pointerOffset
        );
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );

        rect.localPosition = localPoint - pointerOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = true;

        ToolDropHandlerLevel5.Instance.HandleDrop(this, eventData);
        FindObjectOfType<IntroManager>()?.ScalePlayerDown();
    }

    public void ResetToToolbar()
    {
        transform.SetParent(originalParent);

        transform.localScale = originalScale;

        rect.anchoredPosition = Vector2.zero;

        LayoutRebuilder.ForceRebuildLayoutImmediate(
            originalParent as RectTransform
        );
    }
}