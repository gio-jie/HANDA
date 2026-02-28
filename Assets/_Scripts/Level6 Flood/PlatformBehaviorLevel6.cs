using UnityEngine;

public enum PlatformType { Basic, Moving, Falling, Disappearing }

public class PlatformBehaviorLevel6 : MonoBehaviour
{
    public PlatformType platformType;

    public float moveSpeed;      // moving platform
    public float fallSpeed = 5f;      // falling after collision
    public float moveDirection;  // left/right
    private float startX;
    public float moveRange = 2f;

    private bool playerLanded = false;
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startX = transform.position.x;
    }

    void Update()
    {
        if (platformType == PlatformType.Moving)
        {
            float leftBound = startX - moveRange;
            float rightBound = startX + moveRange;

            float newX = transform.position.x + moveSpeed * moveDirection * Time.deltaTime;
            newX = Mathf.Clamp(newX, leftBound, rightBound);

            if (newX <= leftBound || newX >= rightBound)
                moveDirection *= -1f;

            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }

        if (platformType == PlatformType.Falling && playerLanded)
        {
            rb.linearVelocity = new Vector2(0f, -fallSpeed);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerLanded = true;

            if (platformType == PlatformType.Disappearing)
            {
                PlatformSpawnerLevel6.Instance.RemovePlatform(gameObject);
            }

            if (platformType == PlatformType.Falling)
            {
                // start falling
                playerLanded = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Floodwater"))
        {
            PlatformSpawnerLevel6.Instance.RemovePlatform(gameObject);
        }
    }
}
