using UnityEngine;

public class EnvDragItem : MonoBehaviour
{
    private Vector3 startPos;
    private bool isDragging = false;
    private Vector3 offset;
    private Collider2D myCollider;

    void Start()
    {
        startPos = transform.position;
        myCollider = GetComponent<Collider2D>();
    }

    void OnMouseDown()
    {
        isDragging = true;
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        myCollider.enabled = false; // Papatayin muna para tagos ang detect
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
            transform.position = new Vector3(newPos.x, newPos.y, 0); 
        }
    }

    void OnMouseUp()
    {
        isDragging = false;

        // --- ANG FIX NATIN ---
        // 1. Mag-check muna kung ano ang natamaan sa ilalim (dahil naka-disable pa ang sarili niyang collider, lalagpas ito sa kanya at tatama sa Drop Zone).
        Collider2D hit = Physics2D.OverlapPoint(transform.position);

        // 2. Saka natin buhayin ulit ang sariling collider.
        myCollider.enabled = true; 
        // ---------------------

        if (hit != null)
        {
            HazardZone zone = hit.GetComponent<HazardZone>();

            if (zone != null)
            {
                // KUNG TAMA ANG ITEM
                if (zone.requiredItemTag == gameObject.tag)
                {
                    zone.SolveHazard(); 
                    Destroy(gameObject); 
                    return; 
                }
                // KUNG MALI ANG ITEM (Penalty!)
                else
                {
                    if (Level7Part2Manager.instance != null)
                    {
                        Level7Part2Manager.instance.WrongItem();
                    }
                }
            }
        }

        // Kung binitawan sa maling lugar, balik sa umpisa
        transform.position = startPos;
    }
}