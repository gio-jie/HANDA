using UnityEngine;
using System.Collections;

public class TrashItem : MonoBehaviour
{
    [Header("Trash Info")]
    public string itemName;
    public Sprite itemSprite;
    [TextArea] public string description;

    [Header("Task List")]
    public int taskIndex;

    private bool collected = false;
    private bool isPopping = false;
    public void CollectTrash()
    {
        if (collected) return;

        collected = true;

        if (!isPopping)
            StartCoroutine(PopAndCollect());
    }

    private IEnumerator PopAndCollect()
    {
        isPopping = true;

        Vector3 originalScale = transform.localScale;
        Vector3 popScale = originalScale * 1.5f;
        float duration = 0.2f;
        float halfDuration = duration / 2f;
        float t = 0f;

        while (t < halfDuration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, popScale, t / halfDuration);
            yield return null;
        }

        t = 0f;
        while (t < halfDuration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(popScale, originalScale, t / halfDuration);
            yield return null;
        }

        transform.localScale = originalScale;

        if (AudioManager.instance != null)
            AudioManager.instance.PlaySFX(AudioManager.instance.correctSound);

        EndPanelManager_Level10.Instance.AddCollectedItem(itemName, itemSprite, description);
        TaskListManager_Level10.Instance.CompleteTask(taskIndex);
        StarManagerLevel10.Instance.TrashCollected();

        gameObject.SetActive(false);

        isPopping = false;
    }
}