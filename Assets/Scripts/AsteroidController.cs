using UnityEngine;

public class AsteroidController : MonoBehaviour, IDamageable
{
    [SerializeField] private Texture[] textures;
    [SerializeField] private Mesh[] meshes;
    [SerializeField] private float health = 3f;
    [SerializeField] private float baseDamage = 10f;

    Collider asteroidCollider;
    Collider baseCollider;
    
    BaseHealth baseHealth;

    Vector3 velocity;
    float spinSpeed;
    Vector3 spinAxis;
    bool initialized;

    public void Initialize(Vector3 moveVelocity, float rotationDegPerSec)
    {
        velocity = moveVelocity;
        spinSpeed = rotationDegPerSec;
        spinAxis = Random.onUnitSphere;
        initialized = true;
    }

    void Start()
{
    asteroidCollider = GetComponent<Collider>();
    baseHealth = BaseHealth.Instance;

    if (baseHealth != null)
        baseCollider = baseHealth.GetComponent<Collider>();

    if (textures != null && textures.Length > 0)
    {
        int textureIndex = Random.Range(0, textures.Length);
        GetComponent<Renderer>().material.mainTexture = textures[textureIndex];
    }

    if (meshes != null && meshes.Length > 0)
    {
        int meshIndex = Random.Range(0, meshes.Length);
        var selectedMesh = meshes[meshIndex];
        GetComponent<MeshFilter>().mesh = selectedMesh;

        if (TryGetComponent<MeshCollider>(out var meshCollider))
            meshCollider.sharedMesh = selectedMesh;
    }
}

    void Update()
    {
        if (!initialized) return;

        transform.position += velocity * Time.deltaTime;
        transform.Rotate(spinAxis, spinSpeed * Time.deltaTime, Space.Self);

        bool hitBase = baseHealth != null && baseCollider != null && asteroidCollider != null &&
                    baseCollider.bounds.Intersects(asteroidCollider.bounds);

        if (!hitBase && baseHealth != null)
            hitBase = Vector3.Distance(transform.position, baseHealth.transform.position) <= 4.5f;

        if (hitBase)
        {
            baseHealth.TakeDamage(baseDamage);
            Destroy(gameObject);
        }
    }

    public void Hit(float damage, Vector3 point)
    {
        health -= damage;
        if (health <= 0f)
        {
            if (AsteroidManager.Instance != null)
            {
                AsteroidManager.Instance.RegisterAsteroidDestroyed();
            }
            Destroy(gameObject);
        }
    }
}