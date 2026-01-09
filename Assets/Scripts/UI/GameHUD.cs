using UnityEngine;
using TMPro;

public class GameHUD : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI cashText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI healthText;

    [Header("Player References")]
    public PlayerMoney playerMoney;
    public PlayerHealth playerHealth;

    [Header("Settings")]
    public float gameTime = 12 * 60f; // Start at 12:00 (in minutes)

    void Start()
    {
        // ✅ Updated for Unity 2023+: use FindFirstObjectByType instead of deprecated FindObjectOfType
        if (playerMoney == null)
            playerMoney = Object.FindFirstObjectByType<PlayerMoney>();

        if (playerHealth == null)
            playerHealth = Object.FindFirstObjectByType<PlayerHealth>();
    }

    void Update()
    {
        UpdateTimeDisplay();
        UpdateHUD();
    }

    void UpdateHUD()
    {
        // 💰 Update money display
        if (playerMoney != null)
            cashText.text = "$" + playerMoney.money.ToString("000000");
        else
            cashText.text = "$000000";

        // ❤️ Update health display
        if (playerHealth != null)
            healthText.text = "HEALTH: " + playerHealth.currentHealth.ToString("000");
        else
            healthText.text = "HEALTH: ---";
    }
    public void UpdateHUDNow()
    {
        // Instantly refresh money display
        if (playerMoney != null)
            cashText.text = "$" + playerMoney.money.ToString("000000");

        // Instantly refresh health display
        if (playerHealth != null)
            healthText.text = "HEALTH: " + playerHealth.currentHealth.ToString("000");
    }


    void UpdateTimeDisplay()
    {
        // ⏰ Simple in-game clock
        gameTime += Time.deltaTime;
        int hours = (int)(gameTime / 60) % 24;
        int minutes = (int)(gameTime % 60);
        timeText.text = $"{hours:00}:{minutes:00}";
    }

    // Optional external calls
    public void SetHealth(int newHealth)
    {
        if (playerHealth != null)
            playerHealth.currentHealth = Mathf.Clamp(newHealth, 0, playerHealth.maxHealth);
    }

    public void AddCash(int amount)
    {
        if (playerMoney != null)
            playerMoney.AddMoney(amount);
    }
}
