using UnityEngine;

/// <summary>
/// Placeholder target so the turret can be tested before real asteroids exist.
/// Delete (or stop spawning) once Yadiel's asteroids implement IDamageable.
/// </summary>
public class TestTarget : MonoBehaviour, IDamageable
{
    public float health = 3f;

    public void Hit(float damage, Vector3 point)
    {
        health -= damage;
        var r = GetComponent<Renderer>();
        if (r != null) r.material.color = Color.Lerp(Color.red, Color.gray, Mathf.Clamp01(health / 3f));
        transform.localScale *= 0.85f;
        if (health <= 0f) Destroy(gameObject);
    }
}
