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

    private bool isWasted = false;

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

        // Show overlay
        wastedOverlay.SetActive(true);
        CanvasGroup cg = wastedOverlay.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            cg = wastedOverlay.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
        }

        // Fade in
        float t = 0f;
        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Clamp01(t / fadeInTime);
            yield return null;
        }

        // Wait for display
        yield return new WaitForSeconds(displayTime);

        // Fade out
        t = 0f;
        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Clamp01(1 - (t / fadeOutTime));
            yield return null;
        }

        wastedOverlay.SetActive(false);

        // Respawn player
        RespawnPlayer();
        isWasted = false;
    }

    void RespawnPlayer()
    {
        if (playerHealth != null && medicalCenter != null)
        {
            playerHealth.currentHealth = playerHealth.maxHealth;

            // Update HUD
            GameHUD hud = Object.FindFirstObjectByType<GameHUD>();
            if (hud != null)
                hud.UpdateHUDNow();

            // Move player
            playerHealth.transform.position = medicalCenter.position;

            Debug.Log("[WastedSystem] Player respawned at medical center!");
        }
    }

}
