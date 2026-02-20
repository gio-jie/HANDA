using UnityEngine;

public class ObstacleSpawnerLevel6 : MonoBehaviour
{
    public GameObject[] obstacles;
    public Transform[] spawnPoints;

    [Range(0f, 1f)]
    public float spawnChance = 0.5f;

    public float spawnOffsetYAboveCamera = 6f;

    public float spawnInterval = 2f;
    public float moveSpeed = 3f;

    void Start()
    {
        InvokeRepeating(nameof(TrySpawn), 1f, spawnInterval);
    }

    void TrySpawn()
    {
        if (Random.value > spawnChance)
            return;

        SpawnObstacleAboveCamera();
    }

    void SpawnObstacleAboveCamera()
    {
        int randomObstacle = Random.Range(0, obstacles.Length);
        int randomLane = Random.Range(0, spawnPoints.Length);

        float camTopY = Camera.main.transform.position.y + Camera.main.orthographicSize;

        float spawnY = camTopY + spawnOffsetYAboveCamera;

        Vector3 spawnPos = new Vector3(
            spawnPoints[randomLane].position.x,
            spawnY,
            0f
        );

        GameObject obj = Instantiate(
            obstacles[randomObstacle],
            spawnPos,
            Quaternion.identity
        );

        obj.AddComponent<DebrisDrift>().driftSpeed = moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Floodwater"))
        {
            PlatformSpawnerLevel6.Instance.RemovePlatform(gameObject);
        }
    }
}