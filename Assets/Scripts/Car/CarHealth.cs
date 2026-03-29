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
    public float explosionRadius = 5f;
    public int explosionDamage = 100;

    void Start()
    {
        currentHealth = maxHealth;
        // Debug to verify health starts correctly
        Debug.Log($"[CarHealth] {gameObject.name} initialized with {currentHealth} HP.");
    }

    // ==============================
    // DAMAGE CORE
    // ==============================
    public void TakeDamage(int amount)
    {
        if (isExploding) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"[CarHealth] {gameObject.name} took {amount} damage. Remaining: {currentHealth}");

        UpdateSmoke();

        if (!isCritical && currentHealth <= criticalHealth)
        {
            EnterCriticalState();
        }
    }

    // ==============================
    // PHYSICS COLLISION (The Damage Trigger)
    // ==============================
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Don't take damage if we are already blowing up
        if (isExploding) return;

        // Calculate how hard the hit was
        float impactForce = collision.relativeVelocity.magnitude;

        // Only register a "crash" if moving faster than 4 units
        if (impactForce > 4f)
        {
            // Formula: (Force * Multiplier) - small buffer
            int damageToApply = Mathf.RoundToInt(impactForce * 4f);

            TakeDamage(damageToApply);
        }
    }

    // ==============================
    // SMOKE & VISUALS
    // ==============================
    void UpdateSmoke()
    {
        if (currentHealth <= maxHealth * 0.5f && smokeInstance == null) SpawnSmoke();
        if (currentHealth > maxHealth * 0.5f && smokeInstance != null) RemoveSmoke();
    }

    void SpawnSmoke()
    {
        if (smokeEffectPrefab == null) return;
        smokeInstance = Instantiate(smokeEffectPrefab, transform.position, Quaternion.identity, transform);
    }

    void RemoveSmoke()
    {
        if (smokeInstance != null) { Destroy(smokeInstance); smokeInstance = null; }
    }

    // ==============================
    // EXPLOSION LOGIC
    // ==============================
    void EnterCriticalState()
    {
        if (isCritical) return;
        isCritical = true;

        if (warningBeep != null) warningBeep.Play();

        Debug.Log("[CarHealth] CRITICAL STATE: Explosion in " + explosionDelay + " seconds!");
        StartCoroutine(ExplosionCountdown());
    }

    IEnumerator ExplosionCountdown()
    {
        yield return new WaitForSeconds(explosionDelay);
        Explode();
    }

    public void Explode()
    {
        if (isExploding) return;
        isExploding = true;

        // 1. Force the player out so they don't get deleted with the car
        CarInteraction interaction = GetComponent<CarInteraction>();
        if (interaction != null && interaction.isPlayerDriving)
        {
            interaction.ForceExit();
        }

        // 2. Visuals
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // 3. Area Damage
        Collider2D[] objectsInBlast = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in objectsInBlast)
        {
            if (hit.gameObject == gameObject) continue;

            // Damage Player
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<PlayerHealth>()?.TakeDamage(explosionDamage);
            }
            // Damage other cars (Chain reaction!)
            else if (hit.GetComponent<CarHealth>() != null)
            {
                hit.GetComponent<CarHealth>().TakeDamage(explosionDamage / 2);
            }
        }

        // 4. Remove the car from the game
        Destroy(gameObject);
    }

    public void FullRepair()
    {
        currentHealth = maxHealth;
        isCritical = false;
        isExploding = false;
        RemoveSmoke();
        if (warningBeep != null) warningBeep.Stop();
        Debug.Log("[CarHealth] Repaired to Full.");
    }
}