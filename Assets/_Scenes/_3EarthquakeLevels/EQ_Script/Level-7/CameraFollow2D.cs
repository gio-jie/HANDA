using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Sino ang susundan?")]
    public Transform target; // I-drag si Player_Jobert dito

    [Header("Settings")]
    public float smoothSpeed = 0.125f; // Gaano kabilis humabol (0.1 to 0.5 is good)
    public Vector3 offset = new Vector3(0, 0, -10f); // Importante ang -10 sa Z!

    void LateUpdate()
    {
        if (target != null)
        {
            // Kunin ang position ni Jobert + offset
            Vector3 desiredPosition = target.position + offset;
            
            // Swabeng paghabol (Interpolation)
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            
            // I-update ang position ng Camera
            transform.position = smoothedPosition;
        }
    }
}