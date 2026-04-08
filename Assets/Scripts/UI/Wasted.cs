using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class WastedSystem : MonoBehaviour
{
    [Header("References")]
    public GameObject wastedOverlay;  // UI Image (PNG/JPG)
    public Transform medicalCenter;   // Respawn location
    public PlayerHealth playerHealth;

    [Header("Settings")]
    public float fadeInTime = 1.5f;
    public float displayTime = 2f;
    public float fadeOutTime = 1f;

    [Header("Visual Effects")]
    public GameObject greyVolume; // Drag your DeathVolume here

    private bool isWasted = false;
    public delegate void WastedAction();
    public static event WastedAction OnPlayerWasted;
    void Update()
    {
        if (!isWasted && playerHealth != null && playerHealth.currentHealth <= 0)
        {
            StartCoroutine(DoWasted());
        }
    }

    IEnumerator DoWasted()
    {
        isWasted = true;

        // 1. FREEZE TIME
        // This stops all movement, physics, and NPC AI
        Time.timeScale = 0f;

        // TURN ON THE GREY EFFECT
        if (greyVolume != null) greyVolume.SetActive(true);

        // Show overlay
        wastedOverlay.SetActive(true);
        CanvasGroup cg = wastedOverlay.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            cg = wastedOverlay.AddComponent<CanvasGroup>();
        }
        cg.alpha = 0f;

        // 2. FADE IN (Use unscaledDeltaTime because time is frozen!)
        float t = 0f;
        while (t < fadeInTime)
        {
            t += Time.unscaledDeltaTime; // Important: use unscaled!
            cg.alpha = Mathf.Clamp01(t / fadeInTime);
            yield return null;
        }

        // 3. WAIT (Use WaitForSecondsRealtime because time is frozen!)
        yield return new WaitForSecondsRealtime(displayTime);

        // 4. FADE OUT
        t = 0f;
        while (t < fadeOutTime)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Clamp01(1 - (t / fadeOutTime));
            yield return null;
        }

        wastedOverlay.SetActive(false);

        // 5. UNFREEZE TIME
        Time.timeScale = 1f;

        // Respawn player
        RespawnPlayer();
        isWasted = false;
    }
    void RespawnPlayer()
    {
        if (playerHealth != null)
        {
            // 1. Reset Heat/Wanted Level
            if (HeatManager.Instance != null)
            {
                HeatManager.Instance.heatLevel = 0;
                HeatManager.Instance.currentScore = 0;
                Debug.Log("[WastedSystem] Heat Level Reset to 0.");
            }

            // TURN OFF THE GREY EFFECT
            if (greyVolume != null) greyVolume.SetActive(false);

            // 2. Clear out existing police cars so the area is safe
            GameObject[] police = GameObject.FindGameObjectsWithTag("PoliceVehicle");
            foreach (GameObject cop in police)
            {
                Destroy(cop);
            }

            // 3. Reset Player Health and State
            playerHealth.ResetPlayer();
            playerHealth.transform.SetParent(null); // Detach from any wreckage

            // 4. Move player to Hospital
            if (medicalCenter != null)
                playerHealth.transform.position = medicalCenter.position;

            // 5. Update HUD
            GameHUD hud = Object.FindFirstObjectByType<GameHUD>();
            if (hud != null)
                hud.UpdateHUDNow();

            if (OnPlayerWasted != null) OnPlayerWasted();
        }
    }
}
