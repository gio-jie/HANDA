using UnityEngine;

public class ArrowPlace : MonoBehaviour
{
    public float speed;
    public float height;

    private RectTransform rectTransform;
    private Vector2 startPos;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        startPos = rectTransform.anchoredPosition;
    }

    void Update()
    {
        float newY = Mathf.Sin(Time.time * speed) * height;
        rectTransform.anchoredPosition = startPos + new Vector2(0, newY);
    }
}