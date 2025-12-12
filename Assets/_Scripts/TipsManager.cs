using UnityEngine;

public class TipsManager : MonoBehaviour
{
    public GameObject tipsPanel;

    public void OpenTips()
    {
        tipsPanel.SetActive(true);
        // Sound Effect (Optional)
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.clickSound);
    }

    public void CloseTips()
    {
        tipsPanel.SetActive(false);
        // Sound Effect (Optional)
        if (AudioManager.instance != null) AudioManager.instance.PlaySFX(AudioManager.instance.clickSound);
    }
}