using UnityEngine;

public class ConveyorSpawner : MonoBehaviour
{
    [Header("Mga Items na Pwedeng Lumabas")]
    public GameObject[] itemPrefabs; 

    [Header("Saan at Paano Ilalabas?")]
    public Transform spawnPoint;     
    public Transform parentCanvas;   
    
    public float spawnInterval = 2f; 

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

        GameObject newItem = Instantiate(selectedItem, parentCanvas);

        // --- DITO ANG ULTIMATE FIX PARA SA WIDTH & HEIGHT! ---
        RectTransform newRect = newItem.GetComponent<RectTransform>();
        RectTransform prefabRect = selectedItem.GetComponent<RectTransform>();
        
        if (newRect != null && prefabRect != null)
        {
            // Kopyahin ang eksaktong Width at Height ng Prefab mo!
            newRect.sizeDelta = prefabRect.sizeDelta;
            
            // Kopyahin na rin ang Scale para sigurado
            newRect.localScale = prefabRect.localScale;
        }
        // -----------------------------------------------------

        newItem.transform.position = spawnPoint.position;
        newItem.transform.SetSiblingIndex(1); 
    }
}