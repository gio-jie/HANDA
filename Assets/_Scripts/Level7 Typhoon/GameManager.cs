using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject mosquitoPrefab; // Slot para sa Lamok
    public GameObject bucketPrefab;   // BAGONG SLOT: Slot para sa Timba
    
    public float spawnTime = 1.0f;
    public int playerHealth = 5;
    public bool isGameOver = false;

    void Start()
    {
        InvokeRepeating("SpawnEnemy", 0f, spawnTime);
    }

    void SpawnEnemy()
    {
        if (isGameOver) return;

        // Gumawa ng random position
        float randomX = Random.Range(-8f, 8f);
        float randomY = Random.Range(-4f, 4f);
        Vector2 spawnPos = new Vector2(randomX, randomY);

        // BAGONG LOGIC: Coin Toss (Random 0 or 1)
        // Kung 0 = Lamok, Kung 1 = Timba
        int randomPick = Random.Range(0, 2); 

        if (randomPick == 0)
        {
            Instantiate(mosquitoPrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            Instantiate(bucketPrefab, spawnPos, Quaternion.identity);
        }
    }

    public void TakeDamage()
    {
        if (isGameOver) return;

        playerHealth = playerHealth - 1;
        Debug.Log("ARAY! Health: " + playerHealth);

        if (playerHealth <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        isGameOver = true;
        Debug.Log("GAME OVER! Na-Dengue ka na!");
        CancelInvoke("SpawnEnemy");
    }
}