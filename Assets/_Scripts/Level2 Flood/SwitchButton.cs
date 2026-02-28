using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SwitchButton : MonoBehaviour
{
    public int switchID;
    public bool isCorrectSwitch;

    private Button button;
    private Image image;
    private Color originalColor;
    private Vector3 originalScale;

    void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();

        originalColor = image.color;
        originalScale = transform.localScale;

        button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        PlayButtonSpecificSound();
        StartCoroutine(PopEffect());
        PatternManager.Instance.OnSwitchClicked(this);
    }

    void PlayButtonSpecificSound()
    {
        ButtonSoundPlayer soundPlayer = GetComponent<ButtonSoundPlayer>();
        if (soundPlayer != null)
        {
            soundPlayer.PlayButtonSound();
        }
    }

    IEnumerator PopEffect()
    {
        transform.localScale = originalScale * 1.15f;
        yield return new WaitForSeconds(0.1f);
        transform.localScale = originalScale;
    }

    public void PulseRed()
    {
        StartCoroutine(RedPulse());
    }

    IEnumerator RedPulse()
    {
        for(int i = 0; i < 2; i++)
        {
            image.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            image.color = originalColor;
            yield return new WaitForSeconds(0.15f);
        }
    }
}