using UnityEngine;
using System.Collections;

public class HighBounce : MonoBehaviour
{
    public float boostAmount = 6f;
    public float duration = 5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerBounce pb = other.GetComponent<PlayerBounce>();
        if (pb != null)
        {
            StartCoroutine(Boost(pb));
        }

        Destroy(gameObject);
    }

    IEnumerator Boost(PlayerBounce pb)
    {
        pb.bounceForce += boostAmount;
        yield return new WaitForSeconds(duration);
        pb.bounceForce -= boostAmount;
    }
}