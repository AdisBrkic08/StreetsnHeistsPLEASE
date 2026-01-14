using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SimpleCarController2D : MonoBehaviour
{
    [Header("Base Handling")]
    public float acceleration = 20f;
    public float maxSpeed = 15f;
    public float steeringPower = 200f;
    public float linearDamping = 1f;      // replaces deprecated drag

    [Header("Handbrake Drift")]
    public KeyCode handbrakeKey = KeyCode.Space;
    public float handbrakeGrip = 0.25f;
    public float driftBoost = 1.3f;
    public float driftTurnMultiplier = 1.8f;

    [Header("Speedbreaker")]
    public KeyCode speedbreakerKey = KeyCode.LeftShift;
    public float slowTimeScale = 0.3f;
    public float transitionSpeed = 5f;
    public float speedbreakerSteeringMultiplier = 1.8f;
    public float speedbreakerAccelerationMultiplier = 1.3f;
    public float maxSpeedbreakerEnergy = 5f;
    public float speedbreakerDrainRate = 1f;
    public float speedbreakerRechargeRate = 0.7f;
    public float speedbreakerCooldownTime = 4f;

    [Header("Nitrous Boost")]
    public KeyCode nitrousKey = KeyCode.LeftControl;
    public float nitrousMultiplier = 1.8f;
    public float maxNitrous = 5f;
    public float nitrousDrainRate = 1f;
    public float nitrousRechargeRate = 0.5f;

    [Header("References for UI")]
    [HideInInspector] public float currentNitrous;
    [HideInInspector] public float currentSpeedbreakerEnergy;
    [HideInInspector] public bool speedbreakerActive;
    [HideInInspector] public bool speedbreakerLocked;

    [HideInInspector] public float steeringPowerMultiplier = 1f;
    [HideInInspector] public float accelerationMultiplier = 1f;

    Rigidbody2D rb;
    float steerInput;
    float accelInput;
    bool handbraking;

    float speedbreakerCooldownTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearDamping = linearDamping;
        rb.angularDamping = 2f;

        currentNitrous = maxNitrous;
        currentSpeedbreakerEnergy = maxSpeedbreakerEnergy;
    }

    void Update()
    {
        steerInput = Input.GetAxis("Horizontal");
        accelInput = Input.GetAxis("Vertical");
        handbraking = Input.GetKey(handbrakeKey);

        HandleSpeedbreakerInput();
        HandleNitrousInput();
    }

    void FixedUpdate()
    {
        ApplyEngine();
        ApplySteering();
        ApplyGrip();
        LimitSpeed();
    }

    #region Engine & Movement
    void ApplyEngine()
    {
        float accel = accelInput * acceleration * accelerationMultiplier;

        // Handbrake boost
        if (handbraking) accel *= driftBoost;

        // Nitrous
        if (nitrousActive && currentNitrous > 0f)
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
        float grip = handbraking ? handbrakeGrip : 1f;

        float turnPower = steeringPower * steeringPowerMultiplier;
        if (handbraking) turnPower *= driftTurnMultiplier;
        if (speedbreakerActive)
        {
            turnPower *= speedbreakerSteeringMultiplier;
        }

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
    #endregion

    #region Speedbreaker
    void HandleSpeedbreakerInput()
    {
        if (!speedbreakerLocked && Input.GetKeyDown(speedbreakerKey) && currentSpeedbreakerEnergy > 0f)
        {
            speedbreakerActive = !speedbreakerActive; // toggle
        }

        // Energy drain/recharge
        if (speedbreakerActive)
        {
            currentSpeedbreakerEnergy -= Time.unscaledDeltaTime * speedbreakerDrainRate;
            if (currentSpeedbreakerEnergy <= 0f)
            {
                speedbreakerActive = false;
                StartSpeedbreakerCooldown();
            }
        }
        else
        {
            currentSpeedbreakerEnergy += Time.unscaledDeltaTime * speedbreakerRechargeRate;
        }

        currentSpeedbreakerEnergy = Mathf.Clamp(currentSpeedbreakerEnergy, 0, maxSpeedbreakerEnergy);

        HandleCooldown();
        HandleTimeScale();
    }

    void StartSpeedbreakerCooldown()
    {
        speedbreakerLocked = true;
        speedbreakerCooldownTimer = speedbreakerCooldownTime;
    }

    void HandleCooldown()
    {
        if (!speedbreakerLocked) return;

        speedbreakerCooldownTimer -= Time.unscaledDeltaTime;
        if (speedbreakerCooldownTimer <= 0f)
            speedbreakerLocked = false;
    }

    void HandleTimeScale()
    {
        float target = speedbreakerActive ? slowTimeScale : 1f;
        Time.timeScale = Mathf.Lerp(Time.timeScale, target, Time.unscaledDeltaTime * transitionSpeed);
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }
    #endregion

    #region Nitrous
    bool nitrousActive;
    void HandleNitrousInput()
    {
        nitrousActive = Input.GetKey(nitrousKey) && currentNitrous > 0f;
    }
    #endregion

    #region UI Helpers
    // Optional: Use these to link to UI bars
    public float SpeedbreakerEnergyPercent => currentSpeedbreakerEnergy / maxSpeedbreakerEnergy;
    public float SpeedbreakerCooldownPercent => speedbreakerLocked ? speedbreakerCooldownTimer / speedbreakerCooldownTime : 0f;
    public float NitrousPercent => currentNitrous / maxNitrous;
    #endregion
}
