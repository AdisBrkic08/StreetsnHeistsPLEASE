using UnityEngine;

public class MoneyPickup : MonoBehaviour
{
    public int value = 50;
    public float lifetime = 10f;
    private bool collected = false;

    void Start()
    {
        Destroy(gameObject, lifetime); // auto-despawn if not picked up
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            var playerMoney = other.GetComponent<PlayerMoney>();
            if (playerMoney != null)
            {
                playerMoney.AddMoney(value);
                collected = true;
                Destroy(gameObject);
            }
        }
    }
}
