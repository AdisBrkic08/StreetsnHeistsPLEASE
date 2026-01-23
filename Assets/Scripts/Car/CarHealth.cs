using UnityEngine;
using System.Collections;

public class CarHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 1000f;
    public float currentHealth;

    [Header("Damage Settings")]
    public float collisionDamageMultiplier = 5f;
    public float minDamageSpeed = 3f;

    [Header("Explosion")]
    public GameObject explosionPrefab;
    public float explosionRadius = 3f;
    public float explosionForce = 12f;
    public int explosionDamage = 100;

    [Header("Fire Effects")]
    public GameObject firePrefab;   // optional
    public float fireHealthThreshold = 30f;

    private bool isDestroyed = false;
    private Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
    }

    // 💥 Collision damage
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDestroyed) return;

        float impactSpeed = collision.relativeVelocity.magnitude;

        if (impactSpeed < minDamageSpeed) return;

        float damage = impactSpeed * collisionDamageMultiplier;
        TakeDamage(damage);
    }

    // 🔥 External damage (bullets, bombs, etc)
    public void TakeDamage(float amount)
    {
        if (isDestroyed) return;

        currentHealth -= amount;
        Debug.Log("🚗 Car Health: " + currentHealth);

        if (currentHealth <= fireHealthThreshold && firePrefab != null)
        {
            if (!IsInvoking(nameof(SpawnFire)))
                SpawnFire();
        }

        if (currentHealth <= 0)
        {
            Explode();
        }
    }

    void SpawnFire()
    {
        Instantiate(firePrefab, transform.position, Quaternion.identity, transform);
    }

    void Explode()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        Debug.Log("💥 CAR EXPLODED");

        // Explosion VFX
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // Explosion force + damage
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in hits)
        {
            // Damage NPCs
            NPCHealth npc = hit.GetComponent<NPCHealth>();
            if (npc != null)
            {
                npc.TakeDamage(explosionDamage);
            }

            // Damage player
            PlayerHealth player = hit.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(explosionDamage);
            }

            // Physics push
            Rigidbody2D rb2 = hit.GetComponent<Rigidbody2D>();
            if (rb2 != null)
            {
                Vector2 dir = (rb2.position - (Vector2)transform.position).normalized;
                rb2.AddForce(dir * explosionForce, ForceMode2D.Impulse);
            }
        }

        // Disable car
        GetComponent<Collider2D>().enabled = false;
        if (rb != null) rb.simulated = false;

        // Optional: destroy after delay
        Destroy(gameObject, 2f);
    }

    // Debug visual
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
