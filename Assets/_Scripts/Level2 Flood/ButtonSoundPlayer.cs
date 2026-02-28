using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ButtonSoundPlayer : MonoBehaviour
{
    public AudioClip clickSFX;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void PlayButtonSound()
    {
        if (clickSFX != null)
            audioSource.PlayOneShot(clickSFX, 10f);
    }
}