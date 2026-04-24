using UnityEngine;
using UnityEngine.InputSystem;

public class BlasterCompanion : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint; // Where the projectile spawns
    public float fireRate = .5f; // Time between shots
    private float lastFireTime;
    public float projectileSpeed = 10f;
    public float projectileLifetime = 3f;
    Rigidbody2D rb;

    void Update()
    {
        rb = GetComponent<Rigidbody2D>();

        MoveCompanion();
        
        // Fire automatically if enough time has passed
        if (Keyboard.current.spaceKey.isPressed && Time.time > lastFireTime + fireRate)
        {
            Fire(transform.up); // Fires in the direction the companion is facing
            lastFireTime = Time.time;
        }
    }

    public void Fire(Vector2 direction)
    {
        if (projectilePrefab == null || firePoint == null) return;

        // Spawn projectile
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // Set velocity like Projectile script
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;
        }

        // Destroy after lifetime
        Destroy(proj, projectileLifetime);
    }
    void MoveCompanion()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            // Calculate mouse direction
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
            Vector2 direction = (mousePos - transform.position).normalized;

            // Move companion in direction of mouse
            transform.up = direction;
            rb.AddForce(direction * 2);
        }
    }
}