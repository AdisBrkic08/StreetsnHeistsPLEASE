using UnityEngine;

public enum MissionType { EliminateTarget, SurviveTime }

[CreateAssetMenu(fileName = "New Mission", menuName = "GTA/Mission Data")]
public class MissionData : ScriptableObject
{
    [Header("Mission Text (Unique for every mission)")]
    public string missionName;
    [TextArea(3, 10)]
    public string description; // This is where you write your unique text!

    [Header("Mission Type & Difficulty")]
    public MissionType type;
    public float survivalTime = 120f;
    public int wantedLevelToSet = 3;

    [Header("Target Settings (For Kill Missions)")]
    public GameObject targetPrefab;
    public Vector3 spawnPosition;
}