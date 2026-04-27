using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Image image;

    private Vector2 startPosition;

    public RectTransform correctPlace;
    public float snapDistance = 100f;

    public GameObject arrowIndicator;
    public GameObject checkIconUI;

    [Header("Feedback")]
    public float flashDuration = 0.15f;
    public int flashCount = 3;
    public Color flashColor = Color.red;

    private bool isPlacedCorrectly = false;
    private Color originalColor;
    private Coroutine flashCoroutine;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        image = GetComponent<Image>();

        if (image != null)
            originalColor = image.color;
    }

    void Start()
    {
        startPosition = rectTransform.anchoredPosition;
        arrowIndicator.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isPlacedCorrectly) return;

        arrowIndicator.SetActive(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isPlacedCorrectly) return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        arrowIndicator.SetActive(false);

        float distance = Vector2.Distance(rectTransform.anchoredPosition, correctPlace.anchoredPosition);

        if (distance <= snapDistance)
        {
            SnapToCorrectPlace();
        }
        else
        {
            ReturnToStart();
        }
    }

    void SnapToCorrectPlace()
    {
        rectTransform.anchoredPosition = correctPlace.anchoredPosition;
        isPlacedCorrectly = true;

        StarManagerEarthquake1.Instance.RegisterCorrect();
        StarManagerEarthquake1.Instance.RegisterTaskComplete();

        checkIconUI.SetActive(true);

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    void ReturnToStart()
    {
        rectTransform.anchoredPosition = startPosition;

        StarManagerEarthquake1.Instance.RegisterWrong();

        PlayFlashEffect();
    }

    void PlayFlashEffect()
    {
        if (image == null) return;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        for (int i = 0; i < flashCount; i++)
        {
            image.color = flashColor;
            yield return new WaitForSeconds(flashDuration);

            image.color = originalColor;
            yield return new WaitForSeconds(flashDuration);
        }

        image.color = originalColor;
    }
}