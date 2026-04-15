using UnityEngine;
using UnityEngine.UI; // Required for UI elements

public class MissionStartTrigger : MonoBehaviour
{
    public GameObject missionUIContainer; // The Parent object of your "Press E" text
    public MissionManager missionManager;
    private bool playerInRange = false;

    void Start()
    {
        // Make sure the text is hidden when the game starts
        if (missionUIContainer != null)
            missionUIContainer.SetActive(false);
    }

    void Update()
    {
        // Only allow the key press if the player is actually in the trigger
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            StartMission();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object entering is the Player
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (missionUIContainer != null)
                missionUIContainer.SetActive(true); // SHOW the text
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (missionUIContainer != null)
                missionUIContainer.SetActive(false); // HIDE the text
        }
    }

    void StartMission()
    {
        missionManager.ActivateMission();

        // Hide the trigger and text once the mission has officially started
        missionUIContainer.SetActive(false);
        gameObject.SetActive(false);
    }
}