using UnityEngine;
using System.Collections;

public class DragAndDrop : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 startPosition;
    private Vector3 offset;

    void Start()
    {
        // I-save ang original position
        startPosition = transform.position;
    }

    void OnMouseDown()
    {
        // Kapag clinick, magsisimula ang drag
        isDragging = true;
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void OnMouseDrag()
    {
        // Habang naka-hold ang click, susunod ang object sa mouse
        if (isDragging)
        {
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
            transform.position = new Vector3(newPosition.x, newPosition.y, 0); 
        }
    }

    void OnMouseUp()
    {
        // Pag binitawan ang click
        isDragging = false;
        CheckDrop();
    }

    void CheckDrop()
    {
        // STEP 1: Patayin muna ang sariling collider para hindi ito ang madetect
        GetComponent<Collider2D>().enabled = false;

        // STEP 2: Kapain kung ano ang nasa ilalim (Yung Bin)
        Collider2D hit = Physics2D.OverlapPoint(transform.position);

        // STEP 3: Buhayin ulit ang collider
        GetComponent<Collider2D>().enabled = true;

        if (hit != null)
        {
            // Debugging: Tignan natin sa Console kung ano ang tinamaan
            Debug.Log("Ang tinamaan ay: " + hit.gameObject.name + " na may Tag na: " + hit.tag);

            // Logic: Kung match ang tags
            if (gameObject.tag == "Food" && hit.tag == "Bin_Food")
            {
                Debug.Log("TAMA! Pumasok sa Food Bin.");
                Destroy(gameObject); 
            }
            else if (gameObject.tag == "Toy" && hit.tag == "Bin_Toy")
            {
                Debug.Log("TAMA! Pumasok sa Toy Bin.");
                Destroy(gameObject); 
            }
            else
            {
                Debug.Log("Maling lalagyan! (Ibabalik sa pwesto)");
                transform.position = startPosition; 
            }
        }
        else
        {
            Debug.Log("Walang tinamaan (Bininitawan sa hangin).");
            transform.position = startPosition;
        }
    }
}