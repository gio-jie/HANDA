using UnityEngine;

public class BridgePlayer : MonoBehaviour
{
    public Transform laneUp;
    public Transform laneDown;
    public float switchSpeed = 5f;

    private Transform targetLane;

    public AudioClip bananaSound;
    public AudioClip glassSound;
    public AudioClip waterSound;

    public AudioSource audioSource;

    void Start()
    {
        targetLane = laneUp;
        transform.position = laneUp.position;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            targetLane.position,
            switchSpeed * Time.deltaTime
        );
    }

    public void MoveUp()
    {
        targetLane = laneUp;
    }

    public void MoveDown()
    {
        targetLane = laneDown;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Banana"))
        {
            audioSource.PlayOneShot(bananaSound);
            AudioManager.instance.PlaySFX(AudioManager.instance.warningSound);
            StarManagerLevel5.Instance.ApplyPenalty(2f);
            GetComponent<PlayerHurtFlash>().PlayHurtEffect();
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Glass"))
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.warningSound);
            audioSource.PlayOneShot(glassSound);
            StarManagerLevel5.Instance.ApplyPenalty(5f);
            GetComponent<PlayerHurtFlash>().PlayHurtEffect();
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Water"))
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.warningSound);
            audioSource.PlayOneShot(waterSound);
            StarManagerLevel5.Instance.ApplyPenalty(5f);
            GetComponent<PlayerHurtFlash>().PlayHurtEffect();
            StarManagerLevel5.Instance.RegisterWaterHit();
            Destroy(collision.gameObject);
        }
    }
}