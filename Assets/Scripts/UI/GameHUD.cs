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
    public PlayerHealth playerHealth;

    [Header("Settings")]
    public float gameTime = 12 * 60f;

    int displayedCash = 0;
    Coroutine cashRoutine;

    void Start()
    {
        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<PlayerHealth>();

        displayedCash = MoneyManager.Instance.money;
        UpdateHUDNow();
    }

    void Update()
    {
        UpdateTimeDisplay();
        UpdateHealth();
        UpdateCash();
    }

    // ---------------- CASH ----------------

    void UpdateCash()
    {
        int realCash = MoneyManager.Instance.money;

        if (displayedCash != realCash)
        {
            if (cashRoutine != null)
                StopCoroutine(cashRoutine);

            cashRoutine = StartCoroutine(
                AnimateCash(displayedCash, realCash));
        }
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
        displayedCash = MoneyManager.Instance.money;
        cashText.text = "$" + displayedCash.ToString("000000");
    }

    // ---------------- HEALTH ----------------

    void UpdateHealth()
    {
        if (playerHealth != null)
            healthText.text = "HEALTH: " +
                playerHealth.currentHealth.ToString("000");
        else
            healthText.text = "HEALTH: ---";
    }

    // ---------------- TIME ----------------

    void UpdateTimeDisplay()
    {
        gameTime += Time.deltaTime;

        int hours = (int)(gameTime / 60) % 24;
        int minutes = (int)(gameTime % 60);

        timeText.text = $"{hours:00}:{minutes:00}";
    }

    // ---------------- PUBLIC HELPERS ----------------

    public void AddCash(int amount)
    {
        MoneyManager.Instance.AddMoney(amount);
    }

    public void SetHealth(int newHealth)
    {
        if (playerHealth != null)
            playerHealth.currentHealth =
                Mathf.Clamp(newHealth, 0, playerHealth.maxHealth);
    }
}