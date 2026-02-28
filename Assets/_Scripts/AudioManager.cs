using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Clips (Library)")]
    public AudioClip backgroundMusic; 
    public AudioClip clickSound;
    public AudioClip winSound;
    public AudioClip loseSound;
    public AudioClip warningSound;
    public AudioClip correctSound;
    public AudioClip wrongSound;  
    public AudioClip starReducedSound;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Pag start, kopyahin ang settings mula sa Options
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);

        musicSource.volume = musicVol;
        sfxSource.volume = sfxVol;

        // Play BGM agad
        PlayMusic(backgroundMusic);
    }

    // Function para magpatugtog ng Music
    public void PlayMusic(AudioClip clip)
    {
        if (clip != null)
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
    }

    // Function para magpatugtog ng SFX (Isang beses lang)
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            // Check natin kung ang pinapapatugtog ba ay yung WIN SOUND
            if (clip == winSound || clip == loseSound)
            {
                // Kung Win Sound, lakasan natin (Example: 1.0f = Normal, 3.0f = Sobrang Lakas)
                // Pwede mong baguhin yung "2.0f" kung kulang pa
                sfxSource.PlayOneShot(clip, 20.0f); 
            }
            else
            {
                // Kung ibang sound (click, warning, etc.), normal volume lang
                sfxSource.PlayOneShot(clip, 2.0f);
            }
        }
    }

    // Function na tatawagin ng Options Manager pag ginalaw ang slider
    public void UpdateVolume()
    {
        musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }
    // Function para i-pause ang Background Music
    public void PauseBGM()
    {
        if (musicSource != null)
        {
            musicSource.Pause();
        }
    }

    // Function para ituloy ang Background Music
    public void ResumeBGM()
    {
        if (musicSource != null)
        {
            musicSource.UnPause();
        }
    }
}