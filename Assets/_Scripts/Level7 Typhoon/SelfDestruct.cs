using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float lifetime = 1.5f; // Gaano katagal bago mawala (1.5 seconds)

    void Start()
    {
        // Wasakin ang sarili pagkatapos ng 'lifetime' seconds
        Destroy(gameObject, lifetime);
    }
}