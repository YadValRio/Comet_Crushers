using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Player turret: mouse aims (yaw on this object, pitch on <see cref="head"/>),
/// left mouse fires. The camera is a child of <see cref="head"/> so the player
/// sees from the turret's perspective. Esc frees the cursor, click re-locks it.
/// </summary>
public class TurretController : MonoBehaviour
{
    [Header("References")]
    public Transform head;          // pitches up/down; camera is its child
    public Camera aimCamera;

    [Header("Aiming")]
    public float sensitivity = 0.12f;       // degrees per mouse pixel
    public float minPitch = -10f;
    public float maxPitch = 80f;
    public float yawLimit = 180f;           // total +/- from start heading (180 = free spin)

    [Header("Firing")]
    public float fireRate = 6f;             // shots per second
    public float projectileSpeed = 60f;
    public float projectileDamage = 1f;

    float yaw, pitch, nextFireTime, startYaw;

    void Start()
    {
        startYaw = yaw = transform.eulerAngles.y;
        LockCursor(true);
    }

    void Update()
    {
        var mouse = Mouse.current;
        var keyboard = Keyboard.current;
        if (mouse == null) return;

        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame) LockCursor(false);
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            if (mouse.leftButton.wasPressedThisFrame) LockCursor(true);
            return;
        }

        Aim(mouse.delta.ReadValue());

        if (mouse.leftButton.isPressed && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
            Fire();
        }
    }

    void Aim(Vector2 delta)
    {
        yaw += delta.x * sensitivity;
        pitch = Mathf.Clamp(pitch - delta.y * sensitivity, minPitch, maxPitch);
        yaw = Mathf.Clamp(yaw, startYaw - yawLimit, startYaw + yawLimit);
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        if (head != null) head.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void Fire()
    {
        Transform aim = aimCamera != null ? aimCamera.transform : head;
        Vector3 spawn = aim.position + aim.forward * 1.5f;   // shots travel along the crosshair line

        var shot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        shot.name = "Projectile";
        shot.transform.SetPositionAndRotation(spawn, aim.rotation);
        shot.transform.localScale = Vector3.one * 0.25f;
        var r = shot.GetComponent<Renderer>();
        if (r != null) r.material.color = new Color(1f, 0.85f, 0.2f);

        shot.AddComponent<Rigidbody>();
        var p = shot.AddComponent<Projectile>();
        p.speed = projectileSpeed;
        p.damage = projectileDamage;
    }

    static void LockCursor(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    void OnGUI()
    {
        // Simple crosshair
        float cx = Screen.width * 0.5f, cy = Screen.height * 0.5f;
        GUI.color = Color.white;
        GUI.DrawTexture(new Rect(cx - 8f, cy - 1f, 16f, 2f), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(cx - 1f, cy - 8f, 2f, 16f), Texture2D.whiteTexture);
        if (Cursor.lockState != CursorLockMode.Locked)
            GUI.Label(new Rect(cx - 90f, cy + 20f, 220f, 24f), "Click to aim (Esc frees mouse)");
    }
}
