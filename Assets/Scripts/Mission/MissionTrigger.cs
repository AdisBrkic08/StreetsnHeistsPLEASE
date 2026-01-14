using UnityEngine;

public class MissionTrigger : MonoBehaviour
{
    [Header("Mission Settings")]
    public string missionName = "Eliminate the Target";
    [TextArea] public string missionDescription = "Find and eliminate the marked enemy.";

    private bool missionStarted = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (missionStarted) return;

        if (other.CompareTag("Player"))
        {
            missionStarted = true;
            MissionSystem.Instance.StartMission(missionName, missionDescription);
        }
    }
}
