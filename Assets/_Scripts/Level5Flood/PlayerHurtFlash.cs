using UnityEngine;
using System.Collections;

public class PlayerHurtFlash : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color originalColor;

    public float flashDuration = 0.1f;
    public int flashCount = 2;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    public void PlayHurtEffect()
    {
        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        for (int i = 0; i < flashCount; i++)
        {
            sr.color = Color.red;
            yield return new WaitForSecondsRealtime(flashDuration);

            sr.color = originalColor;
            yield return new WaitForSecondsRealtime(flashDuration);
        }

        sr.color = originalColor;
    }
}