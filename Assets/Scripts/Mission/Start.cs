using UnityEngine;

public class MissionAutoStart : MonoBehaviour
{
    public MissionManager missionManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Start the mission immediately
            missionManager.ActivateMission();

            // Hide the glowing marker so it's gone from the world
            gameObject.SetActive(false);
        }
    }
}