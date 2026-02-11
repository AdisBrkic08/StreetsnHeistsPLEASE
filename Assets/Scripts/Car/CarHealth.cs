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

        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 2.5f);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
                hit.GetComponent<PlayerHealth>()?.TakeDamage(100);

            if (hit.CompareTag("NPC"))
                hit.GetComponent<NPCHealth>()?.TakeDamage(100);
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
