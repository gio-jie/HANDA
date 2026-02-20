using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider2D))]
public class ObjectiveItem : MonoBehaviour
{
    public string itemID;
    public bool isRequiredItem = true;

    public Transform bagTarget;

    bool collected = false;
    bool isAnimating = false;
    Vector3 baseScale;

    public Canvas canvas;
    public GameObject flyingItemPrefab;

    void Start()
    {
        baseScale = transform.localScale;
    }

    void OnMouseDown()
    {
        if (GameManager.Instance != null && GameManager.Instance.isPaused)
            return;

        AudioManager.instance.PlaySFX(AudioManager.instance.clickSound);
        Collect();
    }

    public void Collect()
    {
        if (collected || isAnimating) return;

        if (isRequiredItem)
        {
            StartCoroutine(CollectRequired());
        }
        else
        {
            StartCoroutine(WrongItemFlow());
        }
    }

    IEnumerator CollectRequired()
    {
        collected = true;

        yield return StartCoroutine(PopEffect());
        AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);

        StarManager.Instance.RegisterCorrectItem();

        ObjectiveManager.Instance.CollectItem(itemID);
        InventoryManager.Instance.CollectItem(itemID);

        Destroy(gameObject);
    }

    IEnumerator WrongItemFlow()
    {
        yield return StartCoroutine(PopEffect());
        AudioManager.instance.PlaySFX(AudioManager.instance.warningSound);
        WrongItemPanel.Instance.Show(this);
    }

    public void ConfirmWrongCollection()
    {
        if (collected || isAnimating) return;

        StarManager.Instance.RegisterWrongItem();

        collected = true;
        StartCoroutine(FlyToBagUI());
    }

    IEnumerator PopEffect()
    {
        isAnimating = true;

        float duration = 0.1f;
        Vector3 bigger = baseScale * 1.2f;

        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(baseScale, bigger, t / duration);
            yield return null;
        }

        t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(bigger, baseScale, t / duration);
            yield return null;
        }

        transform.localScale = baseScale;
        isAnimating = false;
    }

    IEnumerator FlyToBagUI()
    {
        SetInputState(false);
        isAnimating = true;

        BagUI.Instance.CloseBag();
        yield return new WaitForSeconds(0.1f);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Sprite itemSprite = sr.sprite;
        if (sr != null) sr.enabled = false;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        GameObject tempUI = Instantiate(flyingItemPrefab, canvas.transform);
        Image img = tempUI.GetComponent<Image>();
        img.sprite = itemSprite;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        tempUI.transform.position = screenPos;

        Vector3 startScale = tempUI.transform.localScale;
        Vector3 targetScale = startScale * 0.5f;

        Vector3 startPos = tempUI.transform.position;
        Vector3 endPos = bagTarget.position;

        float duration = 1f;
        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            float eased = Mathf.SmoothStep(0, 1, t / duration);
            tempUI.transform.position = Vector3.Lerp(startPos, endPos, eased);
            tempUI.transform.localScale = Vector3.Lerp(startScale, targetScale, eased);
            yield return null;
        }

        tempUI.transform.position = endPos;
        tempUI.transform.localScale = targetScale;

        float popDuration = 0.15f;
        t = 0;
        Vector3 popScale = targetScale * 0.3f;
        while (t < popDuration)
        {
            t += Time.deltaTime;
            tempUI.transform.localScale = Vector3.Lerp(targetScale, popScale, t / popDuration);
            yield return null;
        }

        yield return new WaitForSeconds(0.05f);
        BagUI.Instance.OpenBag();

        Destroy(tempUI);
        ObjectiveManager.Instance.CollectItem(itemID);
        InventoryManager.Instance.CollectItem(itemID);

        isAnimating = false;
        Destroy(gameObject);
        SetInputState(true);
    }

    void SetInputState(bool state)
    {
        Collider2D[] colliders = FindObjectsOfType<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = state;
        }

        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button btn in buttons)
        {
            btn.interactable = state;
        }
    }
}
