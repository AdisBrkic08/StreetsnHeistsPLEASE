using UnityEngine;

public class CarEnterTrigger : MonoBehaviour
{
    private CarInteraction car;

    void Awake()
    {
        car = GetComponentInParent<CarInteraction>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        car.playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        car.playerInRange = false;
    }
}
