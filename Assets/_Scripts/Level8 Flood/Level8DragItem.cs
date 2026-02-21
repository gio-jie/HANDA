using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class Level8DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rect;
    private Canvas canvas;
    public string itemID;
    private Coroutine scaleCoroutine;
    private Vector3 originalScale;

    [Header("Optional Overlays")]
    public GameObject checkMark;    public GameObject crossMark;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        originalScale = rect.localScale;

        if (checkMark != null) checkMark.SetActive(false);
        if (crossMark != null) crossMark.SetActive(false);
    }

    public void PlayPowerupScaleOnly()
    {
        HideMarkers();
        StartScaleCoroutine(originalScale * 1.3f);
    }

    public void PlayPowerupCheck()
    {
        HideMarkers();
        if (checkMark != null) checkMark.SetActive(true);
        StartScaleCoroutine(originalScale * 1.3f);
    }

    public void PlayPowerupCross()
    {
        HideMarkers();
        if (crossMark != null) crossMark.SetActive(true);
        StartScaleCoroutine(originalScale * 0.7f);
    }

    private void StartScaleCoroutine(Vector3 targetScale)
    {
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);

        scaleCoroutine = StartCoroutine(ScaleTo(targetScale, 0.3f));
    }

    private IEnumerator ScaleTo(Vector3 target, float duration)
    {
        Vector3 start = rect.localScale;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            rect.localScale = Vector3.Lerp(start, target, t / duration);
            yield return null;
        }

        rect.localScale = target;
        scaleCoroutine = null;
    }

    public void ClearHighlight()
    {
        HideMarkers();
        StartScaleCoroutine(originalScale);
    }

    private void HideMarkers()
    {
        if (checkMark != null) checkMark.SetActive(false);
        if (crossMark != null) crossMark.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Level8DragManager.Instance.BeginDrag(this);

        LayoutElement le = GetComponent<LayoutElement>();
        if (le != null) le.ignoreLayout = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Level8DragManager.Instance.EndDrag(this, eventData);

        if (RectTransformUtility.RectangleContainsScreenPoint(
            Level8ScenarioManager.Instance.scenarioDisplay.rectTransform,
            eventData.position,
            canvas.worldCamera))
        {
            Level8ScenarioManager.Instance.OnItemDropped(this);
        }
        else
        {
            LayoutElement le = GetComponent<LayoutElement>();
            if (le != null) le.ignoreLayout = false;
        }
    }

    public RectTransform GetRect() => rect;
}