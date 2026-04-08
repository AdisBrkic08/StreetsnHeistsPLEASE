using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Effects")]
    public GameObject bloodEffectPrefab;
    public GameObject deathBloodEffectPrefab;
    public CornerDamageFlash damageFlash;

    [Header("UI")]
    public GameObject deathScreen;

    [Header("Disable On Death")]
    public MonoBehaviour[] componentsToDisable;

    private GameHUD hud;
    private bool isDead = false;
    private PlayerDriving playerDrivingScript; // Reference to your driving script

    void Awake()
    {
        // Ensure health is set BEFORE anything else runs
        currentHealth = maxHealth;
    }

    void Start()
    {
        isDead = false;
        playerDrivingScript = GetComponent<PlayerDriving>();
        hud = Object.FindFirstObjectByType<GameHUD>();

        if (deathScreen != null)
            deathScreen.SetActive(false);

        // Update HUD immediately on start
        if (hud != null) hud.UpdateHUDNow();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        // Trigger effects
        FindFirstObjectByType<DamageMotionBlur>()?.TriggerDamageBlur();
        if (damageFlash != null) damageFlash.FlashCorners();
        if (bloodEffectPrefab != null)
            Instantiate(bloodEffectPrefab, transform.position, Quaternion.identity);

        currentHealth -= amount;

        // Clamp health so it doesn't go below 0
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (hud != null) hud.UpdateHUDNow();

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // Handle car exit
        if (playerDrivingScript != null && playerDrivingScript.isDriving)
        {
            playerDrivingScript.enabled = false;
        }

        // Reset Heat
        if (HeatManager.Instance != null)
        {
            HeatManager.Instance.heatLevel = 0;
            HeatManager.Instance.currentScore = 0;
        }

        // Effects
        if (deathBloodEffectPrefab != null)
            Instantiate(deathBloodEffectPrefab, transform.position, Quaternion.identity);

        // Disable components (Movement, Shooting, etc.)
        foreach (var comp in componentsToDisable)
        {
            if (comp != null) comp.enabled = false;
        }

        // NOTE: We don't show the deathScreen here anymore because 
        // the WastedSystem script will detect currentHealth <= 0 and trigger DoWasted()

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Respawn()
    {
        // Use LoadScene to completely wipe the "Unlimited Health" glitch
        // This resets ALL scripts to their default values
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    // Add this to PlayerHealth.cs
    public void ResetPlayer()
    {
        isDead = false;
        currentHealth = maxHealth;

        // Re-enable components that were disabled on death
        foreach (var comp in componentsToDisable)
        {
            if (comp != null) comp.enabled = true;
        }

        // If the player died in a car, ensure they can move again
        if (GetComponent<Rigidbody2D>() != null)
            GetComponent<Rigidbody2D>().simulated = true;
    }
}