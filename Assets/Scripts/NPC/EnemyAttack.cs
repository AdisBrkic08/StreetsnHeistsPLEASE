using System.Collections;
using UnityEngine;
using UnityEngine.AI; // Required for Anti-Hugging

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float fireRate = 0.6f;
    [SerializeField] private float range = 12f;
    [SerializeField] private float stopDistance = 4.5f; // ANTI-HUGGING DISTANCE

    private Transform player;
    private bool isFiring = false;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Essential for 2D NavMesh
        if (agent != null)
        {
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.stoppingDistance = stopDistance;
        }

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        // Movement Logic (Chase but stay back)
        if (agent != null)
        {
            agent.SetDestination(player.position);
        }

        // Shooting Logic
        if (dist < range && !isFiring)
        {
            StartCoroutine(FireBulletRoutine());
        }
    }

    IEnumerator FireBulletRoutine()
    {
        isFiring = true;

        Vector2 dir = (player.position - transform.position).normalized;

        // SPAWN OFFSET: Spawns the bullet slightly in front of the cop 
        // so it doesn't touch the cop's own collider and kill him.
        Vector3 spawnOffset = (Vector3)dir * 0.8f;
        GameObject bullet = Instantiate(bulletPrefab, transform.position + spawnOffset, Quaternion.identity);

        // --- THE OWNER FIX ---
        DamageObject dmgScript = bullet.GetComponent<DamageObject>();
        if (dmgScript != null) dmgScript.owner = gameObject;

        // Rotate bullet to face player
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle - 90);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = dir * bulletSpeed;

        yield return new WaitForSeconds(fireRate);
        isFiring = false;
    }
}