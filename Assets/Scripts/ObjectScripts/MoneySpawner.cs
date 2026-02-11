using UnityEngine;

public class MoneySpawner : MonoBehaviour
{
    public GameObject moneyPickupPrefab;

    public int maxPickups = 10;
    public float spawnInterval = 3f;

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
            moneyPickupPrefab,
            spawnPos,
            Quaternion.identity
        );

        currentPickups++;

        pickup.AddComponent<PickupTracker>().spawner = this;
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
