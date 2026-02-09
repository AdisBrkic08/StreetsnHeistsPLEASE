using UnityEngine;

public class CarRepairZone : MonoBehaviour
{
    [Header("Repair Settings")]
    public float minSpeed = 8f;
    public float cooldown = 5f;
    public int repairCost = 50;

    float lastRepairTime = -999f;

    PlayerMoney playerMoney;

    void Start()
    {
        playerMoney = FindFirstObjectByType<PlayerMoney>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("PlayerVehicle")) return;

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        CarHealth carHealth = other.GetComponent<CarHealth>();

        if (rb == null || carHealth == null) return;


        float speed = rb.linearVelocity.magnitude;

        if (speed < minSpeed) return;

        if (Time.time < lastRepairTime + cooldown) return;

        // ❗ Check money
        if (playerMoney == null)
        {
            Debug.LogWarning("No PlayerMoney found!");
            return;
        }

        if (!playerMoney.SpendMoney(repairCost))
        {
            Debug.Log("Not enough money for repair!");
            return;
        }

        // ✅ Repair
        Repair(carHealth);
    }

    void Repair(CarHealth carHealth)
    {
        carHealth.FullRepair();

        lastRepairTime = Time.time;

        Debug.Log("Car repaired for $" + repairCost);
    }
}
