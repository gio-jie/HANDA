using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    public void Click()
    {
        // Tawagin ang DJ para patunugin ang Click Sound
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.clickSound);
        }
    }
}