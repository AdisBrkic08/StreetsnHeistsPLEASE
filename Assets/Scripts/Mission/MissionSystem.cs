using UnityEngine;
using System.Collections;

public class MissionManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject missionDescriptionUI;
    [SerializeField] private RectTransform missionPassedRect;
    [SerializeField] private CanvasGroup missionPassedFade;

    [Header("Animation Settings")]
    [SerializeField] private float targetScale = 6f; // SET THIS TO 6 IN INSPECTOR
    [SerializeField] private float displayDuration = 4f; // How long it stays visible

    [Header("Mission Settings")]
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private Transform spawnPoint;

    private bool isMissionRunning = false;

    void Awake()
    {
        if (missionDescriptionUI != null) missionDescriptionUI.SetActive(false);

        if (missionPassedRect != null)
        {
            missionPassedRect.gameObject.SetActive(false);
            missionPassedRect.localScale = Vector3.zero;
        }
        if (missionPassedFade != null) missionPassedFade.alpha = 0;
    }

    public void ActivateMission()
    {
        if (!isMissionRunning)
        {
            isMissionRunning = true;
            if (npcPrefab != null && spawnPoint != null)
                Instantiate(npcPrefab, spawnPoint.position, Quaternion.identity);

            if (missionDescriptionUI != null) missionDescriptionUI.SetActive(true);
            Invoke("HideDescription", 5f);
        }
    }

    public void TargetKilled()
    {
        if (isMissionRunning)
        {
            isMissionRunning = false;
            StartCoroutine(AnimateMissionPassed());
        }
    }

    IEnumerator AnimateMissionPassed()
    {
        missionPassedRect.gameObject.SetActive(true);
        float animDuration = 0.5f;
        float elapsed = 0f;

        // 1. ANIMATE IN (Scale up to targetScale)
        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / animDuration;

            // Notice: We multiply by targetScale here!
            missionPassedRect.localScale = Vector3.Lerp(Vector3.zero, new Vector3(targetScale, targetScale, 1), percent);

            if (missionPassedFade != null) missionPassedFade.alpha = percent;
            yield return null;
        }

        // 2. WAIT (The text stays on screen)
        yield return new WaitForSeconds(displayDuration);

        // 3. ANIMATE OUT (Fade away)
        elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime;
            if (missionPassedFade != null) missionPassedFade.alpha = 1 - elapsed;
            yield return null;
        }

        missionPassedRect.gameObject.SetActive(false);
    }

    void HideDescription() => missionDescriptionUI.SetActive(false);
}