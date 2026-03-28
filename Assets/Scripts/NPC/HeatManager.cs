using UnityEngine;

public class HeatManager : MonoBehaviour
{
    // This allows other scripts to find this one easily
    public static HeatManager Instance;

    [Header("Heat Status")]
    public int heatLevel = 0; // 0 to 5 stars
    public float currentScore = 0f;
    public float scoreToNextLevel = 100f;

    [Header("Cooldown Settings")]
    public float idleCooldownRate = 5f; // Points lost per second when not committing crimes
    private float lastCrimeTime;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Update()
    {
        // Slowly drain heat if the player hasn't done anything bad for 10 seconds
        if (Time.time > lastCrimeTime + 10f && heatLevel > 0)
        {
            currentScore -= idleCooldownRate * Time.deltaTime;

            if (currentScore <= 0)
            {
                if (heatLevel > 0)
                {
                    heatLevel--;
                    currentScore = scoreToNextLevel * 0.8f; // Drop to 80% of the previous star
                    Debug.Log("Heat Level Dropped: " + heatLevel);
                }
            }
        }
    }

    // Call this using: HeatManager.Instance.ReportCrime(50f);
    public void ReportCrime(float points)
    {
        currentScore += points;
        lastCrimeTime = Time.time; // Reset the cooldown timer

        if (currentScore >= scoreToNextLevel && heatLevel < 5)
        {
            heatLevel++;
            currentScore = 0;
            Debug.Log("HEAT LEVEL UP: " + heatLevel);
        }
    }
}