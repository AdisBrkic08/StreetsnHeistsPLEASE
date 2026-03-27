using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TrafficSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject[] carPrefabs;
    public float spawnRadius = 20f;   // How close player must be to a path to spawn a car
    public float despawnRadius = 30f; // Distance at which cars disappear to save memory
    public float spawnInterval = 3f;  // Seconds between spawn attempts
    public int maxCars = 10;          // Global limit

    private Transform player;
    private List<Path> allPaths = new List<Path>();
    private List<GameObject> activeCars = new List<GameObject>();
    private float timer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        // Find all path scripts in the scene
        allPaths.AddRange(FindObjectsByType<Path>(FindObjectsSortMode.None));
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            CheckAndSpawn();
            timer = 0;
        }

        CleanupCars();
    }

    void CheckAndSpawn()
    {
        if (activeCars.Count >= maxCars) return;

        foreach (Path path in allPaths)
        {
            // Check if player is near the start of this path
            float dist = Vector3.Distance(player.position, path.waypoints[0].position);

            if (dist < spawnRadius)
            {
                SpawnCarOnPath(path);
                break; // Spawn one car at a time
            }
        }
    }

    void SpawnCarOnPath(Path targetPath)
    {
        GameObject carType = carPrefabs[Random.Range(0, carPrefabs.Length)];
        GameObject newCar = Instantiate(carType, targetPath.waypoints[0].position, Quaternion.identity);

        // Tell the RandomWalker which path to use
        RandomWalker walker = newCar.GetComponent<RandomWalker>();
        if (walker != null)
        {
            walker.externalPath = targetPath; // We'll update RandomWalker to accept this
        }

        activeCars.Add(newCar);
    }

    void CleanupCars()
    {
        for (int i = activeCars.Count - 1; i >= 0; i--)
        {
            if (activeCars[i] == null) { activeCars.RemoveAt(i); continue; }

            float dist = Vector3.Distance(player.position, activeCars[i].transform.position);
            if (dist > despawnRadius)
            {
                Destroy(activeCars[i]);
                activeCars.RemoveAt(i);
            }
        }
    }
}