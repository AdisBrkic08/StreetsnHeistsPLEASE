using System.Collections;
using UnityEngine;

public class PoliceSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] spawners;
    [SerializeField] private GameObject policePrefab;

    [Header("Spawn Timing")]
    [SerializeField] private float baseSpawnTime = 6f;

    [Header("Dynamic Limits")]
    [SerializeField] private int policePerStar = 2; // Each star adds 2 to the limit
    [SerializeField] private int absoluteMaxPolice = 15; // Hard cap for performance

    private bool spawnDebounce = false;

    void Update()
    {
        if (HeatManager.Instance == null) return;

        int currentStars = HeatManager.Instance.heatLevel;
        if (currentStars <= 0) return;

        // --- DYNAMIC MAX POLICE CALCULATION ---
        // 1 Star = 2 Cops, 2 Stars = 4 Cops, etc.
        int currentMaxPolice = Mathf.Min(currentStars * policePerStar, absoluteMaxPolice);

        // Count current police in the scene
        int policeCount = GameObject.FindGameObjectsWithTag("PoliceVehicle").Length;

        if (spawnDebounce || policeCount >= currentMaxPolice) { return; }

        // Higher stars still spawn faster too!
        float dynamicSpawnTime = baseSpawnTime / currentStars;

        StartCoroutine(SpawnCop(dynamicSpawnTime));
    }

    IEnumerator SpawnCop(float waitTime)
    {
        spawnDebounce = true;

        if (spawners.Length > 0 && policePrefab != null)
        {
            int index = UnityEngine.Random.Range(0, spawners.Length);
            Instantiate(policePrefab, spawners[index].transform.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(waitTime);
        spawnDebounce = false;
    }
}