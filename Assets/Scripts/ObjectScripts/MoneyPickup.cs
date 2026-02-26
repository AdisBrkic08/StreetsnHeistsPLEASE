using UnityEngine;

public class MoneyPickup : MonoBehaviour
{
    public int value = 50;
    public float lifetime = 10f;

    private bool collected = false;

    void Start()
    {
        // Auto destroy if not collected
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            // Add money directly to global manager
            if (MoneyManager.Instance != null)
            {
                MoneyManager.Instance.AddMoney(value);
            }

            collected = true;
            Destroy(gameObject);
        }
    }
}