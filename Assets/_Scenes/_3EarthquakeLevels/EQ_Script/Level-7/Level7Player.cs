using UnityEngine;

public class Level7Player : MonoBehaviour
{
    public float moveSpeed = 500f; 
    public VirtualJoystick joystick; 
    public GameObject warningIcon; 
    public Level7ManagerEQ manager;

    private Rigidbody2D rb;
    private Animator anim; 
    private SpriteRenderer spriteRenderer; 
    public bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); 
        spriteRenderer = GetComponent<SpriteRenderer>(); 

        if (manager == null) manager = FindFirstObjectByType<Level7ManagerEQ>();
        if (joystick == null) joystick = FindFirstObjectByType<VirtualJoystick>();
        if (warningIcon != null) warningIcon.SetActive(false); 
    }

    void FixedUpdate()
    {
        // ========================================================
        // BAGONG DAGDAG: Bawal gumalaw kung hindi pa active ang game (Intro playing)
        // ========================================================
        if (isDead || (manager != null && !manager.isGameActive)) 
        { 
            rb.linearVelocity = Vector2.zero; 
            if (anim != null) anim.SetBool("IsMoving", false);
            return; 
        }

        float x = (joystick != null) ? joystick.Horizontal() : 0;
        float y = (joystick != null) ? joystick.Vertical() : 0;

        Vector2 movement = new Vector2(x, y);
        rb.linearVelocity = movement * moveSpeed * Time.fixedDeltaTime;

        if (movement != Vector2.zero)
        {
            anim.SetBool("IsMoving", true);
            anim.SetFloat("MoveX", x);
            anim.SetFloat("MoveY", y);

            if (x < 0) spriteRenderer.flipX = true;
            else if (x > 0) spriteRenderer.flipX = false;
        }
        else
        {
            anim.SetBool("IsMoving", false);
        }
    }

    // ========================================================
    // --- PROXIMITY DETECTION PARA SA KALAT, SUSI, AT EXTINGUISHER ---
    // ========================================================
    void OnTriggerEnter2D(Collider2D other)
    {
        // Kapag dumikit sa kalat, sirang pinto, o apoy
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
        // Kapag dinaanan yung Fire Extinguisher
        else if (other.gameObject.CompareTag("ExtinguisherPickup"))
        {
            Destroy(other.gameObject); // Mawawala sa map yung extinguisher
            if (manager != null) manager.PickUpExtinguisher(); // Ipapasa sa UI panel yung extinguisher
        }
        else if (other.gameObject.CompareTag("Finish"))
        {
            Debug.Log("NAKATAPAK SA WIN ZONE!"); // Maglalagay tayo nito para sure
            if (manager != null) manager.LevelComplete(); 
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