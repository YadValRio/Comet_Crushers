using UnityEngine;

/// <summary>
/// Straight-flying shot. On contact it calls IDamageable.Hit on whatever it
/// touched (searching parents too) and destroys itself.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public float speed = 60f;
    public float damage = 1f;
    public float lifetime = 4f;

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    void Start()
    {
        rb.linearVelocity = transform.forward * speed;
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        var target = collision.collider.GetComponentInParent<IDamageable>();
        if (target != null)
        {
            Vector3 point = collision.contactCount > 0 ? collision.GetContact(0).point : transform.position;
            target.Hit(damage, point);
        }
        Destroy(gameObject);
    }
}
