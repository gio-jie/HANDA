using UnityEngine;
public class PowerUpFloat : MonoBehaviour
{
    public float floatSpeed = 2f;
    public float floatAmount = 0.5f;

    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmount;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}