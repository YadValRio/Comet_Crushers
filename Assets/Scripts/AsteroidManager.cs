using System.Collections.Generic;
using UnityEngine;

public class AsteroidManager : MonoBehaviour
{
    [Header("Asteroid Setup")]
    [Tooltip("Optional prefab. If left empty, a primitive sphere asteroid is created at runtime.")]
    public GameObject asteroidPrefab;
    public Texture[] textures;
    public Mesh[] meshes;

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

    readonly List<Asteroid> activeAsteroids = new List<Asteroid>();
    float nextSpawnTime;

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
        // Clean up any destroyed references
        activeAsteroids.RemoveAll(a => a == null);

        if (Time.time >= nextSpawnTime && activeAsteroids.Count < maxActiveAsteroids)
        {
            nextSpawnTime = Time.time + spawnInterval;
            SpawnAsteroid();
        }
    }

    public void SpawnAsteroid()
    {
        Vector3 spawnPos = GetRandomSpawnPosition();
        Quaternion spawnRot = Random.rotation;

        GameObject go;
        if (asteroidPrefab != null)
        {
            go = Instantiate(asteroidPrefab, spawnPos, spawnRot, transform);
        }
        else
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "Asteroid";
            go.transform.SetPositionAndRotation(spawnPos, spawnRot);
            go.transform.SetParent(transform, true);
        }

        float scale = Random.Range(minScale, maxScale);
        go.transform.localScale = Vector3.one * scale;

        ApplyRandomAppearance(go);

        if (!go.TryGetComponent<Asteroid>(out var asteroid))
        {
            asteroid = go.AddComponent<Asteroid>();
        }

        // Aim generally toward the play area / turret with some spread
        Vector3 targetPoint = new Vector3(
            Random.Range(-8f, 8f),
            Random.Range(1f, 6f),
            Random.Range(-8f, 8f)
        );
        Vector3 driftDir = (targetPoint - spawnPos).normalized;
        float speed = Random.Range(minSpeed, maxSpeed);

        asteroid.Initialize(this, driftDir * speed, rotationSpeed);
        activeAsteroids.Add(asteroid);
    }

    public void NotifyAsteroidDestroyed(Asteroid asteroid)
    {
        activeAsteroids.Remove(asteroid);
    }

    Vector3 GetRandomSpawnPosition()
    {
        float angle = Random.Range(-110f, 110f) * Mathf.Deg2Rad;
        float radius = Random.Range(minSpawnRadius, maxSpawnRadius);
        float y = Random.Range(minHeight, maxHeight);

        float x = Mathf.Sin(angle) * radius;
        float z = Mathf.Cos(angle) * radius;
        return new Vector3(x, y, z);
    }

    void ApplyRandomAppearance(GameObject go)
    {
        if (meshes != null && meshes.Length > 0)
        {
            var selectedMesh = meshes[Random.Range(0, meshes.Length)];
            if (go.TryGetComponent<MeshFilter>(out var mf))
            {
                mf.mesh = selectedMesh;
            }
            if (go.TryGetComponent<MeshCollider>(out var mc))
            {
                mc.sharedMesh = selectedMesh;
            }
        }

        if (textures != null && textures.Length > 0 && go.TryGetComponent<Renderer>(out var renderer))
        {
            renderer.material.mainTexture = textures[Random.Range(0, textures.Length)];
        }
    }
}

public class Asteroid : MonoBehaviour, IDamageable
{
    public float health = 3f;

    AsteroidManager manager;
    Vector3 velocity;
    float spinSpeed;
    Vector3 spinAxis;

    public void Initialize(AsteroidManager owner, Vector3 moveVelocity, float rotationDegPerSec)
    {
        manager = owner;
        velocity = moveVelocity;
        spinSpeed = rotationDegPerSec;
        spinAxis = Random.onUnitSphere;
    }

    void Update()
    {
        transform.position += velocity * Time.deltaTime;
        transform.Rotate(spinAxis, spinSpeed * Time.deltaTime, Space.Self);
    }

    public void Hit(float damage, Vector3 point)
    {
        health -= damage;
        if (health <= 0f)
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (manager != null)
        {
            manager.NotifyAsteroidDestroyed(this);
        }
    }
}