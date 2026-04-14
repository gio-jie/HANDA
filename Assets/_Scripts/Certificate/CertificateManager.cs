using UnityEngine;
using System.Collections;
using TMPro;

public class CertificateManager : MonoBehaviour
{
    [Header("Certificate")]
    public Texture2D certificateTexture;           // Drag your PNG certificate here
    public string certificateFileName = "Certificate-FloodStage.png";

    [Header("Popup UI")]
    public GameObject popupPanel;
    public TMP_Text popupText;
    public float popupDuration = 2f;

    // ----------------- SAVE TO GALLERY -----------------
    public void SaveCertificate()
    {
        if (certificateTexture == null)
        {
            Debug.LogError("Certificate texture not assigned!");
            return;
        }

        NativeGallery.SaveImageToGallery(
            certificateTexture,
            "DisasterPreparednessCertificates",
            certificateFileName,
            (bool success, string path) =>
            {
                if (success)
                {
                    Debug.Log("Certificate saved to gallery at: " + path);
                    ShowPopup("Certificate saved to gallery!");
                }
                else
                {
                    Debug.LogError("Failed to save certificate!");
                    ShowPopup("Failed to save certificate!");
                }
            }
        );
    }

    // ----------------- SHARE CERTIFICATE -----------------
    public void ShareCertificate()
    {
        if (certificateTexture == null)
        {
            Debug.LogError("Certificate texture not assigned!");
            return;
        }

        // Save temporarily to cache for sharing
        string tempPath = System.IO.Path.Combine(Application.temporaryCachePath, certificateFileName);
        byte[] pngData = certificateTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes(tempPath, pngData);

        new NativeShare()
            .AddFile(tempPath)
            .SetSubject("I Completed Disaster Preparedness Training!")
            .SetText("I earned my Disaster Preparedness Certificate! 🌎🚨")
            .Share();

        ShowPopup("Certificate shared!");
    }

    // ----------------- POPUP -----------------
    private void ShowPopup(string message)
    {
        if (popupPanel == null || popupText == null)
            return;

        popupText.text = message;
        popupPanel.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(HidePopup());
    }

    private IEnumerator HidePopup()
    {
        yield return new WaitForSeconds(popupDuration);
        popupPanel.SetActive(false);
    }

    // --- BUG FIX FOR NEXT STAGE ---
    public void GoToNextStage()
    {
        Time.timeScale = 1;

        PlayerPrefs.SetInt("Flood_Unlocked", 1);
        PlayerPrefs.Save();

        UnityEngine.SceneManagement.SceneManager.LoadScene("FloodLevelSelect"); 
    }
}