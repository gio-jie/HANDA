using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems; 
using System.Collections;
using System.IO; 

public class CertificateUIManager : MonoBehaviour
{
    [Header("Win Panel Elements")]
    public GameObject winPanelMainContent; 
    public GameObject smallCertButtonObj;  

    [Header("Large Certificate Panel")]
    public GameObject largeCertPanelParent; 
    public Animator largeCertAnimator;      
    public TMP_InputField nameInputField;   
    public TMP_Text finalNameText;          
    public GameObject confirmButtonObj;     

    [Header("Final Buttons Panel")]
    public GameObject finalButtonsPanel;    

    public void ShowWinWithSmallCert()
    {
        winPanelMainContent.SetActive(true);
        smallCertButtonObj.SetActive(true);

        largeCertPanelParent.SetActive(false);
        finalButtonsPanel.SetActive(false);
    }

    public void OnSmallCertClicked()
    {
        winPanelMainContent.SetActive(false);
        smallCertButtonObj.SetActive(false);

        largeCertPanelParent.SetActive(true);
        
        nameInputField.text = "";
        nameInputField.gameObject.SetActive(true);
        confirmButtonObj.SetActive(true);
        finalButtonsPanel.SetActive(false);

        // --- ANG FIX: Tatawag tayo ng Coroutine imbes na direktang i-trigger ---
        StartCoroutine(TriggerZoomDelay());
    }

    // --- BAGONG DAGDAG: Ang 1-Frame Delay para sa Animator ---
    IEnumerator TriggerZoomDelay()
    {
        // Maghintay ng isang frame (split-second) para magising nang buo ang Animator
        yield return null; 

        if (largeCertAnimator)
        {
            largeCertAnimator.SetTrigger("ZoomIn");
            Debug.Log("ZoomIn Trigger Fired!");
        }
    }

    public void OnNameConfirmed()
    {
        if (string.IsNullOrEmpty(nameInputField.text)) return;

        if (finalNameText)
        {
            finalNameText.text = nameInputField.text;
            finalNameText.gameObject.SetActive(true);
        }

        nameInputField.gameObject.SetActive(false);
        confirmButtonObj.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);

        finalButtonsPanel.SetActive(true);
    }

    public void ScenarioSelectionAction()
    {
        SceneManager.LoadScene("TyphoonLevelSelect"); 
    }
    
    public void SaveToGalleryAction()
    {
        StartCoroutine(TakeScreenshotInternal());
    }

    public void ShareAction()
    {
        Debug.Log("Share button clicked! (Needs Native Share Plugin for actual mobile sharing)");
    }

    IEnumerator TakeScreenshotInternal()
    {
        yield return new WaitForEndOfFrame();

        string filename = "HandaCert_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".png";
        ScreenCapture.CaptureScreenshot(filename);

        Debug.Log("Screenshot taken and saved internally to: " + Path.Combine(Application.persistentDataPath, filename));
    }
}