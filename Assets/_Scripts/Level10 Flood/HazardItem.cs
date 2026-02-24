using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

public class HazardItem : MonoBehaviour
{
    public string hazardName;
    public Sprite hazardSprite;
    [TextArea] public string description;

    private bool isAnimating = false;
    private Transform originalParent;
    private Vector3 originalPosition;

    private EventSystem eventSystem;

    void Awake()
    {
        eventSystem = EventSystem.current;
    }

    public void TriggerHazard()
    {
        if (isAnimating) return;
        StartCoroutine(FlashThenShowQuestion());
    }

    private IEnumerator FlashThenShowQuestion()
    {
        isAnimating = true;

        FreezeGame();

        yield return StartCoroutine(FlashRed());

        UnfreezeGame();

        isAnimating = false;

        QuizManager_Level10.Instance.TryShowRandomQuestion(this);
    }

    private IEnumerator FlashRed(float singleFlashDuration = 0.2f, int flashCount = 2)
    {
        if (AudioManager.instance != null)
            AudioManager.instance.PlaySFX(AudioManager.instance.warningSound);
            
        Image img = GetComponent<Image>();
        if (img == null) yield break;

        Color original = img.color;
        Color red = Color.red;

        for (int i = 0; i < flashCount; i++)
        {
            float half = singleFlashDuration / 2f;
            float t = 0f;

            while (t < half)
            {
                t += Time.unscaledDeltaTime;
                img.color = Color.Lerp(original, red, t / half);
                yield return null;
            }

            t = 0f;

            while (t < half)
            {
                t += Time.unscaledDeltaTime;
                img.color = Color.Lerp(red, original, t / half);
                yield return null;
            }
        }

        img.color = original;
    }

    public void CollectHazard()
    {
        if (isAnimating) return;
        StartCoroutine(PopAndFlySequence());
    }

    private IEnumerator PopAndFlySequence()
    {
        isAnimating = true;

        FreezeGame();

        originalParent = transform.parent;
        originalPosition = transform.position;

        Canvas mainCanvas = UIReferences_Level10.Instance.mainCanvas;
        transform.SetParent(mainCanvas.transform);
        transform.SetAsLastSibling();

        Vector3 originalScale = transform.localScale;
        Vector3 popScale = originalScale * 1.3f;

        float popDuration = 0.12f;
        float t = 0f;

        while (t < popDuration)
        {
            t += Time.unscaledDeltaTime;
            transform.localScale = Vector3.Lerp(originalScale, popScale, t / popDuration);
            yield return null;
        }

        t = 0f;

        while (t < popDuration)
        {
            t += Time.unscaledDeltaTime;
            transform.localScale = Vector3.Lerp(popScale, originalScale, t / popDuration);
            yield return null;
        }

        Transform trashIcon = UIReferences_Level10.Instance.trashIcon;

        Vector3 startPos = transform.position;
        Vector3 endPos = trashIcon.position;

        float flyDuration = 0.5f;
        float arcHeight = 100f;

        t = 0f;

        while (t < flyDuration)
        {
            t += Time.unscaledDeltaTime;
            float progress = t / flyDuration;

            Vector3 currentPos = Vector3.Lerp(startPos, endPos, progress);
            float height = 4 * arcHeight * progress * (1 - progress);
            currentPos.y += height;

            transform.position = currentPos;
            yield return null;
        }

        transform.position = endPos;

        yield return new WaitForSecondsRealtime(0.15f);

        float shrinkDuration = 0.2f;
        t = 0f;

        while (t < shrinkDuration)
        {
            t += Time.unscaledDeltaTime;
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t / shrinkDuration);
            yield return null;
        }

        transform.localScale = Vector3.zero;

        EndPanelManager_Level10.Instance.AddCollectedItem(
            hazardName,
            hazardSprite,
            description
        );

        gameObject.SetActive(false);

        transform.SetParent(originalParent);
        transform.position = originalPosition;

        UnfreezeGame();

        isAnimating = false;
    }

    private void FreezeGame()
    {
        Time.timeScale = 0f;

        if (eventSystem != null)
            eventSystem.enabled = false;
    }

    private void UnfreezeGame()
    {
        Time.timeScale = 1f;

        if (eventSystem != null)
            eventSystem.enabled = true;
    }
}