using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Effects")]
    public GameObject bloodEffectPrefab;       // small blood burst
    public GameObject deathBloodEffectPrefab;  // big blood burst
    public CornerDamageFlash damageFlash;

    [Header("UI")]
    public GameObject deathScreen;

    [Header("Disable On Death")]
    public MonoBehaviour[] componentsToDisable;

    private GameHUD hud;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        // Find HUD safely
        hud = Object.FindFirstObjectByType<GameHUD>();

        if (deathScreen != null)
            deathScreen.SetActive(false);
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        // Flash screen corners  
        if (damageFlash != null)
            damageFlash.FlashCorners();

        // Small blood burst  
        if (bloodEffectPrefab != null)
            Instantiate(bloodEffectPrefab, transform.position, Quaternion.identity);

        // Update HUD instantly  
        if (hud != null)
            hud.UpdateHUDNow();

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        isDead = true;

        // Big blood burst
        if (deathBloodEffectPrefab != null)
            Instantiate(deathBloodEffectPrefab, transform.position, Quaternion.identity);

        // Disable movement, shooting, etc.
        foreach (var comp in componentsToDisable)
            if (comp != null)
                comp.enabled = false;

        // Show death UI
        if (deathScreen != null)
            deathScreen.SetActive(true);
    }

    public void Respawn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
