using UnityEngine;

public class TapBag : MonoBehaviour
{
    [Header("Bag Sprites")]
    public Sprite closedBagSprite;
    public Sprite openBagSprite;

    [Header("Item sa Loob (Ang lalabas pag na-tap)")]
    public GameObject kulamboItem; 

    private SpriteRenderer bagRenderer;
    private bool isOpen = false;

    void Start()
    {
        // Pinalitan natin ng SpriteRenderer!
        bagRenderer = GetComponent<SpriteRenderer>();
        
        if (bagRenderer != null && closedBagSprite != null)
        {
            bagRenderer.sprite = closedBagSprite;
        }

        // Itago muna ang Kulambo
        if (kulamboItem != null) 
        {
            kulamboItem.SetActive(false);
        }
    }

    // --- DITO ANG MAGIC PARA SA 2D OBJECTS ---
    // Awtomatikong ma-dedetect ng Unity kapag kinlick/tinap mo ang BoxCollider2D niya!
    void OnMouseDown()
    {
        if (!isOpen)
        {
            OpenBag();
        }
    }

    void OpenBag()
    {
        isOpen = true;
        
        if (bagRenderer != null && openBagSprite != null)
        {
            bagRenderer.sprite = openBagSprite;
        }
        
        if (kulamboItem != null)
        {
            // Ilipat ang pwesto ng Kulambo doon mismo sa pwesto ng Bag bago siya lumitaw!
            kulamboItem.transform.position = transform.position; 
            kulamboItem.SetActive(true); 
        }

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.correctSound); 
        }
    }
}