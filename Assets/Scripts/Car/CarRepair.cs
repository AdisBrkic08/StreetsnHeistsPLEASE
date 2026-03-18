using UnityEngine;

public class CarRepairZone : MonoBehaviour
{
    [Header("Repair Settings")]
    public float minSpeed = 8f;
    public float cooldown = 5f;
    public int repairCost = 50;

    float lastRepairTime = -999f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("PlayerVehicle")) return;

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        CarHealth carHealth = other.GetComponent<CarHealth>();

        if (rb == null || carHealth == null) return;

        float speed = rb.linearVelocity.magnitude;

        if (speed < minSpeed) return;

        if (Time.time < lastRepairTime + cooldown) return;

        // ✅ Use MoneyManager instead
        if (MoneyManager.Instance == null)
        {
            Debug.LogWarning("MoneyManager not found!");
            return;
        }

        if (!MoneyManager.Instance.SpendMoney(repairCost))
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