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
    public float explosionRadius = 5f; // Increased for realism
    public int explosionDamage = 100;
    public float ejectForce = 500f;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (isExploding) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateSmoke();

        if (!isCritical && currentHealth <= criticalHealth)
        {
            EnterCriticalState();
        }
    }

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

    void EnterCriticalState()
    {
        if (isCritical) return;
        isCritical = true;

        if (smokeInstance == null) SpawnSmoke();
        if (warningBeep != null) warningBeep.Play();

        StartCoroutine(ExplosionCountdown());
    }

    IEnumerator ExplosionCountdown()
    {
        yield return new WaitForSeconds(explosionDelay);
        Explode();
    }

    void Explode()
    {
        if (isExploding) return;
        isExploding = true;

        RemoveSmoke();

        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // --- FIXED EJECTION & DAMAGE LOGIC ---

        // Find ALL objects in blast radius
        Collider2D[] objectsInBlast = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hit in objectsInBlast)
        {
            // Don't damage the car itself
            if (hit.gameObject == gameObject) continue;

            // 1. Handle Player
            if (hit.CompareTag("Player"))
            {
                // Unparent immediately so they aren't destroyed with the car
                hit.transform.SetParent(null);

                // Apply Damage ONCE
                PlayerHealth pHealth = hit.GetComponent<PlayerHealth>();
                if (pHealth != null) pHealth.TakeDamage(explosionDamage);

                // Push player away from blast center (The "Eject")
                Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 blastDir = (hit.transform.position - transform.position).normalized;
                    // Add an upward "pop" for 2D feel
                    rb.AddForce((blastDir + Vector2.up * 0.5f) * ejectForce);
                }
            }

            // 2. Handle NPCs
            else if (hit.CompareTag("NPC"))
            {
                // Use your NPC health script name here
                // hit.GetComponent<NPCHealth>()?.TakeDamage(explosionDamage);
            }

            // 3. Chain Reaction (Other Cars)
            else if (hit.CompareTag("PoliceVehicle") || hit.gameObject.GetComponent<CarHealth>())
            {
                if (hit.gameObject != gameObject)
                    hit.GetComponent<CarHealth>()?.TakeDamage(explosionDamage);
            }
        }

        // Final safety: Tell the driving script the player is no longer inside
        // (Prevents the "dragged back" glitch)
        PlayerDriving driving = FindFirstObjectByType<PlayerDriving>();
        if (driving != null && driving.isDriving)
        {
            // driving.ForceExit(); // Call your exit function here if you have one
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude > 6f)
        {
            int dmg = Mathf.RoundToInt(collision.relativeVelocity.magnitude * 4f);
            TakeDamage(dmg);
        }
    }

    // Drawing the radius in editor so you can see how big the BOOM is
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
    // ==============================
    // REPAIR (Fixes CS1061 Error)
    // ==============================
    public void FullRepair()
    {
        currentHealth = maxHealth;
        isCritical = false;
        isExploding = false;

        // Clean up the visuals
        RemoveSmoke();

        // Stop the "about to blow up" sound
        if (warningBeep != null)
            warningBeep.Stop();

        Debug.Log("Car fully repaired via FullRepair()");
    }
}