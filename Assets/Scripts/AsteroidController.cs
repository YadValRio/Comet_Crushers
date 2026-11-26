using UnityEngine;

public class AsteroidController : MonoBehaviour, IDamageable
{
    [SerializeField] private Texture[] textures;
    [SerializeField] private Mesh[] meshes;
    [SerializeField] private float health = 3f;

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
            {
                meshCollider.sharedMesh = selectedMesh;
            }
        }
    }

    void Update()
    {
        if (!initialized) return;

        transform.position += velocity * Time.deltaTime;
        transform.Rotate(spinAxis, spinSpeed * Time.deltaTime, Space.Self);
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