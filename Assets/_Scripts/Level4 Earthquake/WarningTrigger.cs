using UnityEngine;

public class WarningTrigger : MonoBehaviour
{
    public RectTransform player;
    public string warningMessage;
    public float radius = 80f;

    [Header("SFX")]
    public AudioSource audioSource;
    public AudioClip warningSFX;

    private bool isInside = false;

    void Update()
    {
        float distance = Vector2.Distance(player.position, transform.position);

        // =========================
        // ENTER
        // =========================
        if (distance <= radius && !isInside)
        {
            isInside = true;

            // 🎮 show warning UI
            WarningPopUp.Instance.ShowWarning(warningMessage);

            // 🔊 play sound
            if (audioSource != null && warningSFX != null)
            {
                audioSource.PlayOneShot(warningSFX);
            }
        }

        // =========================
        // EXIT RESET
        // =========================
        if (distance > radius && isInside)
        {
            isInside = false;
        }
    }
}