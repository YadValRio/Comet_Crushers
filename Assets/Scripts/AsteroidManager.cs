using System.Collections.Generic;
using UnityEngine;

public class AsteroidManager : MonoBehaviour
{
    public static AsteroidManager Instance { get; private set; }

    [Header("Asteroid Prefab")]
    public GameObject asteroidPrefab;

    [Header("Spawning")]
    public int initialCount = 8;
    public int maxActiveAsteroids = 12;
    public float spawnInterval = 3f;
    public float minSpawnRadius = 18f;
    public float maxSpawnRadius = 38f;
    public float minHeight = 2f;
    public float maxHeight = 16f;
    public float minScale = 1.5f;
    public float maxScale = 4.5f;

    [Header("Movement")]
    public float minSpeed = 1.5f;
    public float maxSpeed = 5f;
    public float rotationSpeed = 35f;

    [Header("Score Display")]
    [SerializeField] private int scoreFontSize = 20;

    private GUIStyle scoreStyle;

    public int AsteroidsDestroyed { get; private set; }

    readonly List<GameObject> activeAsteroids = new List<GameObject>();
    float nextSpawnTime;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        for (int i = 0; i < initialCount; i++)
        {
            SpawnAsteroid();
        }

        nextSpawnTime = Time.time + spawnInterval;
    }

    void Update()
    {
        activeAsteroids.RemoveAll(a => a == null);

        if (Time.time >= nextSpawnTime && activeAsteroids.Count < maxActiveAsteroids)
        {
            nextSpawnTime = Time.time + spawnInterval;
            SpawnAsteroid();
        }
    }

    public void RegisterAsteroidDestroyed()
    {
        AsteroidsDestroyed++;
    }

    public void SpawnAsteroid()
    {
        if (asteroidPrefab == null)
        {
            Debug.LogWarning("AsteroidManager: Assign an asteroidPrefab in the Inspector.");
            return;
        }

        Vector3 spawnPos = GetRandomSpawnPosition();
        GameObject go = Instantiate(asteroidPrefab, spawnPos, Random.rotation, transform);
        go.transform.localScale = Vector3.one * Random.Range(minScale, maxScale);

        if (go.TryGetComponent<AsteroidController>(out var controller))
        {
            Vector3 targetPoint = new Vector3(
                Random.Range(-8f, 8f),
                Random.Range(1f, 6f),
                Random.Range(-8f, 8f)
            );
            Vector3 driftDir = (targetPoint - spawnPos).normalized;
            controller.Initialize(driftDir * Random.Range(minSpeed, maxSpeed), rotationSpeed);
        }

        activeAsteroids.Add(go);
    }

    Vector3 GetRandomSpawnPosition()
    {
        float angle = Random.Range(-110f, 110f) * Mathf.Deg2Rad;
        float radius = Random.Range(minSpawnRadius, maxSpawnRadius);
        float y = Random.Range(minHeight, maxHeight);
        return new Vector3(Mathf.Sin(angle) * radius, y, Mathf.Cos(angle) * radius);
    }

    void OnGUI()
    {
        if (scoreStyle == null)
        {
            scoreStyle = new GUIStyle(GUI.skin.label);
        }

        scoreStyle.fontSize = scoreFontSize;
        scoreStyle.normal.textColor = Color.white;

        GUI.Label(new Rect(16f, 16f, 360f, scoreFontSize + 16f), $"Asteroids Destroyed: {AsteroidsDestroyed}", scoreStyle);
    }
}