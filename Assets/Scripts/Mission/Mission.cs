using UnityEngine;

public class MissionTrigger : MonoBehaviour
{
    public MissionData missionData;
    private MissionManager manager;

    void Start()
    {
        manager = Object.FindFirstObjectByType<MissionManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && manager != null)
        {
            manager.ActivateMission(missionData, this.gameObject);
        }
    }
}