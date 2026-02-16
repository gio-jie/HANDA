using UnityEngine;

public class FlyingMosquito : MonoBehaviour
{
    [Header("Flight Settings")]
    public float wanderSpeed = 3f; 
    public float attackSpeed = 6f; 
    public float timeBeforeAttack = 4f; // Oras bago sumugod
    
    private float aliveTimer = 0f;
    private bool isAttacking = false;
    private Vector2 randomTargetPosition; 
    private Transform playerTransform; 

    private float minX = -7f, maxX = 7f, minY = -4f, maxY = 4f;

    void Start()
    {
        PickNewRandomTarget();
        
        // Hanapin si Jobert gamit ang Tag
        GameObject jobert = GameObject.FindGameObjectWithTag("Player");
        if (jobert != null) playerTransform = jobert.transform;
    }

    void Update()
    {
        if (Level7Manager.instance != null && !Level7Manager.instance.isGameActive) return;

        aliveTimer += Time.deltaTime; 

        // WANDERING MODE (Lipad-lipad muna)
        if (!isAttacking)
        {
            transform.position = Vector2.MoveTowards(transform.position, randomTargetPosition, wanderSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, randomTargetPosition) < 0.2f)
            {
                PickNewRandomTarget();
            }

            // Kapag naubos ang oras, SUSUGOD NA!
            if (aliveTimer >= timeBeforeAttack && playerTransform != null)
            {
                isAttacking = true;
            }
        }
        // ATTACK MODE (Sugod kay Jobert)
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, attackSpeed * Time.deltaTime);

            // Kapag tumama kay Jobert
            if (Vector2.Distance(transform.position, playerTransform.position) < 0.5f)
            {
                Level7Manager.instance.TakeDamage(); // Bawas buhay
                Destroy(gameObject); // Wasakin ang lamok
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

        Debug.Log("SPLAT!");
        if (Level7Manager.instance != null) Level7Manager.instance.AddScore(); 
        Destroy(gameObject); 
    }
}