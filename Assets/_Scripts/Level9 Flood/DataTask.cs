using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class DataTask : MonoBehaviour
{
    [Header("UI References")]
    public Slider slider;
    public Button actionButton;
    public Image buttonImage;
    public Image folderImage;

    [Header("Folder Sprites")]
    public Sprite normalFolder;
    public Sprite downloadFolder;
    public Sprite uploadFolder;

    [Header("Button Sprites")]
    public Sprite downloadButtonSprite;
    public Sprite uploadButtonSprite;

    [Header("Settings")]
    public float fillDuration = 3f;

    [Header("SFX")]
    public AudioClip processSFX;
    public AudioClip processDoneSFX;
    private AudioSource audioSource;

    private bool downloadDone = false;
    private bool uploadDone = false;
    private bool isProcessing = false;

    private Coroutine fillCoroutine;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;
    }

    void OnEnable()
    {
        if (downloadDone && uploadDone)
        {
            slider.value = 1f;
            slider.gameObject.SetActive(true);
            actionButton.gameObject.SetActive(false);
            folderImage.sprite = normalFolder;
        }
        else if (downloadDone)
        {
            slider.value = 0f;
            slider.gameObject.SetActive(false);
            actionButton.gameObject.SetActive(true);
            folderImage.sprite = uploadFolder;
            if (buttonImage != null) buttonImage.sprite = uploadButtonSprite;
        }
        else
        {
            slider.value = 0f;
            slider.gameObject.SetActive(false);
            actionButton.gameObject.SetActive(true);
            folderImage.sprite = downloadFolder;
            if (buttonImage != null) buttonImage.sprite = downloadButtonSprite;
        }
    }

    public void OnActionButtonClick()
    {
        if (isProcessing) return;

        actionButton.gameObject.SetActive(false);
        slider.gameObject.SetActive(true);

        fillCoroutine = StartCoroutine(FillSlider());
    }

    private IEnumerator FillSlider()
    {
        isProcessing = true;

        float timer = 0f;
        slider.value = 0f;

        if (processSFX != null)
            audioSource.PlayOneShot(processSFX, 20f);

        while (timer < fillDuration)
        {
            timer += Time.deltaTime;
            slider.value = Mathf.Clamp01(timer / fillDuration);
            yield return null;
        }

        if (!downloadDone)
        {
            if (processDoneSFX != null)
                audioSource.PlayOneShot(processDoneSFX, 3f);

            downloadDone = true;
            StarManagerLevel9.Instance.SetDataPartial();

            slider.gameObject.SetActive(false);
            actionButton.gameObject.SetActive(true);

            folderImage.sprite = uploadFolder;
            if (buttonImage != null) buttonImage.sprite = uploadButtonSprite;
        }
        else if (!uploadDone)
        {
            if (processDoneSFX != null)
                audioSource.PlayOneShot(processDoneSFX, 3f);

            uploadDone = true;
            StarManagerLevel9.Instance.CompleteTask("Data");

            slider.value = 1f;
            folderImage.sprite = normalFolder;

            Level9PanelManager.Instance.CloseCurrentPanel();
        }

        isProcessing = false;
    }

    public void ResetIfNotFinished()
    {
        if (isProcessing)
        {
            StopCoroutine(fillCoroutine);
            slider.value = 0f;
            isProcessing = false;

            slider.gameObject.SetActive(false);
            actionButton.gameObject.SetActive(true);

            if (!downloadDone)
            {
                folderImage.sprite = downloadFolder;
                if (buttonImage != null) buttonImage.sprite = downloadButtonSprite;
            }
            else
            {
                folderImage.sprite = uploadFolder;
                if (buttonImage != null) buttonImage.sprite = uploadButtonSprite;
            }
        }
    }

    public bool IsCompleted()
    {
        return downloadDone && uploadDone;
    }
}