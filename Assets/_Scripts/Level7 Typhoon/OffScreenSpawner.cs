using UnityEngine;

public class OffScreenSpawner : MonoBehaviour
{
    public GameObject mosquitoPrefab; 
    public float spawnRate = 1.5f;    
    public float offScreenX = 10f; 
    public float offScreenY = 6f;

    void Start()
    {
        InvokeRepeating("SpawnMosquito", 1f, spawnRate);
    }

    void SpawnMosquito()
    {
        if (Level7Manager.instance != null && !Level7Manager.instance.isGameActive) return; 

        Vector2 spawnPos = Vector2.zero;
        int side = Random.Range(0, 4);

        if (side == 0) spawnPos = new Vector2(Random.Range(-offScreenX, offScreenX), offScreenY); // TAAS
        else if (side == 1) spawnPos = new Vector2(Random.Range(-offScreenX, offScreenX), -offScreenY); // BABA
        else if (side == 2) spawnPos = new Vector2(-offScreenX, Random.Range(-offScreenY, offScreenY)); // KALIWA
        else if (side == 3) spawnPos = new Vector2(offScreenX, Random.Range(-offScreenY, offScreenY)); // KANAN

        Instantiate(mosquitoPrefab, spawnPos, Quaternion.identity);
    }
}