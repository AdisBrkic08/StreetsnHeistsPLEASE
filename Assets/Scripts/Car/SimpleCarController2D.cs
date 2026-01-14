using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SimpleCarController2D : MonoBehaviour
{
    [Header("Base Handling")]
    public float acceleration = 20f;
    public float maxSpeed = 15f;
    public float steeringPower = 200f;
    public float linearDamping = 1f;  // renamed


    [Header("Handbrake Drift")]
    public KeyCode handbrakeKey = KeyCode.Space;
    public float handbrakeGrip = 0.25f;
    public float driftBoost = 1.3f;
    public float driftTurnMultiplier = 1.8f;

    [Header("Speedbreaker Modifiers")]
    [HideInInspector] public float steeringPowerMultiplier = 1f;
    [HideInInspector] public float accelerationMultiplier = 1f;

    Rigidbody2D rb;
    float steerInput;
    float accelInput;
    bool handbraking;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearDamping = linearDamping;  // smooth slowing of forward motion
        rb.angularDamping = 2f;            // smooth angular rotation

    }


    void Update()
    {
        steerInput = Input.GetAxis("Horizontal");
        accelInput = Input.GetAxis("Vertical");
        handbraking = Input.GetKey(handbrakeKey);
    }

    void FixedUpdate()
    {
        ApplyEngine();
        ApplySteering();
        LimitSpeed();
        ApplyGrip();
    }

    void ApplyEngine()
    {
        float accel = accelInput * acceleration * accelerationMultiplier;

        if (handbraking)
            accel *= driftBoost;

        rb.AddForce(transform.up * accel, ForceMode2D.Force);
    }

    void ApplySteering()
    {
        float speedFactor = rb.linearVelocity.magnitude / maxSpeed;
        float grip = handbraking ? handbrakeGrip : 1f;

        float turnPower = steeringPower * steeringPowerMultiplier;
        if (handbraking)
            turnPower *= driftTurnMultiplier;

        rb.angularVelocity = -steerInput * turnPower * speedFactor * grip;
    }

    void ApplyGrip()
    {
        Vector2 forwardVel = transform.up * Vector2.Dot(rb.linearVelocity, transform.up);
        Vector2 sideVel = transform.right * Vector2.Dot(rb.linearVelocity, transform.right);

        float grip = handbraking ? handbrakeGrip : 1f;

        rb.linearVelocity = forwardVel + sideVel * grip;
    }

    void LimitSpeed()
    {
        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }
}
