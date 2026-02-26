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

    void Start()
    {
        ResetState();

        hud = Object.FindFirstObjectByType<GameHUD>();

        if (deathScreen != null)
            deathScreen.SetActive(false);
    }

    // ================= DAMAGE =================

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        FindFirstObjectByType<DamageMotionBlur>()?.TriggerDamageBlur();

        currentHealth -= amount;

        if (damageFlash != null)
            damageFlash.FlashCorners();

        if (bloodEffectPrefab != null)
            Instantiate(bloodEffectPrefab, transform.position, Quaternion.identity);

        if (hud != null)
            hud.UpdateHUDNow();

        if (currentHealth <= 0)
            Die();
    }

    // ================= DEATH =================

    void Die()
    {
        if (isDead) return;

        isDead = true;

        if (deathBloodEffectPrefab != null)
            Instantiate(deathBloodEffectPrefab, transform.position, Quaternion.identity);

        // Disable movement / shooting / etc
        foreach (var comp in componentsToDisable)
        {
            if (comp != null)
                comp.enabled = false;
        }

        if (deathScreen != null)
            deathScreen.SetActive(true);
    }

    // ================= RESPawn =================

    public void Respawn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ================= RESET STATE =================

    void ResetState()
    {
        currentHealth = maxHealth;
        isDead = false;

        // Re-enable components after scene reload
        foreach (var comp in componentsToDisable)
        {
            if (comp != null)
                comp.enabled = true;
        }
    }
}