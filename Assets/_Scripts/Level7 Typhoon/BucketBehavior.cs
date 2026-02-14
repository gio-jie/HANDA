using UnityEngine;

public class BucketBehavior : MonoBehaviour
{
    public float breedingTime = 5.0f; // 5 seconds bago maging danger

    void Start()
    {
        // Simulan ang timer ng breeding
        Invoke("BreedMosquito", breedingTime);
    }

    void OnMouseDown()
    {
        // Kapag na-click (Tinakpan/Nilinis):
        CancelInvoke("BreedMosquito"); // Tigil ang timer
        
        // Visual Feedback: Gawing GREEN para alam na safe na
        GetComponent<SpriteRenderer>().color = Color.green;
        
        Debug.Log("LIGTAS! Tinakpan ang timba.");
        
        // Wasakin ang object pagkalipas ng 0.5 seconds (para makita muna yung green)
        Destroy(gameObject, 0.5f);
    }

    void BreedMosquito()
    {
        // Kapag naubos ang oras (HINDI NALINIS):
        FindObjectOfType<GameManager>().TakeDamage();
        
        Debug.Log("DANGER! Namahayan ng lamok ang tubig!");
        
        // Gawing RED para alam na danger
        GetComponent<SpriteRenderer>().color = Color.red;

        // Mawawala pagkatapos makasakit
        Destroy(gameObject, 0.5f);
    }
}