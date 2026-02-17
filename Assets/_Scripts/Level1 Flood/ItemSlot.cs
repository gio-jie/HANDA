using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemSlot : MonoBehaviour
{
    public Image icon;

    ItemData item;
    Vector3 originalScale;
    Coroutine popRoutine;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    public void Setup(ItemData data)
    {
        item = data;
        icon.sprite = data.icon;
    }

    public void OnClick()
    {
        if(popRoutine != null)
            StopCoroutine(popRoutine);

        AudioManager.instance.PlaySFX(AudioManager.instance.clickSound);
        popRoutine = StartCoroutine(PopThenShow());
    }

    IEnumerator PopThenShow()
    {
        float duration = 0.08f;
        float t = 0f;

        Vector3 pressed = originalScale * 0.85f;

        while(t < duration)
        {
            t += Time.unscaledDeltaTime;
            transform.localScale = Vector3.Lerp(originalScale, pressed, t / duration);
            yield return null;
        }

        t = 0f;

        while(t < duration)
        {
            t += Time.unscaledDeltaTime;
            transform.localScale = Vector3.Lerp(pressed, originalScale, t / duration);
            yield return null;
        }

        transform.localScale = originalScale;

        ItemInfoPanel.Instance.Show(item);
    }
}