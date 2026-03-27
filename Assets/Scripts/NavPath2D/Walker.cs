using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWalker : MonoBehaviour
{
    [HideInInspector] public Path externalPath;
    private int currentWaypointIndex = 0;

    [Header("Movement Settings")]
    public float speed = 5f;
    public float waypointTolerance = 0.5f;
    public float rotationSpeed = 5f;
    public float spriteRotationOffset = -90f;

    [HideInInspector] public bool isAIActive = true;

    void Update()
    {
        // Safety check: Don't run if no path is assigned
        if (!isAIActive || externalPath == null || externalPath.waypoints == null || externalPath.waypoints.Count == 0)
            return;

        // Ensure we don't go out of bounds
        if (currentWaypointIndex >= externalPath.waypoints.Count) return;

        Vector3 targetPos = externalPath.waypoints[currentWaypointIndex].position;

        // 1. Handle Rotation
        RotateTowardsTarget(targetPos);

        // 2. Handle Movement
        // ERROR CHECK: Ensure 'speed' is a float and 'Time.deltaTime' has no brackets
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // 3. Check distance to current waypoint
        if (Vector2.Distance(transform.position, targetPos) < waypointTolerance)
        {
            currentWaypointIndex++;

            // If we reached the end of the path
            if (currentWaypointIndex >= externalPath.waypoints.Count)
            {
                Destroy(gameObject);
            }
        }
    }

    void RotateTowardsTarget(Vector3 target)
    {
        Vector2 direction = (Vector2)target - (Vector2)transform.position;

        if (direction.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRot = Quaternion.Euler(0, 0, angle + spriteRotationOffset);

            // ERROR CHECK: Ensure 'rotationSpeed' is a float
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }
}