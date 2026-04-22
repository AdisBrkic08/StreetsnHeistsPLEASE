using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class WastedSystem : MonoBehaviour
{
    [Header("References")]
    public GameObject wastedOverlay;
    public Transform medicalCenter;
    public PlayerHealth playerHealth;

    [Header("Settings")]
    public float fadeInTime = 1.5f;
    public float displayTime = 2f;
    public float fadeOutTime = 1f;

    [Header("Visual Effects")]
    public GameObject greyVolume;

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
        Time.timeScale = 0f;

        if (greyVolume != null) greyVolume.SetActive(true);

        wastedOverlay.SetActive(true);
        CanvasGroup cg = wastedOverlay.GetComponent<CanvasGroup>();
        if (cg == null) cg = wastedOverlay.AddComponent<CanvasGroup>();

        cg.alpha = 0f;

        float t = 0f;
        while (t < fadeInTime)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Clamp01(t / fadeInTime);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(displayTime);

        t = 0f;
        while (t < fadeOutTime)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Clamp01(1 - (t / fadeOutTime));
            yield return null;
        }

        wastedOverlay.SetActive(false);
        Time.timeScale = 1f;

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
            }

            if (greyVolume != null) greyVolume.SetActive(false);

            // 2. Clear out police
            GameObject[] police = GameObject.FindGameObjectsWithTag("PoliceVehicle");
            foreach (GameObject cop in police) Destroy(cop);

            // --- MISSION RESET LOGIC ---
            // This finds your MissionManager and tells it the player failed
            MissionManager mission = Object.FindFirstObjectByType<MissionManager>();
            if (mission != null)
            {
                mission.FailMission();
            }
            // ---------------------------

            // 3. Reset Player Health and State
            playerHealth.ResetPlayer();
            playerHealth.transform.SetParent(null);

            // 4. Move player to Hospital
            if (medicalCenter != null)
                playerHealth.transform.position = medicalCenter.position;

            // 5. Update HUD
            GameHUD hud = Object.FindFirstObjectByType<GameHUD>();
            if (hud != null) hud.UpdateHUDNow();

            if (OnPlayerWasted != null) OnPlayerWasted();
        }
    }
}