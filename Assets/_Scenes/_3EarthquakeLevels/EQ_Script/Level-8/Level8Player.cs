using UnityEngine;

public class Level8Player : MonoBehaviour
{
    public float moveSpeed = 500f; 
    public VirtualJoystick joystick; 
    public GameObject warningIcon; 
    public Level8ManagerEQ manager;

    private Rigidbody2D rb;
    private Animator anim; 
    private SpriteRenderer spriteRenderer; 
    public bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); 
        spriteRenderer = GetComponent<SpriteRenderer>(); 

        if (manager == null) manager = FindFirstObjectByType<Level8ManagerEQ>();
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
        // 1. Kapag lumapit sa Obstacle (Kasama na dito ang NPC)
        if (other.gameObject.CompareTag("Obstacle"))
        {
            if (manager != null) manager.SetCurrentObstacle(other.gameObject);
            
            // ITO YUNG LINYA NA NAWALA KANINA:
            if (warningIcon != null) warningIcon.SetActive(true); 
        }
        // 2. Kapag dumampot ng Susi
        else if (other.gameObject.CompareTag("Key"))
        {
            Destroy(other.gameObject);
            if (manager != null) manager.PickUpKey();
        }
        // 3. Kapag dumampot ng Extinguisher
        else if (other.gameObject.CompareTag("Extinguisher"))
        {
            Destroy(other.gameObject);
            if (manager != null) manager.PickUpExtinguisher();
        }
        // 4. Kapag dumampot ng First Aid Kit sa kalsada
        else if (other.gameObject.CompareTag("FirstAidPickup"))
        {
            Destroy(other.gameObject);
            if (manager != null) manager.PickUpFirstAid();
        }
        // 5. Kapag nakarating sa Evacuation Center (Win Zone)
        else if (other.gameObject.CompareTag("Finish"))
        {
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