using UnityEngine;

public class ConveyorSpawner : MonoBehaviour
{
    [Header("Mga Items na Pwedeng Lumabas")]
    public GameObject[] itemPrefabs; 

    [Header("Saan at Paano Ilalabas?")]
    public Transform spawnPoint;     
    public Transform parentCanvas;   
    
    public float spawnInterval = 10f; 

    void Start()
    {
        InvokeRepeating("SpawnItem", 1f, spawnInterval);
    }

    void SpawnItem()
    {
        if (Level6Manager.instance != null && !Level6Manager.instance.isGameActive)
        {
            return;
        }

        int randomIndex = Random.Range(0, itemPrefabs.Length);
        GameObject selectedItem = itemPrefabs[randomIndex];

        // 1. Instantiate na may 'false' para mapanatili ang local properties ng UI!
        GameObject newItem = Instantiate(selectedItem, parentCanvas, false);

        // --- BRUTE FORCE FIX PARA SA EXACT WIDTH & HEIGHT ---
        RectTransform newRect = newItem.GetComponent<RectTransform>();
        RectTransform prefabRect = selectedItem.GetComponent<RectTransform>();
        
        if (newRect != null && prefabRect != null)
        {
            // Pwersahang ilapat ang eksaktong Width
            newRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, prefabRect.rect.width);
            
            // Pwersahang ilapat ang eksaktong Height
            newRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, prefabRect.rect.height);
            
            // Kopyahin ang Scale para sure!
            newRect.localScale = prefabRect.localScale;
        }
        // --------------------------------------------------

        newItem.transform.position = spawnPoint.position;
        newItem.transform.SetSiblingIndex(10); 
    }
}