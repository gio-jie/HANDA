using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ObjectiveIcon : MonoBehaviour
{
    public string itemID;
    public Image icon;

    public Color silhouetteColor = Color.black;
    public Color completeColor = Color.white;

    void Awake()
    {
        icon.color = silhouetteColor;
    }

    public void Complete()
    {
        icon.color = completeColor;

        StopAllCoroutines();
        StartCoroutine(PopAnimation());
    }

    private IEnumerator PopAnimation()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 1.3f;
        float duration = 0.15f;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t / duration);
            yield return null;
        }

        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, t / duration);
            yield return null;
        }

        transform.localScale = originalScale;
    }

    public void ResetSilhouette()
    {
        icon.color = silhouetteColor;
        transform.localScale = Vector3.one;
    }
}