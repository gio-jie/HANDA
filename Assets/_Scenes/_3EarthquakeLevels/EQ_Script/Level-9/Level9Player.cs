using UnityEngine;

public class Level9Player : MonoBehaviour
{
    public float moveSpeed = 500f; 
    public VirtualJoystick joystick; 
    public GameObject warningIcon; 
    public Level9ManagerEQ manager; // NAKA-LINK NA SA LEVEL 9 MANAGER

    private Rigidbody2D rb;
    private Animator anim; 
    private SpriteRenderer spriteRenderer; 
    public bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); 
        spriteRenderer = GetComponent<SpriteRenderer>(); 

        if (manager == null) manager = FindFirstObjectByType<Level9ManagerEQ>(); // LEVEL 9 NA ITO
        if (joystick == null) joystick = FindFirstObjectByType<VirtualJoystick>();
        if (warningIcon != null) warningIcon.SetActive(false); 
    }

    void FixedUpdate()
    {
        // Bawal gumalaw kung hindi pa active ang game (o kapag may Lindol QTE)
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
    // --- PROXIMITY DETECTION ---
    // ========================================================
    void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Kapag lumapit sa Obstacle (Mga sasakyan na nakaharang)
        if (other.gameObject.CompareTag("Obstacle"))
        {
            if (manager != null) manager.SetCurrentObstacle(other.gameObject);
            if (warningIcon != null) warningIcon.SetActive(true); 
        }
        // 2. Kapag dumampot ng Emergency Vehicle Tool sa kalsada
        else if (other.gameObject.CompareTag("VehiclePickup")) // GUMAWA KA NG TAG NA "VehiclePickup" sa Unity
        {
            Destroy(other.gameObject);
            if (manager != null) manager.PickUpVehicle();
        }
        // 3. Kapag nakarating sa Finish Line
        else if (other.gameObject.CompareTag("Finish"))
        {
            if (manager != null) manager.LevelComplete();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Obstacle")) 
        {
            if (warningIcon != null) warningIcon.SetActive(false); 
            if (manager != null) manager.ClearCurrentObstacle(); 
        }
    }
}