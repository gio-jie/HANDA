using UnityEngine;

public class PlayerBounce : MonoBehaviour
{
    [Header("Movement")]
    public float bounceForce = 12f;
    public float firstBounceForce = 18f;
    public float moveSpeed = 5f;
    public float tiltSensitivity = 3f;

    [Header("Power-Up Settings")]
    public float powerUpBounceMultiplier = 1.5f;
    public float powerUpDuration = 5f;
    private bool powerUpActive = false;
    private float powerUpTimer = 0f;

    private bool firstBounceDone = false;
    private bool cameraFollowStarted = false;
    public float lastSafeY;
    private bool playerLanded = false;

    private Rigidbody2D rb;

    public AudioClip jumpClip;
    public AudioClip powerUpSfx;
    public AudioSource audioSource;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float move = Input.GetAxis("Horizontal") + Input.acceleration.x * tiltSensitivity;
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        if (powerUpActive)
        {
            powerUpTimer -= Time.deltaTime;
            if (powerUpTimer <= 0f)
                powerUpActive = false;
        }
    }

    void FixedUpdate()
    {
        if(transform.position.x >=  10f)
        {
            transform.position = new Vector3(-10f, transform.position.y, transform.position.z);
        }

        if (transform.position.x < -10f)
        {
            transform.position = new Vector3(10f, transform.position.y, transform.position.z);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            if (rb.linearVelocity.y > 0f) return;

            PlatformSpawnerLevel6.Instance.EnableSpawning();

            if (!firstBounceDone)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, firstBounceForce);
                firstBounceDone = true;
            }
            else
            {
                lastSafeY = transform.position.y;
                Bounce();

                if (!cameraFollowStarted)
                {
                    Camera.main.GetComponent<CameraFollowLevel6>().StartFollowing();
                    cameraFollowStarted = true;
                }
                else
                {
                    PlatformSpawnerLevel6.Instance.SpawnPlatformAboveCamera();
                }

                playerLanded = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "PowerUp":
                audioSource.PlayOneShot(powerUpSfx);
                if (playerLanded)
                {
                    ActivatePowerUp();
                    Destroy(collision.gameObject);
                }
                else
                {
                    Debug.Log("PowerUp ignored: player not landed yet.");
                }
                break;

            case "Banana":
                AudioManager.instance.PlaySFX(AudioManager.instance.warningSound);
                StarManagerLevel6.Instance.ApplyPenalty(3f);
                GetComponent<PlayerHurtFlash>()?.PlayHurtEffect();
                Destroy(collision.gameObject);
                break;

            case "Glass":
                AudioManager.instance.PlaySFX(AudioManager.instance.warningSound);
                StarManagerLevel6.Instance.ApplyPenalty(5f);
                GetComponent<PlayerHurtFlash>()?.PlayHurtEffect();
                Destroy(collision.gameObject);
                break;

            case "Water":
                AudioManager.instance.PlaySFX(AudioManager.instance.warningSound);
                StarManagerLevel6.Instance.ApplyPenalty(5f);
                StarManagerLevel6.Instance.RegisterWaterHit();
                GetComponent<PlayerHurtFlash>()?.PlayHurtEffect();
                Destroy(collision.gameObject);
                break;
        }
    }

    void ActivatePowerUp()
    {
        StarManagerLevel6.Instance.ActivatePowerUp();

        powerUpActive = true;
        powerUpTimer = powerUpDuration;
    }

    public void StartPowerUp()
    {
        powerUpActive = true;
        powerUpTimer = powerUpDuration;
    }

    void Bounce()
    {
        if (rb.linearVelocity.y <= 0f)
        {
            audioSource.PlayOneShot(jumpClip);
            
            float finalBounce = bounceForce;

            if (powerUpActive)
                finalBounce *= powerUpBounceMultiplier;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, finalBounce);
        }
    }

    public bool IsJumping()
    {
        return rb.linearVelocity.y > 0.1f;
    }

    public bool IsHighJumpActive()
    {
        return StarManagerLevel6.Instance != null && StarManagerLevel6.Instance.IsHighJumpActive();
    }
}