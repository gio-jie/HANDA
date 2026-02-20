using UnityEngine;
using System.Collections.Generic;

public class PlatformSpawnerLevel6 : MonoBehaviour
{
    public static PlatformSpawnerLevel6 Instance;

    [Header("Platforms")]
    public GameObject[] platformPrefabs;
    public Sprite[] platformSprites;

    [Header("Lanes")]
    public Transform[] spawnPoints;

    [Header("Spawn Settings")]
    public bool spawningEnabled = false;
    public float spawnOffsetYAboveCamera = 5f;
    public float minVerticalSpacing = 2.5f;

    private int lastLane = -1;
    private float lastSpawnY = float.MinValue;

    [Header("Active Platforms")]
    public List<GameObject> activePlatforms = new List<GameObject>();

    [Header("Player")]
    public Transform player;

    [Header("Power-Up Settings")]
    public float extraSpawnHeightDuringPowerUp = 5f;

    public GameObject cookiePrefab;

    [Range(0f, 1f)]
    public float cookieSpawnChance = 0.2f;

    void Awake()
    {
        Instance = this;
    }

    public void EnableSpawning()
    {
        SpawnPlatformAboveCamera();
    }

    public void SpawnPlatformAboveCamera()
    {
        if (platformPrefabs.Length == 0 || spawnPoints.Length == 0) return;

        float camTopY = Camera.main.transform.position.y + Camera.main.orthographicSize;

        float dynamicSpacing = minVerticalSpacing;
        float dynamicOffset = spawnOffsetYAboveCamera;
        int extraLayers = 0;

        bool highJump = StarManagerLevel6.Instance != null &&
                        StarManagerLevel6.Instance.IsHighJumpActive();

        if (highJump)
        {
            dynamicSpacing = 0.05f;
            dynamicOffset += 0.05f;
            extraLayers = 3;
        }

        if (lastSpawnY > camTopY + dynamicOffset + 1f)
            return;

        float desiredY = camTopY + dynamicOffset;

        float spawnY = (lastSpawnY == float.MinValue)
            ? desiredY
            : Mathf.Max(desiredY, lastSpawnY + dynamicSpacing);

        int totalLayers = 1 + extraLayers;

        for (int i = 0; i < totalLayers; i++)
        {
            GameObject prefab = platformPrefabs[Random.Range(0, platformPrefabs.Length)];
            int lane = GetNearbyLane();
            lastLane = lane;

            float platformY = spawnY + i * dynamicSpacing;

            Vector3 spawnPos = new Vector3(spawnPoints[lane].position.x, platformY, 0f);

            GameObject platform = Instantiate(prefab, spawnPos, Quaternion.identity);

            SpriteRenderer sr = platform.GetComponent<SpriteRenderer>();
            if (sr != null && platformSprites.Length > 0)
                sr.sprite = platformSprites[Random.Range(0, platformSprites.Length)];

            activePlatforms.Add(platform);

            float adjustedChance = highJump ? cookieSpawnChance * 0.005f : cookieSpawnChance;

            if (Random.value < adjustedChance)
            {
                Vector3 cookiePos = spawnPos + Vector3.up * 1.5f;
                GameObject cookie = Instantiate(cookiePrefab, cookiePos, Quaternion.identity);
                cookie.transform.SetParent(platform.transform);
            }

            lastSpawnY = platformY;
        }
    }

    int GetNearbyLane()
    {
        if (lastLane == -1)
            return Random.Range(0, spawnPoints.Length);

        int min = Mathf.Max(0, lastLane - 1);
        int max = Mathf.Min(spawnPoints.Length - 1, lastLane + 1);

        int newLane;
        do
        {
            newLane = Random.Range(min, max + 1);
        }
        while (newLane == lastLane);

        return newLane;
    }

    public void RemovePlatform(GameObject plat)
    {
        if (activePlatforms.Contains(plat))
            activePlatforms.Remove(plat);

        Destroy(plat);
    }
}
