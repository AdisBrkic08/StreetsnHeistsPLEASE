using UnityEngine;

public class DamageObject : MonoBehaviour
{
    public int damageAmount = 10;
    public float damageInterval = 1f;   // damage every X seconds
    public bool continuousDamage = false;

    private float nextDamageTime = 0f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!continuousDamage)
        {
            if (other.CompareTag("Player"))
            {
                PlayerHealth ph = other.GetComponent<PlayerHealth>();
                if (ph != null)
                    ph.TakeDamage(damageAmount);
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (continuousDamage && other.CompareTag("Player"))
        {
            if (Time.time >= nextDamageTime)
            {
                PlayerHealth ph = other.GetComponent<PlayerHealth>();
                if (ph != null)
                    ph.TakeDamage(damageAmount);

                nextDamageTime = Time.time + damageInterval;
            }
        }
    }
}
