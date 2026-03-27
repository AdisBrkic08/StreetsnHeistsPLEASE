using UnityEngine;

public class RandomWalker : MonoBehaviour
{
    [HideInInspector] public Path externalPath;
    private int currentWaypointIndex = 0;

    [Header("Movement")]
    public float speed = 5f;
    private float currentSpeed; // Internal speed for smoothing
    public float waypointTolerance = 0.6f;
    public float rotationSpeed = 5f;
    public float spriteRotationOffset = -90f;

    [Header("Avoidance")]
    public float detectionDistance = 3f;
    public LayerMask obstacleLayers; // Set this to 'Player' and 'Traffic' in Inspector

    [HideInInspector] public bool isAIActive = true;

    void Start()
    {
        currentSpeed = speed;
    }

    public void SetStartingWaypoint(int index)
    {
        currentWaypointIndex = index + 1;
        if (externalPath != null && currentWaypointIndex >= externalPath.waypoints.Count)
            currentWaypointIndex = 0;

        LookAtImmediately();
    }

    void Update()
    {
        if (!isAIActive || externalPath == null || externalPath.waypoints.Count == 0) return;
        if (currentWaypointIndex >= externalPath.waypoints.Count) return;

        Vector3 targetPos = externalPath.waypoints[currentWaypointIndex].position;

        // --- AVOIDANCE LOGIC ---
        bool obstacleAhead = CheckForObstacles();
        float targetSpeed = obstacleAhead ? 0f : speed;

        // Smoothly accelerate/decelerate
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 3f);

        // --- MOVEMENT & ROTATION ---
        RotateTowardsTarget(targetPos);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, currentSpeed * Time.deltaTime);

        // --- WAYPOINT PROGRESSION ---
        if (Vector3.Distance(transform.position, targetPos) < waypointTolerance)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= externalPath.waypoints.Count) Destroy(gameObject);
        }
    }

    bool CheckForObstacles()
    {
        // Direction is the car's current "Forward" based on its sprite rotation
        // If your car faces Right by default, use transform.right
        // If it faces Up by default, use transform.up
        Vector2 forwardDir = transform.right;
        if (spriteRotationOffset == -90f) forwardDir = transform.up;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, forwardDir, detectionDistance, obstacleLayers);

        // Debug line so you can see the "eyes" in the Scene view
        Debug.DrawRay(transform.position, forwardDir * detectionDistance, hit ? Color.red : Color.green);

        return hit.collider != null;
    }

    void RotateTowardsTarget(Vector3 target)
    {
        Vector2 direction = (Vector2)target - (Vector2)transform.position;
        if (direction.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRot = Quaternion.Euler(0, 0, angle + spriteRotationOffset);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    void LookAtImmediately()
    {
        if (externalPath == null) return;
        Vector3 target = externalPath.waypoints[currentWaypointIndex].position;
        Vector2 direction = (Vector2)target - (Vector2)transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + spriteRotationOffset);
    }
}