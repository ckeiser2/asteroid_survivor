using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public float spawnInterval = 1f;
    public float spawnDistance = 10f;
    private float timer = 0f;
    public int maxAsteroids = 25;

    [Header("Player Reference")]
    public Transform playerTransform;
    public PlayerController player;

    [Header("Spawn Bounds")]
    public float minDistanceFromPlayer = 10f;
    public float minX = -18f;
    public float maxX = 17f;
    public float minY = -10f;
    public float maxY = 10f;

    [Header("Asteroid Prefabs")]
    public GameObject defaultAsteroid;
    public GameObject iceAsteroid;
    public GameObject fireAsteroid;
    public GameObject whiteAsteroid;
    public GameObject blackAsteroid;
    public GameObject cometAsteroid;


    void Update()
    {
        timer += Time.deltaTime;

        // Gradually spawn faster over time
        spawnInterval = Mathf.Max(0.5f, spawnInterval - Time.deltaTime * 0.01f);

        if (timer >= spawnInterval)
        {
            SpawnAsteroid();
            timer = 0f;
        }
    }

    GameObject GetAsteroidToSpawn()
    {
        int count = player.asteroidsDestroyed;

        if (count < 10)
            return defaultAsteroid;
        else
        {
            float rand = Random.value;
            if (rand < 0.55f) return defaultAsteroid;
            if (rand < 0.70f) return iceAsteroid;
            if (rand < 0.85f) return fireAsteroid;
            if (rand < 0.90f) return cometAsteroid;
            if (rand < 0.95f) return whiteAsteroid;
            return blackAsteroid;
        }
    }

    void SpawnAsteroid()
    {
        if (GameObject.FindGameObjectsWithTag("Asteroid").Length >= maxAsteroids)
            return;

        Vector2 spawnPosition;
        int attempts = 0;

        do
        {
            float x = Random.Range(minX, maxX);
            float y = Random.Range(minY, maxY);
            spawnPosition = new Vector2(x, y);
            attempts++;
        } 
        while (Vector2.Distance(spawnPosition, playerTransform.position) < minDistanceFromPlayer && attempts < 10);

        GameObject toSpawn = GetAsteroidToSpawn();
        Instantiate(toSpawn, spawnPosition, Quaternion.identity);
    }
}