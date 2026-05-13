using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SimpleCarController2D : MonoBehaviour
{
    // --- NEW VARIABLE FOR THE FIX ---
    [Header("Player Control")]
    public bool isDriving = false; // Set this to TRUE when player enters, FALSE when player exits
    // --------------------------------

    [Header("Base Handling")]
    public float acceleration = 20f;
    public float maxSpeed = 15f;
    public float steeringPower = 200f;
    public float linearDamping = 1f;
    public float slowingLinearDamping = 3f; // Linear damping but when there is no input or when the car has been exited out of

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

    [Header("Money Reward System")]
    public float speedThreshold = 6f;
    public int cashPerSecond = 5;
    float earnTimer;

    private Rigidbody2D rb;
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
        // --- THE FIX ---
        // If the player isn't in this car, reset inputs and stop reading keyboard
        if (!isDriving)
        {
            steerInput = 0f;
            accelInput = 0f;
            handbraking = false;
            speedbreakerActive = false;
            nitrousActive = false;
            return;
        }

        steerInput = Input.GetAxis("Horizontal");
        accelInput = Input.GetAxis("Vertical");
        handbraking = Input.GetKey(handbrakeKey);

        HandleSpeedbreakerInput();
        HandleNitrousInput();
        HandleMoneyReward();
    }

    void FixedUpdate()
    {
        Debug.Log("steer input: " + steerInput);
        Debug.Log("accel input: " + accelInput);

        // Don't apply physics forces if nobody is driving (unless you want it to roll)
        if (!isDriving)
        {
            // Optional: apply some extra friction so the car stops faster when you jump out
            rb.linearDamping = slowingLinearDamping;
            return;
        }

        if (steerInput == 0 && accelInput == 0)
        {
            rb.linearDamping = slowingLinearDamping; // Apply extra friction when no input is detected
        } 
        else
        {
            rb.linearDamping = linearDamping; // Reset to normal when driving
        }
        ApplyEngine();
        ApplySteering();
        ApplyGrip();
        LimitSpeed();
    }

    void HandleMoneyReward()
    {
        if (rb == null || !isDriving) return; // Only earn money if driving

        float speed = Mathf.Min(rb.linearVelocity.magnitude, 20f);

        if (speed >= speedThreshold)
        {
            earnTimer += Time.deltaTime;

            if (earnTimer >= 1f)
            {
                MoneyManager.Instance.AddMoney(cashPerSecond);
                earnTimer = 0f;
            }
        }
        else
        {
            earnTimer = 0f;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if we hit something hard (high velocity)
        if (collision.relativeVelocity.magnitude > 5f)
        {
            // Find the camera and tell it to shake
            CameraShake shake = Camera.main.GetComponent<CameraShake>();
            if (shake != null)
            {
                // TriggerShake(duration, magnitude)
                shake.TriggerShake(0.15f, 0.4f);
            }
        }
    }

    #region Engine & Movement
    void ApplyEngine()
    {
        float accel = accelInput * acceleration * accelerationMultiplier;

        if (handbraking) accel *= driftBoost;

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
            speedbreakerActive = !speedbreakerActive;
        }

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

    public float SpeedbreakerEnergyPercent => currentSpeedbreakerEnergy / maxSpeedbreakerEnergy;
    public float SpeedbreakerCooldownPercent => speedbreakerLocked ? speedbreakerCooldownTimer / speedbreakerCooldownTime : 0f;
    public float NitrousPercent => currentNitrous / maxNitrous;
}