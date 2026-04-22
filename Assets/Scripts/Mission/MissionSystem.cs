using UnityEngine;
using System.Collections;
using TMPro;

public class MissionManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject missionDescriptionUI;
    [SerializeField] private GameObject MissionPassed; // Drag your MissionPassed object here

    [Header("Animation Settings")]
    [SerializeField] private float targetScale = 6f;
    [SerializeField] private float displayDuration = 3f;

    private MissionData currentData;
    private GameObject currentTargetNPC;
    private GameObject currentTriggerObject;
    private bool isMissionRunning = false;

    void Awake()
    {
        if (missionDescriptionUI != null) missionDescriptionUI.SetActive(false);

        // Ensure MissionPassed starts disabled and visible scale
        if (MissionPassed != null)
        {
            MissionPassed.SetActive(false);
            MissionPassed.transform.localScale = new Vector3(targetScale, targetScale, 1);
        }
    }

    public void ActivateMission(MissionData data, GameObject trigger)
    {
        if (isMissionRunning) return;

        Debug.Log("Mission Started: " + data.missionName);
        isMissionRunning = true;
        currentData = data;
        currentTriggerObject = trigger;

        if (currentData.targetPrefab != null)
        {
            currentTargetNPC = Instantiate(currentData.targetPrefab, currentData.spawnPosition, Quaternion.identity);
            currentTargetNPC.name = "Mission_Target";
        }

        if (missionDescriptionUI != null) missionDescriptionUI.SetActive(true);
        if (currentTriggerObject != null) currentTriggerObject.SetActive(false);
    }

    public void TargetKilled()
    {
        Debug.Log("TargetKilled signal received by MissionManager!");

        if (isMissionRunning)
        {
            Debug.Log("Mission Success! Attempting to show MissionPassed UI.");
            isMissionRunning = false;

            if (MissionPassed != null)
            {
                // FORCE ACTIVE TRUE
                MissionPassed.SetActive(true);
                Debug.Log("MissionPassed.SetActive(true) called successfully.");

                StartCoroutine(SimpleUIAnimation());
            }
            else
            {
                Debug.LogError("MissionPassed object is MISSING in the Inspector!");
            }
        }
        else
        {
            Debug.LogWarning("Target died, but isMissionRunning was already false.");
        }
    }

    IEnumerator SimpleUIAnimation()
    {
        // Set scale to 0 first to pop it in
        MissionPassed.transform.localScale = Vector3.zero;

        float elapsed = 0;
        float duration = 0.4f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float s = Mathf.Lerp(0, targetScale, elapsed / duration);
            MissionPassed.transform.localScale = new Vector3(s, s, 1);
            yield return null;
        }

        yield return new WaitForSeconds(displayDuration);

        MissionPassed.SetActive(false);
        Debug.Log("MissionPassed UI auto-hidden after duration.");
    }

    public void FailMission()
    {
        Debug.Log("Mission Failed due to player death.");
        isMissionRunning = false;
        if (currentTargetNPC != null) Destroy(currentTargetNPC);
        if (missionDescriptionUI != null) missionDescriptionUI.SetActive(false);
        if (MissionPassed != null) MissionPassed.SetActive(false);
        if (currentTriggerObject != null) currentTriggerObject.SetActive(true);
    }
}