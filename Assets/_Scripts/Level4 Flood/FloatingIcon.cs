using UnityEngine;

public class FloatingIcon : MonoBehaviour
{
    public float floatHeight = 15f;
    public float floatSpeed = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.localPosition = startPos + new Vector3(0, offset, 0);
    }
}