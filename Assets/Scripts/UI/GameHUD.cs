using UnityEngine;
using TMPro;
using System.Collections;

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
    public float gameTime = 12 * 60f;

    // Animated cash
    int displayedCash = 0;
    Coroutine cashRoutine;

    void Start()
    {
        if (playerMoney == null)
            playerMoney = FindFirstObjectByType<PlayerMoney>();

        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<PlayerHealth>();

        displayedCash = playerMoney.money;
    }

    void Update()
    {
        UpdateTimeDisplay();
        UpdateHUD();
    }

    void UpdateHUD()
    {
        // 💰 Smooth cash animation
        if (playerMoney != null)
        {
            if (displayedCash != playerMoney.money)
            {
                if (cashRoutine != null)
                    StopCoroutine(cashRoutine);

                cashRoutine = StartCoroutine(AnimateCash(displayedCash, playerMoney.money));
            }
        }

        // ❤️ Health
        if (playerHealth != null)
            healthText.text = "HEALTH: " + playerHealth.currentHealth.ToString("000");
        else
            healthText.text = "HEALTH: ---";
    }

    IEnumerator AnimateCash(int from, int to)
    {
        float duration = 0.4f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;

            displayedCash = Mathf.RoundToInt(
                Mathf.Lerp(from, to, t / duration)
            );

            cashText.text = "$" + displayedCash.ToString("000000");

            yield return null;
        }

        displayedCash = to;
        cashText.text = "$" + displayedCash.ToString("000000");
    }

    public void UpdateHUDNow()
    {
        if (playerMoney != null)
            cashText.text = "$" + playerMoney.money.ToString("000000");
    }

    void UpdateTimeDisplay()
    {
        gameTime += Time.deltaTime;

        int hours = (int)(gameTime / 60) % 24;
        int minutes = (int)(gameTime % 60);

        timeText.text = $"{hours:00}:{minutes:00}";
    }

    public void SetHealth(int newHealth)
    {
        if (playerHealth != null)
            playerHealth.currentHealth =
                Mathf.Clamp(newHealth, 0, playerHealth.maxHealth);
    }

    public void AddCash(int amount)
    {
        if (playerMoney != null)
            playerMoney.AddMoney(amount);
    }

}
