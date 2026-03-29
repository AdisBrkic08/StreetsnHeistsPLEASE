using UnityEngine;
using System.Collections;

public class NPCHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 50;
    private int currentHealth;

    [Header("Spawn Protection")]
    private float spawnTimestamp;
    public float gracePeriod = 1.0f;

    [Header("Effects")]
    public GameObject bloodEffectPrefab;
    public GameObject deathBloodEffectPrefab;

    [Header("Drops")]
    public GameObject moneyPrefab;

    void Start()
    {
        currentHealth = maxHealth;
        spawnTimestamp = Time.time;

        // Subscribe to the Wasted event so this NPC despawns when player dies
        // Assuming your WastedSystem has a static event or you call a Global Cleanup
        WastedSystem.OnPlayerWasted += DespawnOnWasted;
    }

    public void TakeDamage(int damage)
    {
        // 🛡️ Spawn Protection
        if (Time.time < spawnTimestamp + gracePeriod) return;

        currentHealth -= damage;

        // 🩸 Blood Burst on Hit
        if (bloodEffectPrefab != null)
        {
            GameObject blood = Instantiate(bloodEffectPrefab, transform.position, Quaternion.identity);
            FreezeBlood(blood);
        }

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        // 🩸 Big Blood Burst on Death
        if (deathBloodEffectPrefab != null)
        {
            GameObject deathBlood = Instantiate(deathBloodEffectPrefab, transform.position, Quaternion.identity);
            FreezeBlood(deathBlood);
        }

        // Drop money
        if (moneyPrefab != null)
            Instantiate(moneyPrefab, transform.position, Quaternion.identity);

        // Report to HeatManager
        if (HeatManager.Instance != null)
            HeatManager.Instance.ReportCrime(100f);

        CleanupAndDestroy();
    }

    void DespawnOnWasted()
    {
        // Simple despawn without triggering death effects or crime reports
        CleanupAndDestroy();
    }

    void CleanupAndDestroy()
    {
        WastedSystem.OnPlayerWasted -= DespawnOnWasted; // Unsubscribe to prevent memory leaks
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        WastedSystem.OnPlayerWasted -= DespawnOnWasted;
    }

    // --- BLOOD FREEZE LOGIC ---
    void FreezeBlood(GameObject blood)
    {
        ParticleSystem ps = blood.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            StartCoroutine(FreezeBloodRoutine(ps));
        }
        Destroy(blood, 5f); // Cleanup the stain after 5 seconds
    }

    IEnumerator FreezeBloodRoutine(ParticleSystem ps)
    {
        yield return new WaitForSeconds(0.5f); // Spray for half a second
        if (ps != null)
        {
            var main = ps.main;
            main.simulationSpeed = 0f; // Freeze into a "stain"
        }
    }
}