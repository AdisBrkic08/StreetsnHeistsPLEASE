using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healAmount = 50;
    public float lifetime = 15f;

    private bool collected = false;

    void Start()
    {
        Destroy(gameObject, lifetime); // auto despawn
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();

            if (health != null)
            {
                health.currentHealth = Mathf.Clamp(
                    health.currentHealth + healAmount,
                    0,
                    health.maxHealth
                );

                collected = true;
                Destroy(gameObject);
            }
        }
    }
}