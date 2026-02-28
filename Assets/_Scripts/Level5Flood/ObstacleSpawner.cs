using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstacles;
    public Transform laneUpSpawn;
    public Transform laneDownSpawn;

    public float spawnInterval = 2f;
    public float moveSpeed = 5f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnObstacle), 1f, spawnInterval);
    }

    void SpawnObstacle()
    {
        int randomIndex = Random.Range(0, obstacles.Length);
        Transform spawnLane = Random.value > 0.5f ? laneUpSpawn : laneDownSpawn;

        GameObject obj = Instantiate(obstacles[randomIndex], spawnLane.position, Quaternion.identity);
        obj.AddComponent<ObstacleMover>().speed = moveSpeed;
    }
}