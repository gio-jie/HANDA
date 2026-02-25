using UnityEngine;

public class MosquitoBuzz : MonoBehaviour
{
    [Header("Animation Settings")]
    public float flapSpeed = 50f;      // Gaano kabilis pumagaspas? (Mas mataas, mas mabilis)
    public float flapAmount = 0.15f;   // Gaano kanipis/kakapal ang stretch?
    
    public float buzzSpeed = 40f;      // Bilis ng panginginig
    public float buzzAngle = 5f;       // Gaano kalakas ang tagilid ng panginginig?

    private Vector3 originalScale;
    private Vector3 originalRotation;

    void Start()
    {
        // Tandaan ang orihinal na size at rotation ng lamok
        originalScale = transform.localScale;
        originalRotation = transform.eulerAngles;
    }

    void Update()
    {
        // 1. WING FLAP ILLUSION (Nipis-Kapal gamit ang Scale Y)
        // Gamit ang Mathf.Sin, pinapataas at pinapababa natin ang size nang mabilis!
        float scaleY = originalScale.y + (Mathf.Sin(Time.time * flapSpeed) * flapAmount);
        transform.localScale = new Vector3(originalScale.x, scaleY, originalScale.z);

        // 2. BUZZING VIBRATION ILLUSION (Mabilisang tagilid kaliwa't kanan)
        float rotZ = originalRotation.z + (Mathf.Sin(Time.time * buzzSpeed) * buzzAngle);
        transform.rotation = Quaternion.Euler(originalRotation.x, originalRotation.y, rotZ);
    }
}