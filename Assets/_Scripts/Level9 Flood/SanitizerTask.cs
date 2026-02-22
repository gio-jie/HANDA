using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SanitizerTask : MonoBehaviour
{
    [Header("UI & Objects")]
    public Slider slider;
    public DraggableSanitizer bottle;

    [Header("Settings")]
    public float fillTime = 3f;

    [Header("SFX")]
    public AudioClip snapSFX;
    public AudioClip fillSFX;
    public float sfxVolume;

    public AudioSource audioSource;
    private bool filling;
    private bool isCompleted = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
    }

    void OnEnable()
    {
        if (isCompleted)
        {
            slider.value = 1f;
            return;
        }

        slider.value = 0f;
        filling = false;
    }

    public void OnBottleSnapped()
    {
        if (snapSFX != null)
            audioSource.PlayOneShot(snapSFX, sfxVolume);
    }

    public void StartFilling()
    {
        if (!filling && !isCompleted)
            StartCoroutine(Fill());
    }

    IEnumerator Fill()
    {
        filling = true;

        if (fillSFX != null)
            audioSource.PlayOneShot(fillSFX, sfxVolume);

        float timer = 0f;

        while (timer < fillTime)
        {
            timer += Time.deltaTime;
            slider.value = timer / fillTime;
            yield return null;
        }

        isCompleted = true;
        filling = false;

        yield return new WaitForSeconds(0.3f);

        StarManagerLevel9.Instance.CompleteTask("Sanitizer");

        gameObject.SetActive(false);
    }

    public void ResetTask()
    {
        if (isCompleted) return;

        filling = false;
        slider.value = 0f;
        StopAllCoroutines();

        bottle.ResetBottle();
    }

    public bool IsCompleted()
    {
        return isCompleted;
    }
}