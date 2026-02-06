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
    [SerializeField] private float exitCarDistance = 5.5f; // Distance when the officer should hop out and chase the player (if the player is on foot)

    [Header("Driving")]
    [SerializeField] private float maxSteerInput = 1f;
    [SerializeField] private float steeringSensitivity = 2f;
    [SerializeField] private float slowingAngle = 60f;

    // Scripts
    private PlayerDriving playerDrivingScript; // Code for variables to see e.g whether the player is driving or not

    //[Header("Other")]
    private Rigidbody2D rb;
    private NavMeshAgent agent;

    private int direction;
    private float distance;
    private bool drivable = true;
    private bool start = true;
    private bool reverse = false;
    private bool accelerating = true; // Used to correctly execute the reverse coroutine, the variable should not be taken literally
    private float reverseTime = 0.5f;
    private float reversePower = 5f;

    // Car inputs (AI-controlled)
    float steerInput;
    float accelInput = 1f; // Police always tries to accelerate

    // Speed
    float currentSpeed;

    // Coroutines

    IEnumerator Reverse()
    {
        reverse = true;
        start = true;
        accelerating = true;

        ApplyEngine(-reversePower);

        Debug.Log("REVERSE");

        yield return new WaitForSeconds(reverseTime);

        reverse = false;

        ApplyEngine(reversePower);
    }

    void Awake()
    {
        playerDrivingScript = FindObjectOfType<PlayerDriving>();

        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();

        // IMPORTANT: NavMeshAgent is path-only
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        if (drivable == false) return; 

        distance = Vector2.Distance(gameObject.transform.position, target.transform.position);
        currentSpeed = rb.linearVelocity.magnitude;

        //Debug.Log("Distance: " + distance);
        //Debug.Log("Speed: " + currentSpeed);

        if (distance < exitCarDistance && playerDrivingScript.isDriving == false) // If the target is nearby, officer jump out and chase
        {
            Instantiate(policeOfficer, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y), Quaternion.identity);
            rb.simulated = false;
            pursuitScript.enabled = false;
        }

        if (!target) return;

        // Update agent destination
        agent.SetDestination(target.position);

        HandleSteering();
        HandleAcceleration();
    }

    void FixedUpdate()
    {
        if (reverse) return;

        agent.nextPosition = rb.position; // sync NavMesh

        // Uncomment to enable a slight reverse in case the car is stuck. add "direction" to parameter of ApplyEngine method as well.
        if (currentSpeed <= 0.21 && start == false && reverse == false && accelerating == false)
        {
            Debug.Log("Speed: " + currentSpeed);
            Debug.Log("Start: " + start);
            Debug.Log("Reverse: " + reverse);
            Debug.Log("Accelerating: " + accelerating);
            StartCoroutine(Reverse());
        }

        ApplyEngine(1);
        ApplySteering();
        LimitSpeed();

        start = false;

        if (currentSpeed >= maxSpeed / 2)
        {
            accelerating = false;
        }
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

    void ApplyEngine(float dir)
    {
        Vector2 forward = rb.transform.up; // car's local forward
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
