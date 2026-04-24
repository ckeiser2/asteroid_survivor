using UnityEngine;

public class ShieldCompanion : MonoBehaviour
{
    private PlayerController player;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (player == null) return;

        if (!player.shieldActive) return;

        if (collision.gameObject.CompareTag("Asteroid"))
        {
            Destroy(collision.gameObject);

            player.BreakShield();
        }
    }
}