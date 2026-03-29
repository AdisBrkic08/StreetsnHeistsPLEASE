using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(NavMeshAgent))]
public class PoliceVehicularPursuit : MonoBehaviour
{
    [SerializeField] private MonoBehaviour pursuitScript;

    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private GameObject policeOfficer;
    [SerializeField] private float exitCarDistance = 5.5f;

    [Header("Driving AI")]
    [SerializeField] private float maxSteerInput = 1f;
    [SerializeField] private float steeringSensitivity = 2f;
    [SerializeField] private float slowingAngle = 60f;

    [Header("Catch-Up")]
    [SerializeField] private bool useCatchUp = true;
    [SerializeField] private float minDistance = 10f;
    [SerializeField] private float maxDistance = 50f;
    [SerializeField] private float catchUpMultiplier = 2.5f;
    private float baseMaxSpeed;
    private float baseAcceleration;

    [Header("Other")]
    [SerializeField] private float lifeTimeAfterOfficerExit = 10f;

    private PlayerDriving playerDrivingScript;
    private Rigidbody2D rb;
    private NavMeshAgent agent;

    private bool drivable = true;
    private bool start = true;
    private bool reverse = false;
    private bool accelerating = true;
    private float reverseTime = 0.5f;
    private float reversePower = 5f;
    private float distance;

    float steerInput;
    float accelInput = 1f;
    float currentSpeed;

    void Awake()
    {
        playerDrivingScript = FindFirstObjectByType<PlayerDriving>();
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();

        baseMaxSpeed = maxSpeed;
        baseAcceleration = acceleration;

        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        if (!drivable) return;

        if (!target)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
            else return;
        }

        distance = Vector2.Distance(transform.position, target.position);
        currentSpeed = rb.linearVelocity.magnitude;

        if (useCatchUp) ApplyCatchUpLogic();

        if (distance < exitCarDistance && playerDrivingScript != null && !playerDrivingScript.isDriving)
        {
            SpawnOfficerAndCleanup();
        }

        agent.SetDestination(target.position);
        HandleSteering();
        HandleAcceleration();
    }

    void SpawnOfficerAndCleanup()
    {
        drivable = false;

        // Spawn to the side
        Vector3 spawnPos = transform.position + (transform.right * 2f);

        // CRITICAL: Set parent to null so the officer is independent of the car
        GameObject officer = Instantiate(policeOfficer, spawnPos, Quaternion.identity, null);
        officer.tag = "NPC";

        // Physics Cleanup
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        if (pursuitScript) pursuitScript.enabled = false;

        // The car will vanish, but the officer (now independent) will stay
        Destroy(gameObject, lifeTimeAfterOfficerExit);
    }

    void ApplyCatchUpLogic()
    {
        float t = Mathf.InverseLerp(minDistance, maxDistance, distance);
        maxSpeed = Mathf.Lerp(baseMaxSpeed, baseMaxSpeed * catchUpMultiplier, t);
        acceleration = Mathf.Lerp(baseAcceleration, baseAcceleration * catchUpMultiplier, t);
    }

    void FixedUpdate()
    {
        if (reverse || !drivable) return;
        agent.nextPosition = rb.position;
        if (currentSpeed <= 0.21 && !start && !reverse && !accelerating) StartCoroutine(ReverseRoutine());
        ApplyEngine(1);
        ApplySteering();
        LimitSpeed();
        start = false;
        if (currentSpeed >= maxSpeed / 2) accelerating = false;
    }

    IEnumerator ReverseRoutine() { reverse = true; ApplyEngine(-reversePower); yield return new WaitForSeconds(reverseTime); reverse = false; }

    #region AI Math
    void HandleSteering()
    {
        if (agent.pathPending || agent.path.corners.Length < 2) { steerInput = 0f; return; }
        Vector2 nextCorner = agent.path.corners[1];
        Vector2 toCorner = (nextCorner - rb.position).normalized;
        float angle = Vector2.SignedAngle(transform.up, toCorner);
        steerInput = Mathf.Clamp(angle / steeringSensitivity, -maxSteerInput, maxSteerInput);
    }

    void HandleAcceleration() { float absAngle = Mathf.Abs(steerInput * steeringSensitivity); accelInput = absAngle > slowingAngle ? 0.5f : 1f; }

    [Header("Car Settings")]
    public float acceleration = 20f;
    public float maxSpeed = 15f;
    public float steeringPower = 200f;

    void ApplyEngine(float dir) { rb.AddForce(transform.up * (accelInput * dir) * acceleration, ForceMode2D.Force); }
    void ApplySteering() { float speedFactor = rb.linearVelocity.magnitude / maxSpeed; rb.angularVelocity = steerInput * steeringPower * speedFactor; }
    void LimitSpeed() { if (rb.linearVelocity.magnitude > maxSpeed) rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed; }
    #endregion
}