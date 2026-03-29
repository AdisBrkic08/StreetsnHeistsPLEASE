using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SimpleCarController2D : MonoBehaviour
{
    [Header("Player Control")]
    public bool isDriving = false;

    [Header("Base Handling")]
    public float acceleration = 20f;
    public float maxSpeed = 15f;
    public float steeringPower = 200f;
    public float linearDamping = 1f;

    [Header("Handbrake Drift")]
    public KeyCode handbrakeKey = KeyCode.Space;
    public float handbrakeGrip = 0.25f;
    public float driftBoost = 1.3f;
    public float driftTurnMultiplier = 1.8f;

    [Header("Nitrous Boost")]
    public KeyCode nitrousKey = KeyCode.LeftControl;
    public float nitrousMultiplier = 1.8f;
    public float maxNitrous = 5f;
    public float nitrousDrainRate = 1f;
    public float nitrousRechargeRate = 0.5f;

    [Header("Money Reward System")]
    public float speedThreshold = 6f;
    public int cashPerSecond = 5;
    float earnTimer;

    [HideInInspector] public float currentNitrous;
    [HideInInspector] public float steeringPowerMultiplier = 1f;
    [HideInInspector] public float accelerationMultiplier = 1f;

    private Rigidbody2D rb;
    float steerInput;
    float accelInput;
    bool handbraking;
    bool nitrousActive;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearDamping = linearDamping;
        rb.angularDamping = 2f;
        currentNitrous = maxNitrous;
    }

    void Update()
    {
        if (!isDriving)
        {
            ResetInputs();
            return;
        }

        steerInput = Input.GetAxis("Horizontal");
        accelInput = Input.GetAxis("Vertical");
        handbraking = Input.GetKey(handbrakeKey);
        nitrousActive = Input.GetKey(nitrousKey) && currentNitrous > 0f;

        HandleMoneyReward();
    }

    void ResetInputs()
    {
        steerInput = 0f;
        accelInput = 0f;
        handbraking = false;
        nitrousActive = false;
        rb.linearDamping = 3f; // Friction so empty cars stop
    }

    void FixedUpdate()
    {
        if (!isDriving) return;

        rb.linearDamping = linearDamping;
        ApplyEngine();
        ApplySteering();
        ApplyGrip();
        LimitSpeed();
    }

    void ApplyEngine()
    {
        float accel = accelInput * acceleration * accelerationMultiplier;
        if (handbraking) accel *= driftBoost;

        if (nitrousActive)
        {
            accel *= nitrousMultiplier;
            currentNitrous -= Time.deltaTime * nitrousDrainRate;
        }
        else
        {
            currentNitrous += Time.deltaTime * nitrousRechargeRate;
        }
        currentNitrous = Mathf.Clamp(currentNitrous, 0, maxNitrous);
        rb.AddForce(transform.up * accel, ForceMode2D.Force);
    }

    void ApplySteering()
    {
        float speedFactor = rb.linearVelocity.magnitude / maxSpeed;
        float turnPower = steeringPower * steeringPowerMultiplier;
        if (handbraking) turnPower *= driftTurnMultiplier;

        rb.angularVelocity = -steerInput * turnPower * speedFactor * (handbraking ? handbrakeGrip : 1f);
    }

    void ApplyGrip()
    {
        Vector2 forwardVel = transform.up * Vector2.Dot(rb.linearVelocity, transform.up);
        Vector2 sideVel = transform.right * Vector2.Dot(rb.linearVelocity, transform.right);
        rb.linearVelocity = forwardVel + sideVel * (handbraking ? handbrakeGrip : 1f);
    }

    void LimitSpeed()
    {
        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }

    void HandleMoneyReward()
    {
        if (rb.linearVelocity.magnitude >= speedThreshold)
        {
            earnTimer += Time.deltaTime;
            if (earnTimer >= 1f) { MoneyManager.Instance.AddMoney(cashPerSecond); earnTimer = 0f; }
        }
    }
}