using UnityEngine;
using System.Collections;

public class ItemPopUp : MonoBehaviour
{
    public float popTime = 0.3f;
    public Vector3 startScale = Vector3.zero;
    public Vector3 endScale = Vector3.one;

    void Awake()
    {
        transform.localScale = Vector3.zero;
    }

    public void PopUp()
    {
        StartCoroutine(PopUpCoroutine());
    }

    private IEnumerator PopUpCoroutine()
    {
        float elapsed = 0f;
        transform.localScale = startScale;

        while (elapsed < popTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / popTime;
            t = Mathf.Sin(t * Mathf.PI * 0.5f); // ease out
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        transform.localScale = endScale;
    }
}