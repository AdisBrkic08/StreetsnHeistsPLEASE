using UnityEngine;
using System.Collections;

public class CarHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Critical State")]
    public int criticalHealth = 20;
    public float explosionDelay = 2.5f;
    private bool isCritical = false;
    private bool isExploding = false;

    [Header("Effects")]
    public GameObject smokeEffectPrefab;
    public GameObject explosionPrefab;
    public AudioSource warningBeep;

    private GameObject smokeInstance;

    [Header("Explosion Settings")]
    public float explosionRadius = 2.5f;
    public int explosionDamage = 100;
    public float ejectOffset = 1f; // How far player is ejected

    void Start()
    {
        currentHealth = maxHealth;
        Debug.Log($"[CarHealth] Spawned with health: {currentHealth}");
    }

    // ==============================
    // DAMAGE
    // ==============================
    public void TakeDamage(int amount)
    {
        if (isExploding) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"[CarHealth] Health: {currentHealth}/{maxHealth}");

        UpdateSmoke();

        if (!isCritical && currentHealth <= criticalHealth)
        {
            EnterCriticalState();
        }
    }

    // ==============================
    // SMOKE HANDLING
    // ==============================
    void UpdateSmoke()
    {
        // Start smoking when below 50%
        if (currentHealth <= maxHealth * 0.5f && smokeInstance == null)
        {
            SpawnSmoke();
        }

        // Stop smoke if repaired
        if (currentHealth > maxHealth * 0.5f && smokeInstance != null)
        {
            RemoveSmoke();
        }
    }

    void SpawnSmoke()
    {
        if (smokeEffectPrefab == null) return;

        smokeInstance = Instantiate(
            smokeEffectPrefab,
            transform.position,
            Quaternion.identity,
            transform
        );
    }

    void RemoveSmoke()
    {
        if (smokeInstance != null)
        {
            Destroy(smokeInstance);
            smokeInstance = null;
        }
    }

    // ==============================
    // CRITICAL STATE
    // ==============================
    void EnterCriticalState()
    {
        isCritical = true;

        Debug.Log("[CarHealth] CRITICAL! Explosion countdown started.");

        // Force smoke if not already
        if (smokeInstance == null)
            SpawnSmoke();

        if (warningBeep != null)
            warningBeep.Play();

        StartCoroutine(ExplosionCountdown());
    }

    IEnumerator ExplosionCountdown()
    {
        yield return new WaitForSeconds(explosionDelay);
        Explode();
    }

    // ==============================
    // EXPLOSION
    // ==============================
    void Explode()
    {
        if (isExploding) return;

        isExploding = true;

        Debug.Log("[CarHealth] BOOM! Car exploded.");

        RemoveSmoke();

        // Spawn explosion effect
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // =========================
        // Step 1: Find player anywhere
        // =========================
        PlayerController2D player = GameObject.FindWithTag("Player")?.GetComponent<PlayerController2D>();
        if (player != null)
        {
            Debug.Log("[CarHealth] Player detected for ejection.");

            // Optional: Detach from car if parented
            player.transform.parent = null;

            // Move slightly outside car
            player.transform.position = transform.position + new Vector3(ejectOffset, 0f, 0f);

            // Apply explosion damage
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null)
            {
                Debug.Log("[CarHealth] Applying explosion damage to player.");
                health.TakeDamage(explosionDamage);
            }

            // Optional: add force to player
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 ejectDir = (Vector2.right + Vector2.up).normalized;
                rb.AddForce(ejectDir * 300f);
            }
        }

        // =========================
        // Step 2: Damage nearby objects
        // =========================
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<PlayerHealth>()?.TakeDamage(explosionDamage / 2);
            }

            if (hit.CompareTag("NPC"))
                hit.GetComponent<NPCHealth>()?.TakeDamage(explosionDamage);
        }

        Destroy(gameObject);
    }


    // ==============================
    // COLLISION DAMAGE
    // ==============================
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude > 5f)
        {
            int dmg = Mathf.RoundToInt(collision.relativeVelocity.magnitude * 5f);
            TakeDamage(dmg);
        }
    }

    // ==============================
    // REPAIR
    // ==============================
    public void FullRepair()
    {
        currentHealth = maxHealth;
        isCritical = false;
        isExploding = false;

        RemoveSmoke();

        if (warningBeep != null)
            warningBeep.Stop();

        Debug.Log("Car fully repaired");
    }
}
