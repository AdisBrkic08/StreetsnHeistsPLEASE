using UnityEngine;
using System.Collections.Generic;

public class TrafficSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] carPrefabs;

    [Header("Spawn Settings")]
    public float spawnRadius = 25f;
    public float despawnRadius = 45f;
    public float spawnInterval = 1.5f;
    public int maxCars = 15;

    private Transform player;
    private List<Path> allPaths = new List<Path>();
    private List<GameObject> activeCars = new List<GameObject>();
    private float timer;

    struct WaypointData { public Path path; public int index; }

    void Start()
    {
        FindPlayer();
        // Find all paths in the scene
        allPaths.AddRange(Object.FindObjectsByType<Path>(FindObjectsSortMode.None));

        if (allPaths.Count == 0) Debug.LogError("TrafficSpawner: No Path scripts found in scene!");
    }

    void FindPlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (player == null)
        {
            FindPlayer(); // Try to find player again if they were missing
            return;
        }

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

        List<WaypointData> nearbyWaypoints = new List<WaypointData>();

        foreach (Path path in allPaths)
        {
            if (path.waypoints == null) continue;

            for (int i = 0; i < path.waypoints.Count; i++)
            {
                float dist = Vector3.Distance(player.position, path.waypoints[i].position);
                // We want to spawn them slightly away from the player so they don't pop in
                if (dist < spawnRadius && dist > 10f)
                {
                    nearbyWaypoints.Add(new WaypointData { path = path, index = i });
                }
            }
        }

        if (nearbyWaypoints.Count > 0)
        {
            WaypointData selected = nearbyWaypoints[Random.Range(0, nearbyWaypoints.Count)];
            SpawnCarAtWaypoint(selected);
        }
    }

    void SpawnCarAtWaypoint(WaypointData data)
    {
        if (carPrefabs.Length == 0) return;

        GameObject carType = carPrefabs[Random.Range(0, carPrefabs.Length)];
        Vector3 spawnPos = data.path.waypoints[data.index].position;

        GameObject newCar = Instantiate(carType, spawnPos, Quaternion.identity);

        RandomWalker walker = newCar.GetComponent<RandomWalker>();
        if (walker != null)
        {
            walker.externalPath = data.path;
            walker.SetStartingWaypoint(data.index);
        }

        activeCars.Add(newCar);
    }

    void CleanupCars()
    {
        for (int i = activeCars.Count - 1; i >= 0; i--)
        {
            if (activeCars[i] == null) { activeCars.RemoveAt(i); continue; }

            if (Vector3.Distance(player.position, activeCars[i].transform.position) > despawnRadius)
            {
                Destroy(activeCars[i]);
                activeCars.RemoveAt(i);
            }
        }
    }
}