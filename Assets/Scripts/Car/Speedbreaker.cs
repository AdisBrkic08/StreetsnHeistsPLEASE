using UnityEngine;

public class Speedbreaker : MonoBehaviour
{
    [Header("Controls")]
    public KeyCode speedbreakerKey = KeyCode.LeftShift;

    [Header("Time Settings")]
    public float slowTimeScale = 0.3f;
    public float normalTimeScale = 1f;
    public float transitionSpeed = 5f;

    [Header("Handling Boost")]
    public SimpleCarController2D car;
    public float steeringMultiplier = 1.8f;
    public float accelerationMultiplier = 1.3f;

    [Header("Energy")]
    public float maxEnergy = 5f;
    public float drainRate = 1f;
    public float rechargeRate = 0.7f;

    [Header("Cooldown")]
    public float cooldownTime = 4f;
    float cooldownTimer = 0f;
    bool locked;

    float energy;
    bool active;

    public float EnergyPercent => energy / maxEnergy;
    public float CooldownPercent => locked ? cooldownTimer / cooldownTime : 0f;
    public bool IsLocked => locked;

    void Start()
    {
        energy = maxEnergy;
        if (car == null) car = GetComponent<SimpleCarController2D>();
    }

    void Update()
    {
        if (!car.enabled || !car.isDriving)
        {
            if (active) ResetSpeedbreaker();
            return;
        }

        HandleCooldown();

        if (Input.GetKeyDown(speedbreakerKey) && !locked && energy > 0f)
            active = !active;

        if (active)
            energy -= Time.unscaledDeltaTime * drainRate;
        else
            energy += Time.unscaledDeltaTime * rechargeRate;

        energy = Mathf.Clamp(energy, 0, maxEnergy);

        if (energy <= 0f && active)
        {
            active = false;
            StartCooldown();
        }

        HandleTime();
        HandleHandling();
    }

    public void ResetSpeedbreaker()
    {
        active = false;
        Time.timeScale = normalTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        if (car != null)
        {
            car.steeringPowerMultiplier = 1f;
            car.accelerationMultiplier = 1f;
        }
    }

    void HandleCooldown()
    {
        if (!locked) return;
        cooldownTimer -= Time.unscaledDeltaTime;
        if (cooldownTimer <= 0f) locked = false;
    }

    void StartCooldown()
    {
        locked = true;
        cooldownTimer = cooldownTime;
    }

    void HandleTime()
    {
        float target = active ? slowTimeScale : normalTimeScale;
        Time.timeScale = Mathf.Lerp(Time.timeScale, target, Time.unscaledDeltaTime * transitionSpeed);
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    void HandleHandling()
    {
        car.steeringPowerMultiplier = active ? steeringMultiplier : 1f;
        car.accelerationMultiplier = active ? accelerationMultiplier : 1f;
    }
}