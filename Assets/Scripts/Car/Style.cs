using UnityEngine;

public class DrivingStyleSystem : MonoBehaviour
{
    [Header("Style Settings")]
    public float speedThreshold = 6f;     // Speed to start earning
    public int cashPerSecond = 5;         // Reward rate

    [Header("References")]
    public PlayerMoney playerMoney;
    public GameHUD hud;

    Rigidbody2D rb;

    float earnTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (playerMoney == null)
            playerMoney = FindAnyObjectByType<PlayerMoney>();

        if (hud == null)
            hud = FindAnyObjectByType<GameHUD>();
    }

    void Update()
    {
        if (rb == null || playerMoney == null) return;

        float speed = rb.linearVelocity.magnitude;

        // Only earn when going fast
        if (speed >= speedThreshold)
        {
            earnTimer += Time.deltaTime;

            // Every 1 second → give money
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

        Debug.Log("STYLE CASH +$" + cashPerSecond);
    }
}
