using UnityEngine;

public class MosquitoBehavior : MonoBehaviour
{
    public float lifeTime = 3.0f; // 3 seconds bago kumagat

    void Start()
    {
        // Simulan ang timer ng pagkagat pagkapanganak pa lang
        Invoke("BitePlayer", lifeTime);
    }

    void OnMouseDown()
    {
        // Kapag na-click ng player (NAPATAY):
        CancelInvoke("BitePlayer"); // Tigil ang timer ng kagat
        Debug.Log("SPLAT! Patay ang lamok!");
        Destroy(gameObject); // Mawawala ang lamok
    }

    void BitePlayer()
    {
        // Kapag naubos ang oras (NAKAGAT):
        // Hanapin ang GameManager at bawasan ang buhay
        FindObjectOfType<GameManager>().TakeDamage();

        // Mawawala ang lamok pagkatapos kumagat
        Destroy(gameObject);
    }
}