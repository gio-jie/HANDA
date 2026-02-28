using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WaterTask : MonoBehaviour
{
    public Image fillImage;
    [Header("SFX")]
    public AudioClip fillSFX;       // looping fill sound
    [Range(0f, 10f)] public float sfxVolume; // higher volume

    private AudioSource audioSource;

    private float holdTime = 5f;
    private float timer;
    private bool holding;
    private bool isCompleted = false;
    private bool fillSfxPlaying = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = true;  // loop while filling
        audioSource.spatialBlend = 0f;
    }

    void OnEnable()
    {
        if (isCompleted)
        {
            fillImage.fillAmount = 1f;
        }
        else
        {
            timer = 0f;
            fillImage.fillAmount = 0f;
            StopFillSFX();
        }
    }

    void Update()
    {
        if (!holding || isCompleted) return;

        // Start the fill SFX once
        if (!fillSfxPlaying && fillSFX != null)
        {
            audioSource.clip = fillSFX;
            audioSource.volume = sfxVolume;
            audioSource.Play();
            fillSfxPlaying = true;
        }

        timer += Time.deltaTime;
        fillImage.fillAmount = timer / holdTime;

        if (timer >= holdTime)
        {
            holding = false;
            StartCoroutine(FinishTaskWithDelay(0.2f)); // wait 0.3s before closing panel
        }
    }

    private IEnumerator FinishTaskWithDelay(float delay)
    {
        isCompleted = true;

        // Ensure fill is at 100%
        fillImage.fillAmount = 1f;

        // Stop SFX smoothly after delay
        yield return new WaitForSeconds(delay);

        StopFillSFX();

        StarManagerLevel9.Instance.CompleteTask("Water");
        Level9PanelManager.Instance.CloseCurrentPanel();
    }

    public void StartFill()
    {
        if (!isCompleted)
            holding = true;
    }

    public void StopFill()
    {
        holding = false;
        StopFillSFX();
    }

    void StopFillSFX()
    {
        if (fillSfxPlaying)
        {
            audioSource.Stop();
            fillSfxPlaying = false;
        }
    }

    public void ResetTask()
    {
        if (isCompleted) return;

        holding = false;
        timer = 0f;
        fillImage.fillAmount = 0f;
        StopFillSFX();
    }

    public bool IsCompleted()
    {
        return isCompleted;
    }
}