using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class HazardItem : MonoBehaviour
{
    public string hazardName;
    public Sprite hazardSprite;
    [TextArea] public string description;

    private bool isAnimating = false;
    private Transform originalParent; // store original parent
    private Vector3 originalPosition;

    public void TriggerHazard()
    {
        if (PowerUpManager_Level10.Instance.SkipNextHazard())
            return;

        QuizManager_Level10.Instance.TryShowRandomQuestion(this);
    }

    public void CollectHazard()
    {
        if (isAnimating) return;
        StartCoroutine(PopAndFlySequence());
    }

    private IEnumerator PopAndFlySequence()
    {
        isAnimating = true;

        // Store original parent and position
        originalParent = transform.parent;
        originalPosition = transform.position;

        // Move to main canvas so it appears above everything
        Canvas mainCanvas = UIReferences_Level10.Instance.mainCanvas;
        transform.SetParent(mainCanvas.transform);
        transform.SetAsLastSibling();

        // Disable all other UI interaction
        CanvasGroup[] groups = FindObjectsOfType<CanvasGroup>();
        foreach (var g in groups)
            g.interactable = false;

        Vector3 originalScale = transform.localScale;
        Vector3 popScale = originalScale * 1.3f;

        float popDuration = 0.12f;
        float t = 0f;

        // -------- POP --------
        while (t < popDuration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, popScale, t / popDuration);
            yield return null;
        }

        t = 0f;
        while (t < popDuration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(popScale, originalScale, t / popDuration);
            yield return null;
        }

        // -------- FLY (ARC STYLE) --------
        Transform trashIcon = UIReferences_Level10.Instance.trashIcon;

        Vector3 startPos = transform.position;
        Vector3 endPos = trashIcon.position;

        float flyDuration = 0.5f;
        float arcHeight = 100f;

        t = 0f;
        while (t < flyDuration)
        {
            t += Time.deltaTime;
            float progress = t / flyDuration;

            Vector3 currentPos = Vector3.Lerp(startPos, endPos, progress);
            float height = 4 * arcHeight * progress * (1 - progress);
            currentPos.y += height;

            transform.position = currentPos;
            yield return null;
        }

        // Ensure exact landing
        transform.position = endPos;

        // -------- STOP MOMENT --------
        yield return new WaitForSeconds(0.15f);

        // -------- SHRINK --------
        float shrinkDuration = 0.2f;
        t = 0f;
        while (t < shrinkDuration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t / shrinkDuration);
            yield return null;
        }

        transform.localScale = Vector3.zero;

        // -------- FINALIZE --------
        EndPanelManager_Level10.Instance.AddCollectedItem(hazardName, hazardSprite, description);
        StarManagerLevel10.Instance.TrashCollected();

        gameObject.SetActive(false);

        // Restore parent in case object is reused
        transform.SetParent(originalParent);
        transform.position = originalPosition;

        // Re-enable UI interaction
        foreach (var g in groups)
            g.interactable = true;

        isAnimating = false;
    }
}