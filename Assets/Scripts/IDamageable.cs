using UnityEngine;

/// <summary>
/// Anything a turret projectile can hurt (asteroids, test targets, ...).
/// Yadiel's asteroids and Wayne's hit effects only need to implement this;
/// no turret code has to change.
/// </summary>
public interface IDamageable
{
    void Hit(float damage, Vector3 point);
}
