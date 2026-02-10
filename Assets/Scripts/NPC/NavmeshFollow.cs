using UnityEngine;
using UnityEngine.AI;

public class NavmeshFollow : MonoBehaviour
{
    [SerializeField] Transform target;

    [Header("Shooting")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] float fireRate = 1f; // bullets per second
    [SerializeField] float bulletSpeed = 10f;

    private float fireCooldown;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        // Find player if missing
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
            else
                return; // stop if still null
        }

        // Move toward player
        agent.SetDestination(target.position);

        // Handle shooting cooldown
        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0f)
        {
            ShootAtPlayer();
            fireCooldown = 1f / fireRate;
        }
    }

    void ShootAtPlayer()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // Direction from enemy to player
        Vector2 direction = (target.position - firePoint.position).normalized;

        // Create bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // Give velocity
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = direction * bulletSpeed;
    }
}
