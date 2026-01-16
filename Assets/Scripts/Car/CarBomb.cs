using UnityEngine;
using System.Collections;

public class CarBomb : MonoBehaviour
{
    [Header("Bomb Settings")]
    public bool bombInstalled = false;
    public bool bombArmed = false;

    public KeyCode detonateKey = KeyCode.LeftAlt;

    public float detonationDelay = 3f;
    public float explosionRadius = 5f;
    public float explosionForce = 12f;
    public int damage = 100;

    [Header("Effects")]
    public GameObject explosionPrefab;

    bool detonating = false;
    float countdownTimer;

    void Update()
    {
        // 🔹 Manual detonation while driving
        if (!Application.isPlaying) return;
        if (!bombInstalled || !bombArmed || detonating) return;

        if (Input.GetKeyDown(detonateKey))
        {
            StartDetonation();
        }
    }

    // Called from car spray
    public void InstallBomb()
    {
        bombInstalled = true;
        bombArmed = false;
        detonating = false;
        Debug.Log("💣 Bomb installed on car");
    }

    // Called when entering car
    public void ArmBomb()
    {
        if (!bombInstalled) return;

        bombArmed = true;
        Debug.Log("💣 Bomb armed (Left Alt to detonate)");
    }

    void StartDetonation()
    {
        if (detonating) return;

        detonating = true;
        countdownTimer = detonationDelay;
        StartCoroutine(DetonationRoutine());
    }

    IEnumerator DetonationRoutine()
    {
        while (countdownTimer > 0f)
        {
            Debug.Log($"💣 Detonating in {countdownTimer:F1}...");
            yield return new WaitForSeconds(1f);
            countdownTimer -= 1f;
        }

        Explode();
    }

    void Explode()
    {
        Debug.Log("💥 BOOM!");

        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hit in hits)
        {
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 dir = (rb.position - (Vector2)transform.position).normalized;
                rb.AddForce(dir * explosionForce, ForceMode2D.Impulse);
            }

            if (hit.TryGetComponent(out PlayerHealth player))
                player.TakeDamage(damage);

            if (hit.TryGetComponent(out NPCHealth npc))
                npc.TakeDamage(damage);
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
