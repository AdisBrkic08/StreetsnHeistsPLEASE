using UnityEngine;
using System.Collections;
using TMPro;

public class MissionManager : MonoBehaviour
{
    [Header("UI Slots (Set these ONCE in the Hierarchy)")]
    public GameObject descriptionPanel;   // The UI background
    public TextMeshProUGUI descriptionText; // The actual TextMeshPro component
    public GameObject timerPanel;
    public TextMeshProUGUI timerText;
    public GameObject MissionPassed;

    private MissionData currentData;
    private GameObject currentTargetNPC;
    private GameObject currentTriggerObject;
    private bool isMissionRunning = false;
    private float timeRemaining;

    public void ActivateMission(MissionData data, GameObject trigger)
    {
        if (isMissionRunning) return;

        isMissionRunning = true;
        currentData = data;
        currentTriggerObject = trigger;

        // --- THE MAGIC LINE ---
        // This takes the "Description" you wrote in the MissionData file
        // and puts it into the UI Text component in your scene.
        if (descriptionText != null && descriptionPanel != null)
        {
            descriptionText.text = data.description;
            descriptionPanel.SetActive(true);
            Invoke("HideDescription", 6f);
        }

        // Handle Mission Types
        if (data.type == MissionType.EliminateTarget)
        {
            if (data.targetPrefab != null)
                currentTargetNPC = Instantiate(data.targetPrefab, data.spawnPosition, Quaternion.identity);
        }
        else if (data.type == MissionType.SurviveTime)
        {
            timeRemaining = data.survivalTime;
            if (timerPanel != null) timerPanel.SetActive(true);
            StartCoroutine(SurvivalTimer());
        }

        if (currentTriggerObject != null) currentTriggerObject.SetActive(false);
        if (HeatManager.Instance != null) HeatManager.Instance.heatLevel = data.wantedLevelToSet;
    }

    IEnumerator SurvivalTimer()
    {
        while (timeRemaining > 0)
        {
            if (!isMissionRunning) yield break;

            // Lock Stars: If player loses cops, force them back to 3 (or whatever is in Data)
            if (HeatManager.Instance != null && HeatManager.Instance.heatLevel < currentData.wantedLevelToSet)
            {
                HeatManager.Instance.heatLevel = currentData.wantedLevelToSet;
            }

            timeRemaining -= Time.deltaTime;
            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(timeRemaining / 60);
                int seconds = Mathf.FloorToInt(timeRemaining % 60);
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
            yield return null;
        }
        CompleteMission();
    }

    public void TargetKilled()
    {
        if (isMissionRunning && currentData.type == MissionType.EliminateTarget)
            CompleteMission();
    }

    void CompleteMission()
    {
        isMissionRunning = false;
        StopAllCoroutines();
        if (MissionPassed != null) MissionPassed.SetActive(true);
        if (timerPanel != null) timerPanel.SetActive(false);
        if (HeatManager.Instance != null) HeatManager.Instance.heatLevel = 0;
        Invoke("HidePassedUI", 4f);
    }

    public void FailMission()
    {
        if (isMissionRunning)
        {
            isMissionRunning = false;
            StopAllCoroutines();
            if (currentTargetNPC != null) Destroy(currentTargetNPC);
            if (descriptionPanel != null) descriptionPanel.SetActive(false);
            if (timerPanel != null) timerPanel.SetActive(false);
            if (currentTriggerObject != null) currentTriggerObject.SetActive(true);
        }
    }

    void HideDescription() => descriptionPanel.SetActive(false);
    void HidePassedUI() => MissionPassed.SetActive(false);
}