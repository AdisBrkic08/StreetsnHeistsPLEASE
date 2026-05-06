using UnityEngine;
using System.Collections.Generic;

public class WantedManager : MonoBehaviour
{
    public static WantedManager Instance;

    [Header("Wanted Stats")]
    public int wantedLevel = 0;
    public int crimesCommitted = 0;
    public int crimesNeededForNextStar = 2;

    [Header("Police Spawning")]
    public GameObject policePrefab;
    public float spawnDistance = 30f;
    public float spawnInterval = 5f;
    private float spawnTimer;

    // --- NEW LOGIC FOR CAR HIJACKING ---
    private int lastHijackedCarID = -1;

    private Transform player;
    private List<Path> allPaths = new List<Path>();

    void Awake() { Instance = this; }

    void Start()
    {
        // This forces the game to run at 60 FPS to help stop the stuttering
        Application.targetFrameRate = 60;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        allPaths.AddRange(FindObjectsByType<Path>(FindObjectsSortMode.None));
    }

    void Update()
    {
        if (wantedLevel <= 0) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= (spawnInterval / wantedLevel))
        {
            SpawnPoliceNearPlayer();
            spawnTimer = 0;
        }
    }

    // UPDATED: Added carInstanceID parameter (default to -1 for non-car crimes)
    public void ReportCrime(int severity, GameObject vehicle = null)
    {
       

        // Standard crime logic
        crimesCommitted += severity;
        if (crimesCommitted >= crimesNeededForNextStar && wantedLevel < 5)
        {
            wantedLevel++;
            crimesCommitted = 0;
            Debug.Log("Wanted Level Increased: " + wantedLevel + " Stars!");
        }
    }

    // ... (Keep your SpawnPoliceNearPlayer and other methods the same)
    void SpawnPoliceNearPlayer()
    {
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