using UnityEngine;

public class NPCHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 50;
    private int currentHealth;

    [Header("Effects")]
    public GameObject bloodEffectPrefab;
    public GameObject deathBloodEffectPrefab;

    [Header("Drops")]
    public GameObject moneyPrefab;
    public int moneyAmount = 50;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // Small blood burst when hit
        if (bloodEffectPrefab != null)
            FreezeBlood(Instantiate(bloodEffectPrefab, transform.position, Quaternion.identity));

        if (currentHealth <= 0)
            Die();

    }

    void Die()
    {
        // Big blood burst
        if (deathBloodEffectPrefab != null)
            FreezeBlood(Instantiate(deathBloodEffectPrefab, transform.position, Quaternion.identity));

        // Drop money
        if (moneyPrefab != null)
        {
            Instantiate(moneyPrefab, transform.position, Quaternion.identity);
        }

        TargetLockOn lockOn = FindFirstObjectByType<TargetLockOn>();
        if (lockOn != null && lockOn.GetCurrentTarget() == transform)
        {
            lockOn.Unlock();
        }


        Destroy(gameObject);
    }

    void FreezeBlood(GameObject blood)
    {
        // 🩸 Stop particle motion after a few seconds (leaves stain)
        ParticleSystem ps = blood.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            StartCoroutine(FreezeBloodRoutine(ps));
        }
        Destroy(blood, 5f); // cleanup eventually
    }

    System.Collections.IEnumerator FreezeBloodRoutine(ParticleSystem ps)
    {
        yield return new WaitForSeconds(1f); // let it spray for a bit

        var main = ps.main;
        main.simulationSpeed = 0f; // freeze in place (like stain)
    }
}
