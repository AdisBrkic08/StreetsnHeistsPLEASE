using UnityEngine;

public class Speedbreaker : MonoBehaviour
{
    [Header("Controls")]
    public KeyCode speedbreakerKey = KeyCode.LeftShift;

    [Header("Time Settings")]
    public float slowTimeScale = 0.3f;
    public float transitionSpeed = 5f;

    [Header("Energy")]
    public float maxEnergy = 5f;
    public float drainRate = 1f;
    public float rechargeRate = 0.7f;
    public float cooldownTime = 4f;

    public SimpleCarController2D car;
    private float energy;
    private float cooldownTimer;
    private bool active;
    private bool locked;

    void Awake() => car = GetComponent<SimpleCarController2D>();

    void Start() => energy = maxEnergy;

    void OnEnable() { ResetState(); }
    void OnDisable() { ResetState(); }

    void ResetState()
    {
        active = false;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        if (car != null) { car.steeringPowerMultiplier = 1f; car.accelerationMultiplier = 1f; }
    }

    void Update()
    {
        if (car == null || !car.isDriving) return;

        if (locked)
        {
            cooldownTimer -= Time.unscaledDeltaTime;
            if (cooldownTimer <= 0) locked = false;
        }

        if (Input.GetKeyDown(speedbreakerKey) && !locked && energy > 0.1f) active = !active;

        if (active)
        {
            energy -= Time.unscaledDeltaTime * drainRate;
            if (energy <= 0) { active = false; locked = true; cooldownTimer = cooldownTime; }
        }
        else
        {
            energy += Time.unscaledDeltaTime * rechargeRate;
        }

        energy = Mathf.Clamp(energy, 0, maxEnergy);
        HandleTimeAndHandling();
    }

    void HandleTimeAndHandling()
    {
        float targetTime = active ? slowTimeScale : 1f;
        Time.timeScale = Mathf.Lerp(Time.timeScale, targetTime, Time.unscaledDeltaTime * transitionSpeed);
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        car.steeringPowerMultiplier = active ? 1.8f : 1f;
        car.accelerationMultiplier = active ? 1.3f : 1f;
    }

    public float EnergyPercent => energy / maxEnergy;
    public float CooldownPercent => locked ? (cooldownTimer / cooldownTime) : 0f;
    public bool IsLocked => locked;
}