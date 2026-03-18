using UnityEngine;

public class HealthSpawner : MonoBehaviour
{
    public GameObject healthPickupPrefab;

    public int maxPickups = 1; // 👈 only one at a time
    public float spawnInterval = 5f;

    public Vector2 minSpawnPosition;
    public Vector2 maxSpawnPosition;

    private int currentPickups;

    void Start()
    {
        InvokeRepeating(nameof(SpawnPickup), 0f, spawnInterval);
    }

    void SpawnPickup()
    {
        if (currentPickups >= maxPickups)
            return;

        Vector2 spawnPos = new Vector2(
            Random.Range(minSpawnPosition.x, maxSpawnPosition.x),
            Random.Range(minSpawnPosition.y, maxSpawnPosition.y)
        );

        GameObject pickup = Instantiate(
            healthPickupPrefab,
            spawnPos,
            Quaternion.identity
        );

        currentPickups++;

        pickup.AddComponent<HealthTracker>().spawner = this;
    }

    public void OnPickupDestroyed()
    {
        currentPickups--;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Vector3 center = (minSpawnPosition + maxSpawnPosition) / 2;
        Vector3 size = maxSpawnPosition - minSpawnPosition;

        Gizmos.DrawWireCube(center, size);
    }
}