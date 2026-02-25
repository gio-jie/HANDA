using UnityEngine;

public class FlyingMosquito : MonoBehaviour
{
    [Header("Flight Settings")]
    public float wanderSpeed = 3f; 
    public float attackSpeed = 6f; 
    public float timeBeforeAttack = 4f; 
    
    private float aliveTimer = 0f;
    private bool isAttacking = false;
    private Vector2 randomTargetPosition; 
    private Transform playerTransform; 

    private SpriteRenderer spriteRenderer;

    [Header("VFX & SFX Settings")]
    public GameObject splatPrefab; 
    // --- BAGONG DAGDAG: Slot para sa Splat Sound ---
    public AudioClip splatSound; 
    // ----------------------------------------------

    private float minX = -7f, maxX = 7f, minY = -4f, maxY = 4f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        PickNewRandomTarget();
        
        GameObject jobert = GameObject.FindGameObjectWithTag("Player");
        if (jobert != null) playerTransform = jobert.transform;
    }

    void Update()
    {
        if (Level7Manager.instance != null && !Level7Manager.instance.isGameActive) return;

        aliveTimer += Time.deltaTime; 

        if (!isAttacking)
        {
            transform.position = Vector2.MoveTowards(transform.position, randomTargetPosition, wanderSpeed * Time.deltaTime);

            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = (randomTargetPosition.x > transform.position.x);
            }

            if (Vector2.Distance(transform.position, randomTargetPosition) < 0.2f)
            {
                PickNewRandomTarget();
            }

            if (aliveTimer >= timeBeforeAttack && playerTransform != null)
            {
                isAttacking = true;
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, attackSpeed * Time.deltaTime);

            if (spriteRenderer != null && playerTransform != null)
            {
                spriteRenderer.flipX = (playerTransform.position.x > transform.position.x);
            }

            if (Vector2.Distance(transform.position, playerTransform.position) < 0.5f)
            {
                Level7Manager.instance.TakeDamage(); 
                Destroy(gameObject); 
            }
        }
    }

    void PickNewRandomTarget()
    {
        randomTargetPosition = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
    }

    void OnMouseDown()
    {
        if (Level7Manager.instance != null && !Level7Manager.instance.isGameActive) return;

        // SPAWN SPLAT EFFECT (Visual)
        if (splatPrefab != null)
        {
            Instantiate(splatPrefab, transform.position, transform.rotation);
        }

        // --- BAGONG DAGDAG: PLAY SPLAT SOUND (Audio) ---
        if (splatSound != null)
        {
            // Gumagamit tayo ng PlayClipAtPoint para tumunog pa rin kahit i-Destroy na natin yung lamok!
            // Nilagay natin sa pwesto ng Camera para malakas at rinig na rinig.
            AudioSource.PlayClipAtPoint(splatSound, Camera.main.transform.position, 1f);
        }
        // -----------------------------------------------

        if (Level7Manager.instance != null) Level7Manager.instance.AddScore(); 
        Destroy(gameObject); 
    }
}