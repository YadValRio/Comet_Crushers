using System.Collections;
using UnityEngine;

public class CometEnvironment : MonoBehaviour {
    private Material groundMaterial;
    private Material metalMaterial;
    private Material darkMaterial;
    private Material starMaterial;

    //load the environment without changing the existing scene scripts
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void loadEnvironment() {
        if (Object.FindFirstObjectByType<CometEnvironment>() != null) {
            return; //already loaded
        }

        GameObject environment = new GameObject("Comet Environment");
        environment.AddComponent<CometEnvironment>();
    }

    private IEnumerator Start() {
        yield return null; //wait for the turret and camera setup

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) {
            Debug.LogWarning("Environment material shader was not found.");
            yield break;
        }

        groundMaterial = createMaterial(shader, new Color(.18f, .2f, .25f));
        metalMaterial = createMaterial(shader, new Color(.4f, .45f, .5f));
        darkMaterial = createMaterial(shader, new Color(.08f, .12f, .18f));
        starMaterial = createMaterial(shader, new Color(.8f, .85f, 1f));
        starMaterial.EnableKeyword("_EMISSION");
        starMaterial.SetColor("_EmissionColor", new Color(.8f, .85f, 1f));

        //change the ground color, keep its existing collision
        GameObject ground = GameObject.Find("Ground");
        if (ground != null) {
            Renderer groundRenderer = ground.GetComponent<Renderer>();
            if (groundRenderer != null) {
                groundRenderer.sharedMaterial = groundMaterial;
            }
        }

        //base platform and low buildings behind the turret
        createObject(PrimitiveType.Cylinder, "Base Platform", new Vector3(0, .25f, 0), new Vector3(11, .25f, 11), metalMaterial);
        createObject(PrimitiveType.Cube, "Rear Building", new Vector3(0, .7f, -10), new Vector3(8, 1.4f, 4), metalMaterial);
        createObject(PrimitiveType.Cube, "Building Roof", new Vector3(0, 1.45f, -10), new Vector3(8.4f, .1f, 4.4f), darkMaterial);
        createObject(PrimitiveType.Cube, "Left Storage", new Vector3(-11, .6f, -6), new Vector3(3, 1.2f, 4), metalMaterial);
        createObject(PrimitiveType.Cube, "Right Storage", new Vector3(11, .6f, -6), new Vector3(3, 1.2f, 4), metalMaterial);

        System.Random random = new System.Random(4361);

        //rocks stay off to the sides of the firing area
        for (int i = 0; i < 12; i++) {
            int side = i % 2 == 0 ? -1 : 1;
            float x = side * random.Next(30, 45);
            float z = random.Next(-15, 45);
            float size = random.Next(1, 4);
            createObject(PrimitiveType.Sphere, "Rock " + i, new Vector3(x, size * .3f, z), new Vector3(size, size * .7f, size), groundMaterial);
        }

        //small stars above the play area
        for (int i = 0; i < 45; i++) {
            Vector3 position = new Vector3(random.Next(-90, 90), random.Next(20, 80), random.Next(90, 150));
            createObject(PrimitiveType.Sphere, "Star " + i, position, Vector3.one * .18f, starMaterial);
        }

        Camera camera = Camera.main;
        if (camera != null) {
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(.01f, .02f, .045f);
            camera.farClipPlane = Mathf.Max(camera.farClipPlane, 250f);
        }

        //use the scene's light if it already has one
        Light sceneLight = Object.FindFirstObjectByType<Light>();
        if (sceneLight == null) {
            GameObject lightObject = new GameObject("Environment Light");
            lightObject.transform.SetParent(transform, false);
            lightObject.transform.rotation = Quaternion.Euler(40, -30, 0);
            sceneLight = lightObject.AddComponent<Light>();
            sceneLight.type = LightType.Directional;
        }
        sceneLight.color = new Color(.75f, .85f, 1f);
        sceneLight.intensity = 1f;
    }

    private Material createMaterial(Shader shader, Color color) {
        Material material = new Material(shader);
        material.SetColor("_BaseColor", color);
        return material;
    }

    private void createObject(PrimitiveType type, string name, Vector3 position, Vector3 scale, Material material) {
        GameObject obj = GameObject.CreatePrimitive(type);
        obj.name = name;
        obj.transform.SetParent(transform, false);
        obj.transform.position = position;
        obj.transform.localScale = scale;
        obj.GetComponent<Renderer>().sharedMaterial = material;

        Collider collider = obj.GetComponent<Collider>();
        if (collider != null) {
            collider.enabled = false; //decorations should not block projectiles
            Destroy(collider);
        }
    }

    private void OnDestroy() {
        Destroy(groundMaterial);
        Destroy(metalMaterial);
        Destroy(darkMaterial);
        Destroy(starMaterial);
    }
}
