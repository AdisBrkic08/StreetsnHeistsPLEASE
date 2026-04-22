using UnityEngine;

[CreateAssetMenu(fileName = "New Mission", menuName = "GTA/Mission Data")]
public class MissionData : ScriptableObject
{
    [Header("Mission Info")]
    public string missionName;
    [TextArea] public string description;

    [Header("Spawning")]
    public GameObject targetPrefab;
    public Vector3 spawnPosition;
}