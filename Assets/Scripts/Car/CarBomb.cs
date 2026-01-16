using UnityEngine;
using System.Collections;

public class CarBomb : MonoBehaviour
{
    [Header("Bomb Settings")]
    public bool bombInstalled = false;
    public bool bombArmed = false;

    public float detonationDelay = 3f;
    public float explosionRadius = 5f;
    public float explosionForce = 12f;
    public int damage = 100;

    [Header("Effects")]
    public GameObject explosionPrefab;

    bool detonating = false;

    public void InstallBomb()
    {
        bombInstalled = true;
        bombArmed = false;
        Debug.Log("💣 Bomb installed on car");
    }

    public void ArmBomb()
    {
        if (!bombInstalled) return;

        bombArmed = true;
        Debug.Log("💣 Bomb armed");
    }

    public void StartDetonation()
    {
        if (!bombInstalled || !bombArmed || detonating) return;
        detonating = true;
        StartCoroutine(DetonationRoutine());
    }

    IEnumerator DetonationRoutine()
    {
        yield return new WaitForSeconds(detonationDelay);
        Explode();
    }

    void Explode()
    {
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
}
