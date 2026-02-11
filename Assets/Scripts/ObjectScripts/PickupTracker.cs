using UnityEngine;

public class PickupTracker : MonoBehaviour
{
    public MoneySpawner spawner;

    void OnDestroy()
    {
        if (spawner != null)
            spawner.OnPickupDestroyed();
    }
}
