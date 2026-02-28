using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.UI;

public class PatternIconUI : MonoBehaviour
{
    public Image iconImage;
    public GameObject checkMark;

    private Vector3 originalScale;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        originalScale = transform.localScale;
        checkMark.SetActive(false);
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetSprite(Sprite sprite)
    {
        iconImage.sprite = sprite;
    }

    public void StartFlashing()
    {
        StartCoroutine(ScalePulse());
    }

    IEnumerator ScalePulse()
    {
        while (true)
        {
            transform.localScale = originalScale * 1.08f;
            yield return new WaitForSeconds(0.4f);
            transform.localScale = originalScale;
            yield return new WaitForSeconds(0.4f);
        }
    }

    public void MarkComplete()
    {
        checkMark.SetActive(true);
    }

    public void ResetMark()
    {
        checkMark.SetActive(false);
    }

    public IEnumerator AnimateIn()
    {
        canvasGroup.alpha = 0f;
        transform.localScale = Vector3.zero;

        float duration = 0.25f;
        float time = 0;

        while (time < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, time / duration);
            transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, time / duration);

            time += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f;
        transform.localScale = originalScale;
    }

    public IEnumerator AnimateOut()
    {
        float duration = 0.2f;
        float time = 0;

        while (time < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, time / duration);

            time += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }

    public void SetInvisibleInstant()
    {
        canvasGroup.alpha = 0f;
        transform.localScale = originalScale;
    }
}