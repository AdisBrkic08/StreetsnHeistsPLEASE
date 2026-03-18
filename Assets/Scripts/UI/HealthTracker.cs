using UnityEngine;

public class HealthTracker : MonoBehaviour
{
    public HealthSpawner spawner;

    void OnDestroy()
    {
        if (spawner != null)
            spawner.OnPickupDestroyed();
    }
}