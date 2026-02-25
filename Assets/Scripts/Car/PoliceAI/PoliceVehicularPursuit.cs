using System.Collections;
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

    [Header("Driving")]
    [SerializeField] private float maxSteerInput = 1f;
    [SerializeField] private float steeringSensitivity = 2f;
    [SerializeField] private float slowingAngle = 60f;

    // References
    private PlayerDriving playerDrivingScript;
    private Rigidbody2D rb;
    private NavMeshAgent agent;

    // State
    private bool drivable = true;
    private bool exited = false;

    // AI Input
    private float steerInput;
    private float accelInput = 1f;

    // Speed
    private float currentSpeed;

    // Reverse
    private bool reverse = false;
    private bool start = true;
    private bool accelerating = true;

    [Header("Car Settings")]
    public float acceleration = 20f;
    public float maxSpeed = 15f;
    public float steeringPower = 200f;

    private float reverseTime = 0.5f;
    private float reversePower = 5f;


    // =====================================================
    // INIT
    // =====================================================

    void Awake()
    {
        playerDrivingScript = FindFirstObjectByType<PlayerDriving>();

        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();

        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        PlaceOnNavMesh();
    }


    void PlaceOnNavMesh()
    {
        if (!agent.isOnNavMesh)
        {
            NavMeshHit hit;

            if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
        }
    }


    // =====================================================
    // UPDATE
    // =====================================================

    void Update()
    {
        if (!drivable || exited) return;

        FindTarget();

        if (!target) return;

        float distance = Vector2.Distance(transform.position, target.position);
        currentSpeed = rb.linearVelocity.magnitude;


        // 🚔 Exit car when close and player is on foot
        if (!exited &&
            distance < exitCarDistance &&
            playerDrivingScript != null &&
            playerDrivingScript.isDriving == false)
        {
            ExitVehicle();
            return;
        }


        // NavMesh update
        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.SetDestination(target.position);
        }


        if (agent.enabled && agent.isOnNavMesh)
        {
            HandleSteering();
        }
        else
        {
            steerInput = 0f;
        }

        HandleAcceleration();
    }


    // =====================================================
    // PHYSICS
    // =====================================================

    void FixedUpdate()
    {
        if (!drivable || exited) return;
        if (reverse) return;


        if (agent.enabled)
            agent.nextPosition = rb.position;


        if (currentSpeed <= 0.2f && !start && !reverse && !accelerating)
        {
            StartCoroutine(Reverse());
        }


        ApplyEngine(1f);
        ApplySteering();
        LimitSpeed();

        start = false;

        if (currentSpeed >= maxSpeed / 2f)
            accelerating = false;
    }


    // =====================================================
    // AI
    // =====================================================

    void FindTarget()
    {
        if (target) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player)
            target = player.transform;
    }


    void HandleSteering()
    {
        if (!agent.enabled ||
            !agent.isOnNavMesh ||
            agent.pathPending ||
            agent.path.corners.Length < 2)
        {
            steerInput = 0f;
            return;
        }

        Vector2 nextCorner = agent.path.corners[1];
        Vector2 toCorner = (nextCorner - rb.position).normalized;

        float angle = Vector2.SignedAngle(transform.up, toCorner);

        steerInput = Mathf.Clamp(
            angle / steeringSensitivity,
            -maxSteerInput,
            maxSteerInput
        );
    }


    void HandleAcceleration()
    {
        float absAngle = Mathf.Abs(steerInput * steeringSensitivity);

        accelInput = absAngle > slowingAngle ? 0.5f : 1f;
    }


    // =====================================================
    // VEHICLE
    // =====================================================

    void ApplyEngine(float dir)
    {
        Vector2 forward = transform.up;

        rb.AddForce(
            forward * accelInput * dir * acceleration,
            ForceMode2D.Force
        );
    }


    void ApplySteering()
    {
        float speedFactor = rb.linearVelocity.magnitude / maxSpeed;

        rb.angularVelocity = steerInput * steeringPower * speedFactor;
    }


    void LimitSpeed()
    {
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity =
                rb.linearVelocity.normalized * maxSpeed;
        }
    }


    // =====================================================
    // REVERSE
    // =====================================================

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


    // =====================================================
    // EXIT VEHICLE
    // =====================================================

    void ExitVehicle()
    {
        exited = true;
        drivable = false;


        // Spawn officer
        if (policeOfficer)
        {
            Instantiate(
                policeOfficer,
                transform.position,
                Quaternion.identity
            );
        }


        // Disable driving
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.simulated = false;


        if (agent)
            agent.enabled = false;


        if (pursuitScript)
            pursuitScript.enabled = false;


        Debug.Log("[Police AI] Officer exited vehicle");
    }
}
