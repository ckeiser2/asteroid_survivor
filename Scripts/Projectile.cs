using UnityEngine;
using static PlayerController;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Fire(Vector2 direction)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = direction * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Ignore player
        if (collision.gameObject.CompareTag("Player"))
            return;
    
        // Ignore other projectiles
        if (collision.gameObject.CompareTag("Projectile"))
            return;
    
        // Destroy on everything else (asteroids, walls, etc.)
        Destroy(gameObject);
    }
}