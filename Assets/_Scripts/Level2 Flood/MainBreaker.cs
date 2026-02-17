using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainBreaker : MonoBehaviour
{
    public Slider breakerSlider;

    public Image breakerImage;
    public Sprite breakerOnSprite;
    public Sprite breakerOffSprite;

    private bool isTriggered = false;

    public AudioClip clickSFX;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (breakerSlider.value <= 0.05f && !isTriggered)
        {
            isTriggered = true;
            if (clickSFX != null)
                audioSource.PlayOneShot(clickSFX, 15f);

            StartCoroutine(BreakerOffSequence());
        }
    }

    IEnumerator BreakerOffSequence()
    {
        breakerImage.sprite = breakerOffSprite;

        yield return new WaitForSeconds(0.5f);

        StarManagerLevel2.Instance.EndLevel(true);
        gameObject.SetActive(false);
    }
}