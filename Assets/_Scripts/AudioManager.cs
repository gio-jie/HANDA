using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // Singleton: Para matawag siya kahit saan

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Clips (Library)")]
    // Dito mo i-d-drag lahat ng sounds mo para organized
    public AudioClip backgroundMusic; 
    public AudioClip clickSound;
    public AudioClip winSound;
    public AudioClip loseSound;
    public AudioClip warningSound;
    public AudioClip correctSound;
    public AudioClip wrongSound;   
    // Pwede ka magdagdag dito: public AudioClip hammerSound; etc.

    void Awake()
    {
        // SINGLETON PATTERN: Sisiguraduhin na isa lang ang DJ sa buong laro
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // HUWAG SIRAIN PAG LIPAT NG SCENE
        }
        else
        {
            Destroy(gameObject); // May DJ na, layas ka na
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
            sfxSource.PlayOneShot(clip);
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