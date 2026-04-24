using UnityEngine;
using System.Collections;
using TMPro;

public class MissionManager : MonoBehaviour
{
    [Header("UI Slots")]
    public GameObject descriptionPanel;
    public TextMeshProUGUI descriptionText;
    public GameObject timerPanel;
    public TextMeshProUGUI timerText;
    public GameObject MissionPassed;

    private MissionData currentData;
    private GameObject currentTargetNPC;
    private GameObject currentTriggerObject;
    private bool isMissionRunning = false;
    private float timeRemaining;

    void Update()
    {
        // THIS IS THE LOCK
        // If a mission is running, we force the heat level every frame
        if (isMissionRunning && currentData != null && HeatManager.Instance != null)
        {
            // This prevents the stars from going UP (when killing) 
            // or DOWN (when hiding)
            HeatManager.Instance.heatLevel = currentData.wantedLevelToSet;
        }
    }

    public void ActivateMission(MissionData data, GameObject trigger)
    {
        if (isMissionRunning) return;

        isMissionRunning = true;
        currentData = data;
        currentTriggerObject = trigger;

        // Display unique description from Data file
        if (descriptionText != null && descriptionPanel != null)
        {
            descriptionText.text = data.description;
            descriptionPanel.SetActive(true);
            Invoke("HideDescription", 6f);
        }

        // Setup Mission Logic
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
    }

    IEnumerator SurvivalTimer()
    {
        while (timeRemaining > 0)
        {
            if (!isMissionRunning) yield break;

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
        StopAllCoroutines(); // Stop the mission timer

        if (MissionPassed != null) MissionPassed.SetActive(true);
        if (timerPanel != null) timerPanel.SetActive(false);

        // 1. Reset the stars to 0
        if (HeatManager.Instance != null)
        {
            HeatManager.Instance.heatLevel = 0;
        }

        // 2. Start the "Clean Sweep" for 5 seconds
        StartCoroutine(CleanSweepPolice(10f));

        Invoke("HidePassedUI", 4f);
    }

    IEnumerator CleanSweepPolice(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // 1. Clean up everything tagged "NPC" (This includes your Cops)
            GameObject[] npcUnits = GameObject.FindGameObjectsWithTag("NPC");
            foreach (GameObject npc in npcUnits)
            {
                if (npc != null) Destroy(npc);
            }

            // 2. Clean up everything tagged "PoliceVehicle"
            GameObject[] policeVehicles = GameObject.FindGameObjectsWithTag("PoliceVehicle");
            foreach (GameObject vehicle in policeVehicles)
            {
                if (vehicle != null) Destroy(vehicle);
            }

            yield return null;
        }

        Debug.Log("Clean sweep finished: NPCs and PoliceVehicles cleared.");
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

            // Optional: Keep the heat or reset it on fail? 
            // Usually, GTA keeps the heat if you fail by dying.
        }
    }

    void HideDescription() => descriptionPanel.SetActive(false);
    void HidePassedUI() => MissionPassed.SetActive(false);
}