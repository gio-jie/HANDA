using UnityEngine;

public enum PlatformType { Basic, Moving, Falling, Disappearing }

public class PlatformBehaviorLevel6 : MonoBehaviour
{
    public PlatformType platformType;

    public float moveSpeed;      // moving platform
    public float fallSpeed = 5f;      // falling after collision
    public float moveDirection;  // left/right

    private bool playerLanded = false;
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (platformType == PlatformType.Moving)
        {
            transform.position += Vector3.right * moveSpeed * moveDirection * Time.deltaTime;

            float camHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
            float leftBound = Camera.main.transform.position.x - camHalfWidth;
            float rightBound = Camera.main.transform.position.x + camHalfWidth;

            if (transform.position.x < leftBound || transform.position.x > rightBound)
            {
                moveDirection *= -1f;
            }
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
