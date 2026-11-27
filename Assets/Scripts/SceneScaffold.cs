using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Builds a minimal playable scene (ground, base, turret) at Play time
/// ONLY when the open scene has no TurretController. It never touches the saved
/// scene file, so it can't cause merge conflicts with Kyle's environment work.
/// Once the real scene has a turret, this does nothing.
/// </summary>
public static class SceneScaffold
{
    //runs after scene load to fully initilize scene
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Initialize()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Build();
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Build();
    }

    static void Build()
    {
        PauseMenu.EnsureExists();
        if (Object.FindAnyObjectByType<TurretController>() != null) return;

        Make(PrimitiveType.Plane, "Ground", new Vector3(0, 0, 0), new Vector3(8, 1, 8), new Color(0.15f, 0.17f, 0.2f));
        Make(PrimitiveType.Cube, "Base (placeholder)", new Vector3(0, 0.5f, 0), new Vector3(6, 1, 6), new Color(0.35f, 0.4f, 0.5f));

        var turret = new GameObject("Turret");
        turret.transform.position = new Vector3(0, 1f, 0);
        var pedestal = Make(PrimitiveType.Cylinder, "Pedestal", Vector3.zero, new Vector3(1.2f, 0.6f, 1.2f), new Color(0.25f, 0.28f, 0.33f));
        pedestal.transform.SetParent(turret.transform, false);
        pedestal.transform.localPosition = new Vector3(0, 0.6f, 0);

        var head = new GameObject("Head");
        head.transform.SetParent(turret.transform, false);
        head.transform.localPosition = new Vector3(0, 1.5f, 0);

        var barrel = Make(PrimitiveType.Cube, "Barrel", Vector3.zero, new Vector3(0.2f, 0.2f, 1.4f), new Color(0.6f, 0.6f, 0.65f));
        barrel.transform.SetParent(head.transform, false);
        barrel.transform.localPosition = new Vector3(0.4f, -0.35f, 0.9f);

        var cam = Camera.main;
        if (cam == null)
        {
            var camGo = new GameObject("Main Camera") { tag = "MainCamera" };
            cam = camGo.AddComponent<Camera>();
            camGo.AddComponent<AudioListener>();
        }
        cam.transform.SetParent(head.transform, false);
        cam.transform.localPosition = Vector3.zero;
        cam.transform.localRotation = Quaternion.identity;

        var controller = turret.AddComponent<TurretController>();
        controller.head = head.transform;
        controller.aimCamera = cam;
    }

    static GameObject Make(PrimitiveType type, string name, Vector3 pos, Vector3 scale, Color color)
    {
        var go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.transform.position = pos;
        go.transform.localScale = scale;
        var r = go.GetComponent<Renderer>();
        if (r != null) r.material.color = color;
        return go;
    }
}
