using UnityEngine;

public class ConveyorSpawner : MonoBehaviour
{
    [Header("Mga Items na Pwedeng Lumabas")]
    public GameObject[] itemPrefabs; // Dito natin ilalagay yung mga Prefabs mo

    [Header("Saan at Paano Ilalabas?")]
    public Transform spawnPoint;     // Ang pwesto kung saan sila ipapanganak (Kanan ng screen)
    public Transform parentCanvas;   // Kailangan nasa loob ng Canvas para makita ang UI!
    
    public float spawnInterval = 2f; // Ilang seconds bago maglabas ulit?

    void Start()
    {
        // Uulitin niya ang pagtawag sa "SpawnItem" tuwing 'spawnInterval' na segundo
        InvokeRepeating("SpawnItem", 1f, spawnInterval);
    }

    void SpawnItem()
    {
        // Kung Game Over o nanalo na, itigil na ang pag-spawn!
        if (Level6Manager.instance != null && !Level6Manager.instance.isGameActive)
        {
            return;
        }

        // 1. Pumili ng random na item mula sa array
        int randomIndex = Random.Range(0, itemPrefabs.Length);
        GameObject selectedItem = itemPrefabs[randomIndex];

        // 2. Gumawa ng kopya ng item sa pwesto ng Spawn Point
        GameObject newItem = Instantiate(selectedItem, spawnPoint.position, Quaternion.identity);

        // 3. Ipasok siya sa loob ng Canvas para hindi masira ang UI
        newItem.transform.SetParent(parentCanvas, false);

        // Siguraduhing nasa ilalim siya ng mga Info Panel at Boxes para hindi matakpan ang UI
        newItem.transform.SetAsFirstSibling(); 
    }
}