using System.Collections;
using UnityEngine;

public class PoliceSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] spawners;
    [SerializeField] private GameObject policePrefab;
    [SerializeField] private float baseSpawnTime = 5f;
    [SerializeField] private int maxPolice = 5;
    [SerializeField] private string policeTag = "PoliceVehicle"; // Ensure this matches your prefab tag

    private bool spawnDebounce = false;

    void Update()
    {
        // 1. Check HeatManager
        if (HeatManager.Instance == null) return;

        int currentStars = HeatManager.Instance.heatLevel;
        if (currentStars <= 0) return;

        // 2. Check Police Count
        GameObject[] existingPolice = GameObject.FindGameObjectsWithTag(policeTag);
        int policeCount = existingPolice.Length;

        // 3. Logic Check
        if (spawnDebounce) return;

        if (policeCount >= maxPolice)
        {
            // Debug.Log("Max police reached: " + policeCount); // Optional spammy log
            return;
        }

        // 4. Start Spawn
        float dynamicSpawnTime = baseSpawnTime / currentStars;
        StartCoroutine(SpawnCop(dynamicSpawnTime));
    }

    IEnumerator SpawnCop(float waitTime)
    {
        spawnDebounce = true;

        if (spawners.Length == 0)
        {
            Debug.LogError("🚨 SPAWNER ERROR: You haven't assigned any Spawner objects in the Inspector!");
            yield break;
        }

        if (policePrefab == null)
        {
            Debug.LogError("🚨 SPAWNER ERROR: Police Prefab is missing in the Inspector!");
            yield break;
        }

        // Pick a spawner and spawn
        int index = UnityEngine.Random.Range(0, spawners.Length);
        GameObject newPolice = Instantiate(policePrefab, spawners[index].transform.position, Quaternion.identity);

        Debug.Log("🚓 POLICE SPAWNED at: " + spawners[index].name);

        yield return new WaitForSeconds(waitTime);
        spawnDebounce = false;
    }
}