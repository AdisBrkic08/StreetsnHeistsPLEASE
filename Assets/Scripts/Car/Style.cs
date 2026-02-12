using UnityEngine;
using UnityEngine.SceneManagement;

public class DrivingStyleSystem : MonoBehaviour
{
    [Header("Style Settings")]
    public float speedThreshold = 6f;
    public int cashPerSecond = 5;

    [Header("References")]
    public PlayerMoney playerMoney;
    public GameHUD hud;

    Rigidbody2D rb;
    float earnTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (playerMoney == null)
            playerMoney = FindFirstObjectByType<PlayerMoney>();

        if (hud == null)
            hud = FindFirstObjectByType<GameHUD>();
    }

    void Update()
    {
        // Only earn in gameplay scene
        if (SceneManager.GetActiveScene().name != "MainGame")
            return;

        // Stop if paused
        if (Time.timeScale == 0f)
            return;

        if (rb == null || playerMoney == null)
            return;

        float speed = Mathf.Min(rb.linearVelocity.magnitude, 20f);

        if (speed >= speedThreshold)
        {
            earnTimer += Time.deltaTime;

            if (earnTimer >= 1f)
            {
                GiveCash();
                earnTimer = 0f;
            }
        }
        else
        {
            earnTimer = 0f;
        }
    }

    void GiveCash()
    {
        playerMoney.AddMoney(cashPerSecond);

        if (hud != null)
            hud.UpdateHUDNow();
    }
}
