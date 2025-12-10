using UnityEngine;

public class MazePlayer : MonoBehaviour
{
    public float moveSpeed = 500f; 
    public VirtualJoystick joystick; 

    private Rigidbody2D rb;
    private Vector3 startPos;
    private Level5Manager manager;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
        manager = FindFirstObjectByType<Level5Manager>();

        // Auto-find joystick kung nakalimutan i-drag
        if (joystick == null) joystick = FindFirstObjectByType<VirtualJoystick>();
    }

    void FixedUpdate()
    {
        // Kung game over na, wag na gumalaw
        if (isDead) 
        {
            rb.linearVelocity = Vector2.zero; 
            return;
        }

        // Kunin ang input (Joystick or Keyboard)
        float x = 0;
        float y = 0;
        
        if (joystick != null)
        {
            x = joystick.Horizontal();
            y = joystick.Vertical();
        }

        // Apply movement physics
        Vector2 movement = new Vector2(x, y);
        rb.linearVelocity = movement * moveSpeed * Time.fixedDeltaTime;

        // --- ITO ANG BAGO: ROTATION LOGIC ---
        // Iikutin natin si Jobert kung may movement input
        if (x != 0 || y != 0)
        {
            // Kinakalkula ang angle (sa degrees) base sa direction ng joystick
            float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

            // Ina-apply ang rotation
            // NOTE: Yung "- 90" ay kung ang sprite ni Jobert ay nakaharap sa TAAS.
            // Kung nakaharap siya sa KANAN, tanggalin mo yung "- 90".
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
        // ------------------------------------
    }

    // Collision Detection
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Hazard")) // Baha o Landslide
        {
            Debug.Log("Hit Hazard!");
            if(manager != null) manager.HitHazard(); 
            transform.position = startPos; // Reset position
            rb.linearVelocity = Vector2.zero; // Stop momentum
        }
        else if (other.gameObject.CompareTag("Goal")) // Evac Center
        {
            Debug.Log("Goal!");
            isDead = true;
            rb.linearVelocity = Vector2.zero;
            if(manager != null) manager.ReachGoal();
        }
    }
}