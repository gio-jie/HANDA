using UnityEngine;

public class Level7Player : MonoBehaviour
{
    public float moveSpeed = 500f; 
    public VirtualJoystick joystick; 

    [Header("Level 7 Additions")]
    public GameObject warningIcon; 
    public Level7ManagerEQ manager; // Ginawa nating public ito!

    private Rigidbody2D rb;
    public bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Kung hindi mo na-drag sa inspector, susubukan pa rin niyang hanapin
        if (manager == null) manager = FindFirstObjectByType<Level7ManagerEQ>();

        if (joystick == null) joystick = FindFirstObjectByType<VirtualJoystick>();
        if (warningIcon != null) warningIcon.SetActive(false); 
    }

    void FixedUpdate()
    {
        if (isDead) 
        {
            rb.linearVelocity = Vector2.zero; 
            return;
        }

        float x = 0; float y = 0;
        
        if (joystick != null)
        {
            x = joystick.Horizontal();
            y = joystick.Vertical();
        }

        Vector2 movement = new Vector2(x, y);
        rb.linearVelocity = movement * moveSpeed * Time.fixedDeltaTime;

        if (x != 0 || y != 0)
        {
            float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
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