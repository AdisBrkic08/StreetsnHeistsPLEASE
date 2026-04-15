using UnityEngine;

public class MissionTarget : MonoBehaviour
{
    private MissionManager manager;

    void Start()
    {
        // Automatically finds the MissionManager in your scene
        manager = Object.FindAnyObjectByType<MissionManager>();
    }

    // This runs when the NPC is killed/destroyed
    void OnDestroy()
    {
        if (manager != null)
        {
            manager.TargetKilled();
        }
    }
}