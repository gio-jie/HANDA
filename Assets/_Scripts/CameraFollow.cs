using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Dito mo ilalagay yung Character mo
    public float smoothSpeed = 0.125f; // Ito ang pampakinis ng galaw (0.1 to 0.5 recommended)
    
    // Offset kung gusto mong hindi gitnang-gitna (Optional, pero sa 2D usually 0,0,-10)
    public Vector3 offset = new Vector3(0, 0, -10); 

    void LateUpdate()
    {
        if (target != null)
        {
            // Kunin ang pwesto ng player
            // Pinapanatili natin ang 'z' sa -10 para hindi mawala ang camera sa 2D view
            Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, -10f);
            
            // Ito ang nagpapa-smooth. Hindi siya lilipat agad, kundi didulas papunta sa player.
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            
            // I-apply ang bagong position
            transform.position = smoothedPosition;
        }
    }
}