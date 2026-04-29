using UnityEngine;
using System.Collections;

public class CarHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Critical State")]
    public int criticalHealth = 100;
    public float explosionDelay = 2.5f;
    private bool isCritical = false;
    private bool isExploding = false;

    [Header("Visual Thresholds")]
    public int fireHealthThreshold = 35; // Fire starts here, but countdown doesn't start yet

    [Header("Effects")]
    public GameObject smokeEffectPrefab;
    public GameObject fireEffectPrefab; // NEW: The fire that appears at low health
    public GameObject explosionPrefab;
    public AudioSource warningBeep;

    private GameObject smokeInstance;
    private GameObject fireInstance; // NEW: Track the fire object

    [Header("Explosion Settings")]
    public float explosionRadius = 5f;
    public int explosionDamage = 100;

    void Start()
    {
        currentHealth = maxHealth;
        Debug.Log($"[CarHealth] {gameObject.name} initialized with {currentHealth} HP.");
    }

    public void TakeDamage(int amount)
    {
        if (isExploding) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateVisualEffects();

        if (!isCritical && currentHealth <= criticalHealth)
        {
            EnterCriticalState();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isExploding) return;

        float impactForce = collision.relativeVelocity.magnitude;

        if (impactForce > 4f)
        {
            int damageToApply = Mathf.RoundToInt(impactForce * 4f);
            TakeDamage(damageToApply);
        }
    }

    // ==============================
    // VISUAL EFFECTS (Smoke & Fire)
    // ==============================
    void UpdateVisualEffects()
    {
        // Smoke Logic (50% health)
        if (currentHealth <= maxHealth * 0.5f && smokeInstance == null)
            smokeInstance = Instantiate(smokeEffectPrefab, transform.position, Quaternion.identity, transform);

        if (currentHealth > maxHealth * 0.5f && smokeInstance != null)
        {
            Destroy(smokeInstance);
            smokeInstance = null;
        }

        // Fire Logic (Critical Health)
        if (currentHealth <= criticalHealth && fireInstance == null)
            SpawnFire();

        if (currentHealth > criticalHealth && fireInstance != null)
            RemoveFire();
    }

    void SpawnFire()
    {
        if (fireEffectPrefab == null) return;

        // We calculate a position slightly in front of the car (Z = -1)
        Vector3 spawnPos = transform.position;
        spawnPos.z = -1f;

        fireInstance = Instantiate(fireEffectPrefab, spawnPos, Quaternion.identity, transform);

        // Safety: Ensure the particle system actually starts playing
        ParticleSystem ps = fireInstance.GetComponent<ParticleSystem>();
        if (ps != null) ps.Play();

        Debug.Log("Fire Spawned at: " + fireInstance.transform.position);
    }

    void RemoveFire()
    {
        if (fireInstance != null) { Destroy(fireInstance); fireInstance = null; }
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

        // Ensure fire spawns immediately on entering critical
        if (fireInstance == null) SpawnFire();

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

        CarInteraction interaction = GetComponent<CarInteraction>();
        if (interaction != null && interaction.isPlayerDriving)
        {
            interaction.ForceExit();
        }

        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Collider2D[] objectsInBlast = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in objectsInBlast)
        {
            if (hit.gameObject == gameObject) continue;

            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<PlayerHealth>()?.TakeDamage(explosionDamage);
            }
            else if (hit.GetComponent<CarHealth>() != null)
            {
                hit.GetComponent<CarHealth>().TakeDamage(explosionDamage / 2);
            }
        }

        Destroy(gameObject);
    }

    public void FullRepair()
    {
        currentHealth = maxHealth;
        isCritical = false;
        isExploding = false;
        RemoveSmoke();
        RemoveFire(); // Clean up fire on repair
        if (warningBeep != null) warningBeep.Stop();
        Debug.Log("[CarHealth] Repaired to Full.");
    }
}