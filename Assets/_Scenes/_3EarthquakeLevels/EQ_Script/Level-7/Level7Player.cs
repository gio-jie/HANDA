using UnityEngine;

public class Level7Player : MonoBehaviour
{
    public float moveSpeed = 500f; 
    public VirtualJoystick joystick; 
    public GameObject warningIcon; 
    public Level7ManagerEQ manager;

    private Rigidbody2D rb;
    private Animator anim; // BAGONG DAGDAG
    private SpriteRenderer spriteRenderer; // PARA SA MIRRORING
    public bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); // Kuhanin ang Animator
        spriteRenderer = GetComponent<SpriteRenderer>(); // Kuhanin ang SpriteRenderer

        if (manager == null) manager = FindFirstObjectByType<Level7ManagerEQ>();
        if (joystick == null) joystick = FindFirstObjectByType<VirtualJoystick>();
        if (warningIcon != null) warningIcon.SetActive(false); 
    }

    void FixedUpdate()
    {
        if (isDead) { rb.linearVelocity = Vector2.zero; return; }

        float x = (joystick != null) ? joystick.Horizontal() : 0;
        float y = (joystick != null) ? joystick.Vertical() : 0;

        Vector2 movement = new Vector2(x, y);
        rb.linearVelocity = movement * moveSpeed * Time.fixedDeltaTime;

        // ========================================================
        // --- ANIMATION LOGIC ---
        // ========================================================
        if (movement != Vector2.zero)
        {
            anim.SetBool("IsMoving", true);
            anim.SetFloat("MoveX", x);
            anim.SetFloat("MoveY", y);

            // MIRRORING LOGIC: I-flip ang sprite kapag papuntang kaliwa (x < 0)
            if (x < 0) spriteRenderer.flipX = true;
            else if (x > 0) spriteRenderer.flipX = false;
        }
        else
        {
            anim.SetBool("IsMoving", false);
        }
    }

    // ========================================================
    // --- BAGO: PROXIMITY DETECTION PARA SA KALAT AT SUSI ---
    // ========================================================
    void OnTriggerEnter2D(Collider2D other)
    {
        // Kapag dumikit sa kalat o sirang pinto
        if (other.gameObject.CompareTag("Obstacle")) 
        {
            if (warningIcon != null) warningIcon.SetActive(true); 
            if (manager != null) manager.SetCurrentObstacle(other.gameObject); 
        }
        // Kapag dinaanan yung susi
        else if (other.gameObject.CompareTag("Key"))
        {
            Destroy(other.gameObject); // Mawawala sa map yung susi
            if (manager != null) manager.PickUpKey(); // Ipapasa sa UI panel yung susi
        }
    }

    // Kapag lumayo si Jobert sa kalat
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Obstacle")) 
        {
            if (warningIcon != null) warningIcon.SetActive(false); // Mamamatay ang warning icon
            if (manager != null) manager.ClearCurrentObstacle(); // Hindi na gagana yung tools
        }
    }
}