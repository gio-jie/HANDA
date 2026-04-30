using UnityEngine;
using System.Collections;

public class EarthquakeIntro : MonoBehaviour
{
    public Transform cameraTransform;
    public float shakeDuration = 2f;
    public float shakeMagnitude = 0.3f;

    public AudioSource rumbleSound;

    public SpriteRenderer playerSR;
    public Sprite smileSprite;
    public Sprite shockSprite;
    public Sprite seriousSprite;

    public GameObject missionText;

    private Vector3 originalPos;

    void Start()
    {
        StartCoroutine(IntroSequence());

        UIManager.Instance.SetInstruction("EARTHQUAKE! Protect yourself immediately!");
    }

    void Update()
    {
        if (Time.timeScale == 0f)
        {
            if (rumbleSound.isPlaying)
                rumbleSound.Pause();
        }
        else
        {
            if (!rumbleSound.isPlaying)
                rumbleSound.UnPause();
        }
    }

    IEnumerator IntroSequence()
    {
        // Start smiling
        playerSR.sprite = smileSprite;

        yield return new WaitForSeconds(0.1f);

        // Shock during shake

        rumbleSound.Play();

        yield return new WaitForSeconds(0.9f);

        playerSR.sprite = shockSprite;

        originalPos = cameraTransform.localPosition;

        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            if (Time.timeScale == 0f)
            {
                yield return null;
                continue;
            }

            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            cameraTransform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        cameraTransform.localPosition = originalPos;

        // After shake → serious
        playerSR.sprite = seriousSprite;

        missionText.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        if (rumbleSound != null)
            rumbleSound.Stop();

        StarManagerEarthquake3.Instance.StartGameplay();
    }
}