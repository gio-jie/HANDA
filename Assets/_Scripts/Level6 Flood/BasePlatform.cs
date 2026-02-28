using UnityEngine;

public class BasePlatform : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Floodwater"))
        {
            PlatformSpawnerLevel6.Instance.RemovePlatform(gameObject);
        }
    }
}