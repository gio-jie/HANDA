using UnityEngine;
using System.Collections;

public class ItemCardUI : MonoBehaviour
{
    public SortingItemData data;

    public UnityEngine.UI.Image itemImage;
    public TMPro.TMP_Text itemName;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Setup(SortingItemData item)
    {
        data = item;

        itemImage.sprite = item.itemSprite;
        itemName.text = item.itemName;
    }

    public IEnumerator AnimateToBack()
    {
        Vector3 start = transform.position;
        Vector3 end = start + new Vector3(600f, 0f, 0f);

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