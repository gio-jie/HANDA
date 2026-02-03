using UnityEngine;
using System.Collections;

public class PanelAnimator : MonoBehaviour
{
    private RectTransform rect;
    private Vector2 panelOriginalPos;

    public RectTransform bagIcon;
    private Vector2 bagOriginalPos;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        panelOriginalPos = rect.anchoredPosition;

        if (bagIcon != null)
            bagOriginalPos = bagIcon.anchoredPosition;
    }

    public void ShowPanel()
    {
        gameObject.SetActive(true);
        if (bagIcon != null)
        {
            bagIcon.gameObject.SetActive(true);
            bagOriginalPos = bagIcon.anchoredPosition;
        }

        StopAllCoroutines();
        StartCoroutine(OpenFromTop());
    }

    public void HidePanel()
    {
        StopAllCoroutines();
        StartCoroutine(CloseToTop());
    }

    private IEnumerator OpenFromTop()
    {
        Vector2 panelStartPos = panelOriginalPos + new Vector2(0, rect.rect.height);
        rect.anchoredPosition = panelStartPos;

        Vector2 bagStartPos = bagOriginalPos + new Vector2(0, rect.rect.height);
        bagIcon.anchoredPosition = bagStartPos;

        float t = 0f;
        float duration = 0.3f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float factor = Mathf.Sin(t / duration * Mathf.PI * 0.5f);
            rect.anchoredPosition = Vector2.Lerp(panelStartPos, panelOriginalPos, factor);

            if (bagIcon != null)
                bagIcon.anchoredPosition = Vector2.Lerp(bagStartPos, bagOriginalPos, factor);

            yield return null;
        }

        rect.anchoredPosition = panelOriginalPos;
        if (bagIcon != null)
            bagIcon.anchoredPosition = bagOriginalPos;

        yield return StartCoroutine(BouncePosition(panelOriginalPos));
    }

    private IEnumerator CloseToTop()
    {
        Vector2 panelTargetPos = panelOriginalPos + new Vector2(0, rect.rect.height);
        Vector2 panelStartPos = panelOriginalPos;

        Vector2 bagStartPos = bagOriginalPos;
        Vector2 bagTargetPos = bagOriginalPos + new Vector2(0, rect.rect.height);

        float t = 0f;
        float duration = 0.2f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float factor = Mathf.Sin(t / duration * Mathf.PI * 0.5f);
            rect.anchoredPosition = Vector2.Lerp(panelStartPos, panelTargetPos, factor);

            if (bagIcon != null)
                bagIcon.anchoredPosition = Vector2.Lerp(bagStartPos, bagTargetPos, factor);

            yield return null;
        }

        rect.anchoredPosition = panelTargetPos;
        if (bagIcon != null)
            bagIcon.anchoredPosition = bagOriginalPos;

        gameObject.SetActive(false);
        if (bagIcon != null)
            bagIcon.gameObject.SetActive(false);
    }

    private IEnumerator BouncePosition(Vector2 targetPos)
    {
        float t = 0f;
        float duration = 0.1f;
        float bounceHeight = 10f;

        Vector2 startPos = targetPos;
        Vector2 peakPos = targetPos + new Vector2(0, bounceHeight);

        while (t < duration)
        {
            t += Time.deltaTime;
            rect.anchoredPosition = Vector2.Lerp(startPos, peakPos, Mathf.Sin(t / duration * Mathf.PI * 0.5f));
            yield return null;
        }

        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            rect.anchoredPosition = Vector2.Lerp(peakPos, startPos, Mathf.Sin(t / duration * Mathf.PI * 0.5f));
            yield return null;
        }

        rect.anchoredPosition = targetPos;
    }
}