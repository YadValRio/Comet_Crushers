using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    [SerializeField] private Texture[] textures;
    [SerializeField] private Mesh[] meshes;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Randomly select a texture and mesh for the asteroid
        int textureIndex = Random.Range(0, textures.Length);
        int meshIndex = Random.Range(0, meshes.Length);

        // Apply the selected texture and mesh to the asteroid
        GetComponent<Renderer>().material.mainTexture = textures[textureIndex];
        GetComponent<MeshFilter>().mesh = meshes[meshIndex];

        var selectedMesh = meshes[meshIndex];
        GetComponent<MeshFilter>().mesh = selectedMesh;

        if (TryGetComponent<MeshCollider>(out var meshCollider))
        {
            meshCollider.sharedMesh = selectedMesh;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
