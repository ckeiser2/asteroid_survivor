using UnityEngine;

public class OrbitAroundPlayer : MonoBehaviour
{
    public Transform player;
    public float orbitRadius = 1.5f;
    public float orbitSpeed = 100f;
    public float angleOffset = 0f;

    void Update()
    {
        if (player == null) return;

        float angle = Time.time * orbitSpeed + angleOffset;

        float x = Mathf.Cos(angle) * orbitRadius;
        float y = Mathf.Sin(angle) * orbitRadius;

        transform.position = player.position + new Vector3(x, y, 0);
    }
}