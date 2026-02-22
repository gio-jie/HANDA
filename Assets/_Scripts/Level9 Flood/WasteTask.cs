using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WasteTask : MonoBehaviour
{
    [Header("Trash Can")]
    public Image trashCan;
    public Sprite openSprite;
    public Sprite closedSprite;
    public float hoverScale = 1.15f;

    [Header("Trash Bags")]
    public List<DraggableTrash> trashBags;

    [Header("SFX")]
    public AudioClip dropTrashSFX;
    public float sfxVolume;
    private AudioSource audioSource;

    private Vector3 originalScale;
    private int trashCount = 0;
    private bool isAnimating = false;
    private bool isCompleted = false;

    public bool IsCompleted() => isCompleted;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
    }

    void OnEnable()
    {
        originalScale = trashCan.transform.localScale;
        trashCan.sprite = closedSprite;
    }

    public void CheckHover(RectTransform trash)
    {
        if (isAnimating) return;

        if (IsInsideTrash(trash))
        {
            trashCan.sprite = openSprite;
            trashCan.transform.localScale = originalScale * hoverScale;
        }
        else
        {
            trashCan.sprite = closedSprite;
            trashCan.transform.localScale = originalScale;
        }
    }

    public bool IsInsideTrash(RectTransform trash)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            trashCan.rectTransform,
            trash.position,
            null);
    }

    public IEnumerator AbsorbTrash(DraggableTrash trash)
    {
        if (isAnimating) yield break;

        isAnimating = true;

        if (dropTrashSFX != null)
            audioSource.PlayOneShot(dropTrashSFX, sfxVolume);

        trashCan.sprite = openSprite;
        trashCan.transform.localScale = originalScale * hoverScale;

        float timer = 0f;
        Vector3 startScale = trash.transform.localScale;

        while (timer < 0.25f)
        {
            timer += Time.deltaTime;
            trash.transform.localScale =
                Vector3.Lerp(startScale, Vector3.zero, timer / 0.25f);
            yield return null;
        }

        trash.gameObject.SetActive(false);
        trashCount++;

        yield return new WaitForSeconds(0.15f);

        trashCan.sprite = closedSprite;
        trashCan.transform.localScale = originalScale;

        isAnimating = false;

        if (trashCount >= 3)
        {
            yield return new WaitForSeconds(0.3f);
            StarManagerLevel9.Instance.CompleteTask("Waste");
            isCompleted = true;
            gameObject.SetActive(false);
        }
    }

    public void ResetTask()
    {
        if (isCompleted) return;

        trashCount = 0;
        isAnimating = false;

        trashCan.sprite = closedSprite;
        trashCan.transform.localScale = originalScale;

        foreach (DraggableTrash trash in trashBags)
        {
            trash.ResetTrash();
        }
    }
}