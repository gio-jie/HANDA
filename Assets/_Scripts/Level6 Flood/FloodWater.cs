using UnityEngine;

public class FloodWater : MonoBehaviour
{
    public Transform player;

    [Header("Flood Settings")]
    public float triggerDistance = 10f;
    public float riseSpeed = 2f;
    //public float coverOffset;
    public float waterHeight;

    private float waterY;
    private bool floodActive = false;
    private bool soundPlayed = false;
    private bool warningSound = false;

    public AudioClip waterSound;
    public AudioSource audioSource;

    private PlayerBounce playerBounce;

    void Start()
    {
        waterY = transform.position.y;

        if (player != null)
            playerBounce = player.GetComponent<PlayerBounce>();
    }

    void Update()
    {
        if (player == null || playerBounce == null) return;

        float dangerY = playerBounce.lastSafeY - triggerDistance;

        if (!floodActive && player.position.y < dangerY)
        {
            floodActive = true;

            if (!soundPlayed)
            {
                audioSource.PlayOneShot(waterSound);
                soundPlayed = true;
            }
        }

        if (floodActive)
            RiseWater();
    }

    void RiseWater()
    {
        Camera cam = Camera.main;
        float camBottomY = cam.transform.position.y - cam.orthographicSize;
        float waterBottomY = waterY - (waterHeight / 2f);

        waterY = transform.position.y;

        if (waterBottomY < camBottomY)
        {
            waterY += riseSpeed * Time.deltaTime;

            // Freeze player
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;

            // Hurt flash while rising
            if (!warningSound)
            {
                AudioManager.instance.PlaySFX(AudioManager.instance.warningSound);
                warningSound = true;
            }
            PlayerHurtFlash hurtFlash = player.GetComponent<PlayerHurtFlash>();
            if (hurtFlash != null)
                hurtFlash.PlayHurtEffect();

            transform.position = new Vector3(
                transform.position.x,
                waterY,
                transform.position.z
            );
        }
        else
        {
            StarManagerLevel6.Instance.TriggerLose();
        }
    }
}
