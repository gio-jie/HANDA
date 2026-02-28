using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BagAnimator : MonoBehaviour
{
    [Header("Initial Delay")]
    public float startDelay = 1.5f;

    [Header("Wave Animation")]
    public float waveDuration = 1.2f;
    public float waveSpeed = 8f;
    public float waveAngle = 15f;

    [Header("Bag Movement")]
    public RectTransform bagRect;
    public RectTransform targetPosition;
    public Vector3 targetScale = Vector3.one;
    public float moveTime = 0.5f;

    [Header("Bag Fall")]
    public float bagFallDistance = 300f;
    public float bagFallTime = 0.4f;

    [Header("Items Panel")]
    public GameObject itemsPanel;

    private CanvasGroup bagCanvasGroup;

    private void Awake()
    {
        bagCanvasGroup = bagRect.GetComponent<CanvasGroup>();

        if (itemsPanel != null)
            itemsPanel.SetActive(false);
    }

    public void AnimateBag()
    {
        StartCoroutine(FullSequence());
    }

    private IEnumerator FullSequence()
    {
        yield return new WaitForSeconds(startDelay);

        yield return StartCoroutine(WaveBag());

        yield return StartCoroutine(MoveBag());

        yield return StartCoroutine(BagFallAndFade());

        ShowItemsPanel();
    }

    private IEnumerator WaveBag()
    {
        float elapsed = 0f;

        while (elapsed < waveDuration)
        {
            elapsed += Time.deltaTime;

            float angle = Mathf.Sin(Time.time * waveSpeed) * waveAngle;

            bagRect.localRotation = Quaternion.Euler(0f, 0f, angle);

            yield return null;
        }

        bagRect.localRotation = Quaternion.identity;
    }

    private IEnumerator MoveBag()
    {
        Vector2 startPos = bagRect.anchoredPosition;
        Vector3 startScale = bagRect.localScale;

        float elapsed = 0f;

        while (elapsed < moveTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveTime;

            t = Mathf.Sin(t * Mathf.PI * 0.5f);

            bagRect.anchoredPosition =
                Vector2.Lerp(startPos, targetPosition.anchoredPosition, t);

            bagRect.localScale =
                Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }

        bagRect.anchoredPosition = targetPosition.anchoredPosition;
        bagRect.localScale = targetScale;
    }

    private IEnumerator BagFallAndFade()
    {
        Vector2 startPos = bagRect.anchoredPosition;
        Vector2 endPos = startPos - new Vector2(0, bagFallDistance);

        float elapsed = 0f;

        while (elapsed < bagFallTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / bagFallTime;

            t = 1 - Mathf.Pow(1 - t, 3);

            bagRect.anchoredPosition =
                Vector2.Lerp(startPos, endPos, t);

            if (bagCanvasGroup != null)
                bagCanvasGroup.alpha = 1 - t;

            yield return null;
        }

        Image bagImage = bagRect.GetComponent<Image>();
        if (bagImage != null)
            bagImage.enabled = false;
    }

    private void ShowItemsPanel()
    {
        itemsPanel.SetActive(true);
        StartCoroutine(PopItemsSequentially());
    }

    private IEnumerator PopItemsSequentially()
    {
        ItemPopUp[] itemPopUps =
            itemsPanel.GetComponentsInChildren<ItemPopUp>();

        foreach (var item in itemPopUps)
        {
            item.PopUp();
            yield return new WaitForSeconds(0.1f);
        }
    }
}