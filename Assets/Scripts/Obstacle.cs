using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public enum AsteroidType { Default, Ice, Fire, Blackhole, White, Comet }
    public AsteroidType asteroidType;

    [Header("Size & Speed")]
    public float minSize = 0.5f;
    public float maxSize = 3.0f;
    public float minSpeed = 50f;
    public float maxSpeed = 150f;
    public float maxSpinSpeed = 10f;

    [Header("Effects")]
    public GameObject bounceEffectPrefab;
    public GameObject iceTrailPrefab;
    public GameObject fireTrailPrefab;
    public GameObject fireTrailPrefab2;

    public GameObject blackTrailPrefab;
    public GameObject whiteTrailPrefab;
    public GameObject cometTrailPrefab;
    public GameObject cometTrailPrefab2;

    Rigidbody2D rb;
    private Transform playerTransform;
    public GameObject iceFragmentPrefab;
    private int fragmentsToSpawn = 3;
    private float fragmentScaleMultiplier = 0.5f;
    public bool canSplit = true;


    int GetScoreValue()
    {
        int baseScore;

        switch (asteroidType)
        {
            case AsteroidType.Ice: baseScore = 75; break;
            case AsteroidType.Fire: baseScore = 100; break;
            case AsteroidType.Blackhole: baseScore = 150; break;
            case AsteroidType.White: baseScore = 200; break;
            case AsteroidType.Comet: baseScore = 500; break;
            default: baseScore = 50; break;
        }

        // Scale by size
        float sizeMultiplier = transform.localScale.x;
        return Mathf.RoundToInt(baseScore * sizeMultiplier);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTransform = FindObjectOfType<PlayerController>()?.transform;
        
        // Random size
        float randomSize = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(randomSize, randomSize, 1);

        // Base speed adjusted for type
        float baseSpeed = Random.Range(minSpeed, maxSpeed) / randomSize;

        switch (asteroidType)
        {
            case AsteroidType.Ice:
                baseSpeed *= 1.3f; // faster than default
                rb.linearDamping = 0.2f;    // slippery
                rb.angularDamping = 0.05f;
                if (iceTrailPrefab)
                {
                    GameObject trail = Instantiate(iceTrailPrefab, transform.position, Quaternion.identity, transform);
                }
                break;

            case AsteroidType.Blackhole:
                baseSpeed *= 1f; 
                if (blackTrailPrefab)
                {
                    GameObject trail = Instantiate(blackTrailPrefab, transform.position, Quaternion.identity, transform);
                }
                break;

            case AsteroidType.White:
                baseSpeed *= 1; 
                if (whiteTrailPrefab)
                {
                    GameObject trail = Instantiate(whiteTrailPrefab, transform.position, Quaternion.identity, transform);
                }
                break;

            case AsteroidType.Comet:
                baseSpeed *= 3f; 
                if (cometTrailPrefab)
                {
                    GameObject trail = Instantiate(cometTrailPrefab, transform.position, Quaternion.identity, transform);
                }
                if (cometTrailPrefab2)
                {
                    GameObject trail = Instantiate(cometTrailPrefab2, transform.position, Quaternion.identity, transform);
                }
                
                // Slight homing toward player
                if (playerTransform != null)
                {
                    Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
                    rb.AddForce(directionToPlayer * 50f);
                }
                break;

            case AsteroidType.Fire:
                baseSpeed *= 2f; // very fast
                if (fireTrailPrefab)
                {
                    GameObject trail = Instantiate(fireTrailPrefab, transform.position, Quaternion.identity, transform);
                }
                // Wanted to add second sfx to fire asteroid
                if (fireTrailPrefab2)
                {
                    GameObject trail = Instantiate(fireTrailPrefab2, transform.position, Quaternion.identity, transform);
                }

                // Slight homing toward player
                if (playerTransform != null)
                {
                    Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
                    rb.AddForce(directionToPlayer * 30f);
                }
                break;

            case AsteroidType.Default:
                rb.linearDamping = 0.3f;
                rb.angularDamping = 0.1f;
                break;
        }

        // Random movement and spin
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        rb.AddForce(randomDirection * baseSpeed);
        float randomTorque = Random.Range(-maxSpinSpeed, maxSpinSpeed);
        rb.AddTorque(randomTorque);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Collision with projectile
        if (collision.gameObject.CompareTag("Projectile"))
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
                player.AddScore(GetScoreValue());

            if (bounceEffectPrefab)
            {
                Vector2 contactPoint = collision.GetContact(0).point;
                GameObject effect = Instantiate(bounceEffectPrefab, contactPoint, Quaternion.identity);
                Destroy(effect, 1f);
            }

            if (asteroidType == AsteroidType.Ice && canSplit)
            {
                SplitIceAsteroid();
            }

            Destroy(collision.gameObject);
            Destroy(gameObject);
            return;
        }
    }

    void SplitIceAsteroid()
{
    // Prevent infinitely tiny fragments
    if (transform.localScale.x <= 0.5f)
        return;

    for (int i = 0; i < fragmentsToSpawn; i++)
    {
        GameObject fragment = Instantiate(iceFragmentPrefab, transform.position, Quaternion.identity);

        // Scale down
        fragment.transform.localScale = transform.localScale * fragmentScaleMultiplier;

        // Prevent further splitting
        if (asteroidType == AsteroidType.Ice);
            Obstacle fragObstacle = fragment.GetComponent<Obstacle>();
                if (fragObstacle != null)
                    {
                        fragObstacle.canSplit = false;
                    }
        
                // Apply physics
                Rigidbody2D fragRb = fragment.GetComponent<Rigidbody2D>();
                if (fragRb != null)
                    {
                        Vector2 randomDir = Random.insideUnitCircle.normalized;
                        fragRb.AddForce(randomDir * Random.Range(10f, 10f), ForceMode2D.Impulse);
                        fragRb.AddTorque(Random.Range(-5f, 5f), ForceMode2D.Impulse);
                    }
    }
}
}