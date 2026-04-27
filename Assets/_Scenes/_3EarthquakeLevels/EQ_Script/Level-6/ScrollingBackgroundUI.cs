using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class ScrollingBackgroundUI : MonoBehaviour
{
    [Header("Scroll Settings")]
    public float scrollSpeed = 0.5f; // Bilis ng pag-galaw ng kalsada
    private RawImage bgImage;

    void Awake()
    {
        bgImage = GetComponent<RawImage>();
    }

    void Update()
    {
        // Wag pagalawin ang kalsada kung naka-pause o wala pa sa Phase 2
        if (Level6ManagerEQ.instance != null && !Level6ManagerEQ.instance.isGameActive) return;

        // I-scroll ang image pababa (Negative Y) para magmukhang tumatakbo pataas si Jobert
        Rect currentUV = bgImage.uvRect;
        currentUV.y += scrollSpeed * Time.deltaTime;
        bgImage.uvRect = currentUV;
    }
}