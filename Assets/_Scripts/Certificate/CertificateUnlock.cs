using UnityEngine;

public class CertificateUnlock : MonoBehaviour
{
    [Header("Unlock Settings")]
    public string unlockKey = "Earthquake_Unlocked";

    void Start()
    {
        UnlockNextStage();
    }

    void UnlockNextStage()
    {
        PlayerPrefs.SetInt(unlockKey, 1);
        PlayerPrefs.Save();

        Debug.Log(unlockKey + " unlocked!");
    }
}