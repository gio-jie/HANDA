using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class WaterCardUI : MonoBehaviour
{
    public Image image;
    private Color originalColor;
    public CanvasGroup canvasGroup;

    private WaterCardData data;
    private Vector3 startPos;
    private Vector3 startScale;

    void Awake()
    {
        startPos = transform.position;
        startScale = transform.localScale;
        originalColor = image.color;
    }

    public IEnumerator FlashRed(int flashCount = 2, float flashSpeed = 0.15f)
    {
        for (int i = 0; i < flashCount; i++)
        {
            image.color = Color.red;
            yield return new WaitForSeconds(flashSpeed);

            image.color = originalColor;
            yield return new WaitForSeconds(flashSpeed);
        }
    }

    public void Setup(WaterCardData cardData)
    {
        data = cardData;
        image.sprite = data.image;
        transform.position = startPos;
        transform.localScale = startScale;
        if(canvasGroup != null) canvasGroup.alpha = 1f;
    }

    public void SwipeRight()
    {
        WaterSwipeLevelManager.Instance.SubmitAnswer(data, true);
        StartCoroutine(AnimateToBack(Vector3.right));
    }

    public void SwipeLeft()
    {
        WaterSwipeLevelManager.Instance.SubmitAnswer(data, false);
        StartCoroutine(AnimateToBack(Vector3.left));
    }

    public void ResetPosition()
    {
        transform.position = startPos;
        transform.localScale = startScale;
    }

    public IEnumerator AnimateToBack(Vector3 direction)
    {
        Vector3 start = transform.position;
        Vector3 end = start + direction * 600f;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = startScale * 0.7f;

        float t = 0f;
        float duration = 0.25f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;

            transform.position = Vector3.Lerp(start, end, progress);
            transform.localScale = Vector3.Lerp(startScale, endScale, progress);

            if (canvasGroup != null)
                canvasGroup.alpha = 1 - progress;

            yield return null;
        }
    }
}