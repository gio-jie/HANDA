using UnityEngine;

public class DebrisDrift : MonoBehaviour
{
    public float driftSpeed = 2f;

    void Update()
    {
        transform.position += Vector3.down * driftSpeed * Time.deltaTime;
    }
}