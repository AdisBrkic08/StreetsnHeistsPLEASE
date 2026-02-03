using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(NavMeshAgent))]
public class PoliceVehicularPursuit : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Driving")]
    [SerializeField] private float maxSteerInput = 1f;
    [SerializeField] private float steeringSensitivity = 2f;
    [SerializeField] private float slowingAngle = 60f;

    private Rigidbody2D rb;
    private NavMeshAgent agent;

    // Car inputs (AI-controlled)
    float steerInput;
    float accelInput = 1f; // Police always tries to accelerate

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();

        // IMPORTANT: NavMeshAgent is path-only
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        if (!target) return;

        // Update agent destination
        agent.SetDestination(target.position);

        HandleSteering();
        HandleAcceleration();
    }

    void FixedUpdate()
    {
        agent.nextPosition = rb.position; // sync NavMesh

        ApplyEngine();
        ApplySteering();
        LimitSpeed();
    }


    #region AI Logic
    void HandleSteering()
    {
        if (agent.pathPending || agent.path.corners.Length < 2)
        {
            steerInput = 0f;
            return;
        }

        // Next corner on NavMesh path
        Vector2 nextCorner = agent.path.corners[1];
        Vector2 toCorner = (nextCorner - rb.position).normalized;

        // Angle between car forward and desired direction
        float angle = Vector2.SignedAngle(transform.up, toCorner);

        // Convert angle to steering input
        steerInput = Mathf.Clamp(angle / steeringSensitivity, -maxSteerInput, maxSteerInput);
    }

    void HandleAcceleration()
    {
        // Reduce acceleration on sharp turns
        float absAngle = Mathf.Abs(steerInput * steeringSensitivity);
        accelInput = absAngle > slowingAngle ? 0.5f : 1f;
    }
    #endregion

    #region Vehicle Physics (adapted from SimpleCarController2D)
    [Header("Car Settings")]
    public float acceleration = 20f;
    public float maxSpeed = 15f;
    public float steeringPower = 200f;

    void ApplyEngine()
    {
        Vector2 forward = rb.transform.up; // car's local forward
        rb.AddForce(forward * accelInput * acceleration, ForceMode2D.Force);
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
