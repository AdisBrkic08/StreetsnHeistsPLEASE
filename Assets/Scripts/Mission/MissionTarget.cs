using UnityEngine;

public class MissionTarget : MonoBehaviour
{
    private bool hasSentSignal = false;

    // This triggers when the NPC is "killed" or the object is turned off
    void OnDisable()
    {
        // Only run this if the game is actually playing
        if (!gameObject.scene.isLoaded) return;

        SendSignal();
    }

    // Safety backup: also try on Destroy
    void OnDestroy()
    {
        SendSignal();
    }

    void SendSignal()
    {
        if (hasSentSignal) return;

        MissionManager manager = Object.FindFirstObjectByType<MissionManager>();

        if (manager != null)
        {
            Debug.Log("<color=green>NPC: Manager found! Sending signal...</color>");
            manager.TargetKilled();
            hasSentSignal = true;
        }
        else
        {
            Debug.LogError("<color=red>NPC: COULD NOT FIND MISSION MANAGER IN SCENE!</color>");
        }
    }
}