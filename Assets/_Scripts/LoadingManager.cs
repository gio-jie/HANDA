using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections; // Kailangan ito para sa Coroutine

public class LoadingManager : MonoBehaviour
{
    [Header("UI References")]
    public Slider loadingSlider;
    public TMP_Text progressText;

    [Header("Settings")]
    public float loadingDuration = 3.0f; // Tatagal ng 3 seconds ang loading
    public string sceneToLoad = "ScenarioSelection"; // Ang pupuntahan pagkatapos

    void Start()
    {
        // Simulan ang loading pagkabukas ng scene
        StartCoroutine(LoadSceneRoutine());
    }

    IEnumerator LoadSceneRoutine()
    {
        float timer = 0f;

        while (timer < loadingDuration)
        {
            timer += Time.deltaTime; // Magbilang base sa oras
            
            // Compute percentage (0.0 to 1.0)
            float progress = Mathf.Clamp01(timer / loadingDuration);
            
            // Update UI
            loadingSlider.value = progress;
            
            // Update Text (Gawing whole number, e.g., 45%)
            progressText.text = Mathf.RoundToInt(progress * 100) + "%";

            yield return null; // Maghintay ng next frame
        }

        // Pag tapos na ang oras (100%), lumipat na ng scene
        loadingSlider.value = 1;
        progressText.text = "100%";
        
        // Load the actual scene
        SceneManager.LoadScene(sceneToLoad);
    }
}