using UnityEngine;
using System.Collections.Generic;

public class WantedManager : MonoBehaviour
{
    public static WantedManager Instance;

    [Header("Wanted Stats")]
    public int wantedLevel = 0; // 0 to 5 stars
    public int crimesCommitted = 0;
    public int crimesNeededForNextStar = 2;

    [Header("Police Spawning")]
    public GameObject policePrefab;
    public float spawnDistance = 30f;
    public float spawnInterval = 5f;
    private float spawnTimer;

    private Transform player;
    private List<Path> allPaths = new List<Path>();

    void Awake() { Instance = this; }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        allPaths.AddRange(FindObjectsByType<Path>(FindObjectsSortMode.None));
    }

    void Update()
    {
        if (wantedLevel <= 0) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= (spawnInterval / wantedLevel)) // Faster spawns at higher stars
        {
            SpawnPoliceNearPlayer();
            spawnTimer = 0;
        }
    }

    public void ReportCrime(int severity)
    {
        crimesCommitted += severity;
        if (crimesCommitted >= crimesNeededForNextStar && wantedLevel < 5)
        {
            wantedLevel++;
            crimesCommitted = 0;
            Debug.Log("Wanted Level Increased: " + wantedLevel + " Stars!");
        }
    }

    void SpawnPoliceNearPlayer()
    {
        // Find a random waypoint near the player to spawn a cop
        List<Vector3> validSpawnPoints = new List<Vector3>();

        foreach (Path path in allPaths)
        {
            foreach (var wp in path.waypoints)
            {
                float dist = Vector3.Distance(player.position, wp.position);
                if (dist > 15f && dist < spawnDistance)
                {
                    validSpawnPoints.Add(wp.position);
                }
            }
        }

        if (validSpawnPoints.Count > 0)
        {
            Vector3 spawnPos = validSpawnPoints[Random.Range(0, validSpawnPoints.Count)];
            Instantiate(policePrefab, spawnPos, Quaternion.identity);
        }
    }
}