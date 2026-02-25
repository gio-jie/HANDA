using UnityEngine;
using System.IO;
using System.Collections;
using TMPro;

public class CertificateManager : MonoBehaviour
{
    public Texture2D certificateTexture;
    public string certificateFileName = "Certificate-FloodStage.png";

    [Header("Popup UI")]
    public GameObject popupPanel;
    public TMP_Text popupText;
    public float popupDuration = 2f;

    private string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, certificateFileName);
    }

    public void SaveCertificate()
    {
        if (certificateTexture == null)
        {
            Debug.LogError("Certificate texture not assigned!");
            return;
        }

        byte[] pngData = certificateTexture.EncodeToPNG();
        string filePath = GetFilePath();
        File.WriteAllBytes(filePath, pngData);

        ShowPopup("Certificate saved!");
    }

    public void ShareCertificate()
    {
        if (certificateTexture == null)
        {
            Debug.LogError("Certificate texture not assigned!");
            return;
        }

        string tempPath = Path.Combine(Application.temporaryCachePath, certificateFileName);
        byte[] pngData = certificateTexture.EncodeToPNG();
        File.WriteAllBytes(tempPath, pngData);

        new NativeShare()
            .AddFile(tempPath)
            .SetSubject("I Completed Disaster Preparedness Training!")
            .SetText("I earned my Disaster Preparedness Certificate! 🌎🚨")
            .Share();

        ShowPopup("Certificate shared!");
    }

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
}