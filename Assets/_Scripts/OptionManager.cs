using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject optionsPanel;
    
    public Slider musicSlider;
    public TextMeshProUGUI musicPercentText;
    
    public Slider sfxSlider;
    public TextMeshProUGUI sfxPercentText;

    public Button vibrateButton;
    public TextMeshProUGUI vibrateText;

    [Header("Colors")]
    public Color onColor = Color.green;
    public Color offColor = Color.gray;

    // Data Variables
    private bool isVibrationOn = true;

    void Start()
    {
        // 1. LOAD SAVED DATA (Kung wala pa, gamitin ang default)
        // Default Volume is 1.0 (100%), Default Vibrate is 1 (True)
        
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        int savedVibrate = PlayerPrefs.GetInt("VibrationOn", 1); // 1 = True, 0 = False

        // 2. APPLY TO UI
        musicSlider.value = savedMusic;
        sfxSlider.value = savedSFX;
        isVibrationOn = (savedVibrate == 1);

        // 3. Update Visuals
        UpdateMusicText(savedMusic);
        UpdateSFXText(savedSFX);
        UpdateVibrateUI();

        // 4. Setup Listeners (Para gumana pag ginalaw ang slider)
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    // --- MUSIC LOGIC ---
    public void SetMusicVolume(float value)
    {
        // Save sa Memory
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save(); // Force save

        if (AudioManager.instance != null)
        {
            AudioManager.instance.UpdateVolume();
        }
        
        // Update Text
        UpdateMusicText(value);

        // TODO: Dito natin ilalagay ang actual volume change code pag may sounds na
        // AudioListener.volume = value; (Temporary global volume)
    }

    void UpdateMusicText(float value)
    {
        // Convert 0.5 to 50%
        int percent = Mathf.RoundToInt(value * 100);
        musicPercentText.text = percent + "%";
    }

    // --- SFX LOGIC ---
    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
        
        UpdateSFXText(value);

        if (AudioManager.instance != null)
        {
            AudioManager.instance.UpdateVolume();
        }
    }

    void UpdateSFXText(float value)
    {
        int percent = Mathf.RoundToInt(value * 100);
        sfxPercentText.text = percent + "%";
    }

    // --- VIBRATE LOGIC ---
    public void ToggleVibration()
    {
        isVibrationOn = !isVibrationOn; // Baliktarin
        
        // Save as Int (Kasi walang Bool sa PlayerPrefs)
        PlayerPrefs.SetInt("VibrationOn", isVibrationOn ? 1 : 0);
        PlayerPrefs.Save();

        UpdateVibrateUI();

        if (isVibrationOn)
        {
             Handheld.Vibrate(); 
        }

        // Sample Vibrate (Para malaman ng player kung gumagana)
        if (isVibrationOn)
        {
             // Handheld.Vibrate(); // (Gagana lang ito sa totoong phone)
             Debug.Log("Bzzzt! Vibrate Check");
        }
    }

    void UpdateVibrateUI()
    {
        if (isVibrationOn)
        {
            vibrateButton.image.color = onColor;
            vibrateText.text = "VIBRATION: ON";
        }
        else
        {
            vibrateButton.image.color = offColor;
            vibrateText.text = "VIBRATION: OFF";
        }
    }

    // --- OPEN/CLOSE PANEL ---
    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
    }
}