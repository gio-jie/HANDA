using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PowerUpManager_Level10 : MonoBehaviour
{
    public static PowerUpManager_Level10 Instance;

    [Header("Powerup UI")]
    public GameObject powerupPanel;
    public Image powerupImage;
    public TMPro.TMP_Text powerupNameText;

    [Header("Powerup Sprites")]
    public Sprite journalSprite;
    public Sprite hardHatSprite;
    public Sprite timeClockSprite;

    [Header("On-Screen Powerup Icon")]
    public Image powerupIcon;
    public Transform iconTarget;

    [Header("Timer Target (Time Clock)")]
    public Transform timerTarget;

    [Header("Canvas Reference")]
    public Canvas mainCanvas;

    [Header("Powerup SFX")]
    public AudioClip powerupGainSFX;
    public AudioClip powerupUseSFX;

    private AudioSource localAudioSource;
    private bool sfxPlayed = false;

    private bool hasJournal = false;
    private bool hasHardHat = false;
    private bool powerupActive = false;

    private Vector3 iconOriginalScale;
    private Transform iconOriginalParent;
    private Vector3 iconOriginalPosition;

    private Button[] cachedButtons;

    void Awake()
    {
        Instance = this;

        localAudioSource = gameObject.AddComponent<AudioSource>();
        localAudioSource.playOnAwake = false;

        if (mainCanvas != null)
            cachedButtons = mainCanvas.GetComponentsInChildren<Button>(true);

        if (powerupIcon != null)
        {
            powerupIcon.gameObject.SetActive(false);
            iconOriginalScale = powerupIcon.transform.localScale;
            iconOriginalParent = powerupIcon.transform.parent;
            iconOriginalPosition = powerupIcon.transform.localPosition;
        }
    }

    private void SetButtonsInteractable(bool state)
    {
        if (cachedButtons == null) return;

        foreach (Button btn in cachedButtons)
        {
            btn.interactable = state;
        }
    }

    public void GiveRandomPowerUp(bool guaranteed)
    {
        if (powerupActive) return;

        if (hasJournal || hasHardHat)
            return;

        if (StarManagerLevel10.Instance.IsLastTrash())
            return;

        if (!guaranteed && Random.value > 0.5f)
            return;

        int random = Random.Range(0, 3);
        powerupActive = true;

        switch (random)
        {
            case 0:
                hasJournal = true;
                StartCoroutine(ShowPowerUpUI(journalSprite, "Survival Tips Journal", false));
                break;

            case 1:
                StartCoroutine(ShowPowerUpUI(timeClockSprite, "Time Clock", true));
                break;

            case 2:
                hasHardHat = true;
                StartCoroutine(ShowPowerUpUI(hardHatSprite, "Hard Hat", false));
                break;
        }
    }

    public void ClearAllPowerUps()
    {
        hasJournal = false;
        hasHardHat = false;
        powerupActive = false;

        if (powerupIcon != null)
            powerupIcon.gameObject.SetActive(false);

        Time.timeScale = 1f;
    }

    public bool UseJournal()
    {
        if (!hasJournal) return false;
        hasJournal = false;
        StartCoroutine(PopIconBackToPanel());

        PlayPowerupSFX(powerupUseSFX);

        return true;
    }

    public bool SkipNextHazard()
    {
        if (!hasHardHat) return false;
        hasHardHat = false;
        StartCoroutine(PopIconBackToPanel());

        PlayPowerupSFX(powerupUseSFX);

        return true;
    }

    private void PlayPowerupSFX(AudioClip clip)
    {
        if (clip == null || sfxPlayed) return;

        localAudioSource.PlayOneShot(clip);
        sfxPlayed = true;
        StartCoroutine(ResetSFXChecker());
    }

    private IEnumerator ResetSFXChecker()
    {
        yield return null; // wait 1 frame
        sfxPlayed = false;
    }

    private IEnumerator ShowPowerUpUI(Sprite sprite, string powerupName, bool isTimeClock)
    {
        if (powerupPanel == null || powerupImage == null || powerupNameText == null || mainCanvas == null)
            yield break;

        SetButtonsInteractable(false);

        Time.timeScale = 0f;

        powerupPanel.SetActive(true);

        PlayPowerupSFX(powerupUseSFX);
        
        powerupImage.sprite = sprite;
        powerupNameText.text = powerupName;

        powerupIcon.sprite = sprite;
        powerupIcon.transform.SetParent(mainCanvas.transform, true);
        powerupIcon.transform.position = powerupPanel.transform.position;
        powerupIcon.transform.localScale = iconOriginalScale;
        powerupIcon.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(1f);

        powerupPanel.SetActive(false);

        if (isTimeClock)
        {
            yield return StartCoroutine(MoveIcon(powerupIcon.transform, timerTarget.position, 0.5f, 0.4f));
            yield return StartCoroutine(PopEffect(powerupIcon.transform));

            StarManagerLevel10.Instance.AddTime(10f);

            powerupIcon.gameObject.SetActive(false);
        }
        else
        {
            yield return StartCoroutine(MoveIcon(powerupIcon.transform, iconTarget.position, 0.5f, 0.35f));
        }

        Time.timeScale = 1f;

        powerupActive = false;
        SetButtonsInteractable(true);
    }

    private IEnumerator MoveIcon(Transform icon, Vector3 targetPos, float duration, float scaleFactor)
    {
        Vector3 startPos = icon.position;
        Vector3 startScale = icon.localScale;
        Vector3 targetScale = iconOriginalScale * scaleFactor;

        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float factor = t / duration;

            icon.position = Vector3.Lerp(startPos, targetPos, factor);
            icon.localScale = Vector3.Lerp(startScale, targetScale, factor);

            yield return null;
        }

        icon.position = targetPos;
        icon.localScale = targetScale;
    }

    private IEnumerator PopEffect(Transform icon)
    {
        Vector3 original = icon.localScale;
        Vector3 bigger = original * 1.4f;

        float duration = 0.15f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            icon.localScale = Vector3.Lerp(original, bigger, t / duration);
            yield return null;
        }

        t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            icon.localScale = Vector3.Lerp(bigger, original, t / duration);
            yield return null;
        }
    }

    private IEnumerator PopIconBackToPanel()
    {
        SetButtonsInteractable(false);
        Time.timeScale = 0f;

        Vector3 startScale = powerupIcon.transform.localScale;
        Vector3 targetScale = iconOriginalScale;
        Vector3 targetPos = powerupPanel.transform.position;

        float duration = 0.3f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float factor = t / duration;

            powerupIcon.transform.position =
                Vector3.Lerp(powerupIcon.transform.position, targetPos, factor);

            powerupIcon.transform.localScale =
                Vector3.Lerp(startScale, targetScale, factor);

            yield return null;
        }

        powerupIcon.transform.SetParent(iconOriginalParent);
        powerupIcon.transform.localScale = iconOriginalScale;
        powerupIcon.transform.localPosition = iconOriginalPosition;
        powerupIcon.gameObject.SetActive(false);

        Time.timeScale = 1f;

        powerupActive = false;
        SetButtonsInteractable(true);
    }
}