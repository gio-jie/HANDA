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
    }

    // Collision Detection
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Hazard")) // Baha o Landslide
        {
            Debug.Log("Hit Hazard!");
            manager.HitHazard(); 
            transform.position = startPos; // Reset position
            rb.linearVelocity = Vector2.zero; // Stop momentum
        }
        else if (other.gameObject.CompareTag("Goal")) // Evac Center
        {
            Debug.Log("Goal!");
            isDead = true;
            rb.linearVelocity = Vector2.zero;
            manager.ReachGoal();
        }
    }
}