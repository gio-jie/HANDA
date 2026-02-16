using UnityEngine;

public class HazardZone : MonoBehaviour
{
    public string requiredItemTag; // e.g., "Item_Kulambo"
    public Sprite safeSprite;      // Ang ipapalit na image pag na-solve

    public void SolveHazard()
    {
        SpriteRenderer myRenderer = GetComponent<SpriteRenderer>();

        if (safeSprite != null)
        {
            myRenderer.sprite = safeSprite; // Palitan ang drawing
        }

        // Sabihan ang Manager na tama!
        if (Level7Part2Manager.instance != null)
        {
            Level7Part2Manager.instance.AddScore();
        }

        // Papatayin ang sariling collider para hindi na ulit madaganan ng ibang item
        GetComponent<Collider2D>().enabled = false;
    }
}