using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MapPanel : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panel;
    public Button openButton;
    public Button closeButton;

    [Header("Location Icon")]
    public RectTransform locationIcon;
    public float bounceHeight = 20f;
    public float bounceSpeed = 2.5f;

    [Header("Animation")]
    public float popDuration = 0.2f;
    public float startScale = 0.85f;

    [Header("Timer Control (optional)")]
    public bool pauseTimer = true;

    private bool isShowing = false;
    private Coroutine currentAnim;

    private Vector3 iconStartPos;

    void Awake()
    {
        if (panel != null)
        {
            panel.transform.localScale = Vector3.one * startScale;
            panel.SetActive(false);
        }

        if (locationIcon != null)
        {
            iconStartPos = locationIcon.localPosition;
        }

        if (openButton != null)
            openButton.onClick.AddListener(ShowPanel);

        if (closeButton != null)
            closeButton.onClick.AddListener(HidePanel);
    }

    void Update()
    {
        if (!isShowing || locationIcon == null) return;

        float bounce = Mathf.Abs(Mathf.Sin(Time.unscaledTime * bounceSpeed));
        float yOffset = bounce * bounceHeight;

        locationIcon.localPosition = iconStartPos + new Vector3(0, yOffset, 0);
    }

    public void ShowPanel()
    {
        if (isShowing || panel == null) return;

        isShowing = true;
        panel.SetActive(true);
        if (AudioManager.instance != null) AudioManager.instance.PauseBGM();

        if (pauseTimer)
            Time.timeScale = 0f;

        if (currentAnim != null)
            StopCoroutine(currentAnim);

        currentAnim = StartCoroutine(Animate(true));
    }

    public void HidePanel()
    {
        if (!isShowing || panel == null) return;

        isShowing = false;

        if (pauseTimer)
            Time.timeScale = 1f;

        if (currentAnim != null)
            StopCoroutine(currentAnim);

        currentAnim = StartCoroutine(Animate(false));

        if (locationIcon != null)
        {
            locationIcon.localPosition = iconStartPos;
        }

        if (AudioManager.instance != null) AudioManager.instance.ResumeBGM();
    }

    IEnumerator Animate(bool opening)
    {
        float timer = 0f;

        Vector3 start = opening ? Vector3.one * startScale : Vector3.one;
        Vector3 end = opening ? Vector3.one : Vector3.one * startScale;

        while (timer < popDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / popDuration;

            t = 1f - Mathf.Pow(1f - t, 3f);

            panel.transform.localScale = Vector3.Lerp(start, end, t);

            yield return null;
        }

        panel.transform.localScale = end;

        if (!opening)
            panel.SetActive(false);
    }
}