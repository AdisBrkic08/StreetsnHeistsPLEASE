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

    [Header("Catch-Up (Rubber Banding)")]
    [SerializeField] private bool useCatchUp = true;
    [SerializeField] private float minDistance = 10f;  // Normal speed zone
    [SerializeField] private float maxDistance = 50f;  // Super fast zone
    [SerializeField] private float catchUpMultiplier = 2.5f; // How much faster they get when far away
    private float baseMaxSpeed;      // Stores original maxSpeed
    private float baseAcceleration;  // Stores original acceleration

    [Header("Other")]
    [SerializeField] private float lifeTimeAfterOfficerExit;

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

    IEnumerator Reverse()
    {
        reverse = true;
        start = true;
        accelerating = true;
        ApplyEngine(-reversePower);
        yield return new WaitForSeconds(reverseTime);
        reverse = false;
        ApplyEngine(reversePower);
    }

    void Awake()
    {
        playerDrivingScript = FindFirstObjectByType<PlayerDriving>();
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();

        // Store original values from the inspector
        baseMaxSpeed = maxSpeed;
        baseAcceleration = acceleration;

        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        if (drivable == false) return;

        if (!target)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }

        distance = Vector2.Distance(transform.position, target.position);
        currentSpeed = rb.linearVelocity.magnitude;

        // Apply Catch-Up Logic
        if (useCatchUp) ApplyCatchUpLogic();

        if (distance < exitCarDistance && playerDrivingScript.isDriving == false)
        {
            Instantiate(policeOfficer, transform.position, Quaternion.identity);
            rb.simulated = false;
            if (pursuitScript) pursuitScript.enabled = false;
            Destroy(gameObject, lifeTimeAfterOfficerExit);
        }

        agent.SetDestination(target.position);
        HandleSteering();
        HandleAcceleration();
    }

    void ApplyCatchUpLogic()
    {
        // Calculate a 0 to 1 value based on distance
        // 0 = Close (Normal Speed), 1 = Far (Maximum Catch-up)
        float t = Mathf.InverseLerp(minDistance, maxDistance, distance);

        // Dynamically scale maxSpeed and acceleration
        // As distance increases, speed increases up to (base * multiplier)
        maxSpeed = Mathf.Lerp(baseMaxSpeed, baseMaxSpeed * catchUpMultiplier, t);
        acceleration = Mathf.Lerp(baseAcceleration, baseAcceleration * catchUpMultiplier, t);
    }

    void FixedUpdate()
    {
        if (reverse) return;
        agent.nextPosition = rb.position;

        if (currentSpeed <= 0.21 && start == false && reverse == false && accelerating == false)
        {
            StartCoroutine(Reverse());
        }

        ApplyEngine(1);
        ApplySteering();
        LimitSpeed();

        start = false;
        if (currentSpeed >= maxSpeed / 2) accelerating = false;
    }

    #region AI Logic
    void HandleSteering()
    {
        if (agent.pathPending || agent.path.corners.Length < 2)
        {
            steerInput = 0f;
            return;
        }

        Vector2 nextCorner = agent.path.corners[1];
        Vector2 toCorner = (nextCorner - rb.position).normalized;
        float angle = Vector2.SignedAngle(transform.up, toCorner);
        steerInput = Mathf.Clamp(angle / steeringSensitivity, -maxSteerInput, maxSteerInput);
    }

    void HandleAcceleration()
    {
        float absAngle = Mathf.Abs(steerInput * steeringSensitivity);
        accelInput = absAngle > slowingAngle ? 0.5f : 1f;
    }
    #endregion

    #region Vehicle Physics
    [Header("Car Settings")]
    public float acceleration = 20f;
    public float maxSpeed = 15f;
    public float steeringPower = 200f;

    void ApplyEngine(float dir)
    {
        Vector2 forward = transform.up;
        rb.AddForce(forward * (accelInput * dir) * acceleration, ForceMode2D.Force);
    }

    void ApplySteering()
    {
        float speedFactor = rb.linearVelocity.magnitude / maxSpeed;
        rb.angularVelocity = steerInput * steeringPower * speedFactor;
    }

    void LimitSpeed()
    {
        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }
    #endregion
}