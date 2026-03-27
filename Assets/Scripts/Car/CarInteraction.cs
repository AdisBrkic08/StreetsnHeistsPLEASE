using UnityEngine;

public class CarInteraction : MonoBehaviour
{
    [Header("Car References")]
    public SimpleCarController2D carController;
    public RandomWalker aiWalker; // Reference to the AI script
    public CarBomb carBomb;
    public CarLights carLights;
    public DrivingStyleSystem styleSystem;

    [Header("Settings")]
    public KeyCode interactKey = KeyCode.E;
    public Vector2 exitOffset = new Vector2(1.2f, 0f);

    [Header("Status")]
    public bool playerInRange; // Matches CarEnterTrigger.cs
    public bool isPlayerDriving;

    private PlayerDriving playerDrivingScript;
    private GameObject player;

    void Start()
    {
        playerDrivingScript = FindFirstObjectByType<PlayerDriving>();

        // Auto-assign references if they are missing
        if (carController == null) carController = GetComponent<SimpleCarController2D>();
        if (aiWalker == null) aiWalker = GetComponent<RandomWalker>();

        // Ensure car starts disabled (AI or Player will enable it)
        carController.enabled = false;

        if (carLights != null) carLights.lightsOn = false;
    }

    void Update()
    {
        // 🚗 EXIT Logic
        if (isPlayerDriving && Input.GetKeyDown(interactKey))
        {
            ExitCar();
            return;
        }

        // 🚶 ENTER Logic
        // We use playerInRange which is set by your CarEnterTrigger script
        if (!isPlayerDriving && playerInRange && Input.GetKeyDown(interactKey))
        {
            EnterCar();
        }

        // Keep player stuck to the car while driving
        if (isPlayerDriving && player != null)
        {
            player.transform.position = transform.position;
        }
    }

    void EnterCar()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player");

        isPlayerDriving = true;
        if (playerDrivingScript) playerDrivingScript.isDriving = true;

        // 1. Tell the AI to stop moving the car
        if (aiWalker) aiWalker.isAIActive = false;

        // 2. Enable the manual car controller
        carController.enabled = true;

        // 3. Disable Player logic/physics
        player.GetComponent<PlayerController2D>().enabled = false;

        // Use a null-check (?) in case player doesn't have a shooter script
        player.GetComponent<PlayerShooter2D>()?.SetCanShoot(false);

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        // 4. Visuals and Camera
        SetPlayerVisible(player, false);
        CameraFollow2D cam = Camera.main.GetComponent<CameraFollow2D>();
        if (cam) cam.target = transform;
    }

    void ExitCar()
    {
        isPlayerDriving = false;
        if (playerDrivingScript) playerDrivingScript.isDriving = false;

        // 1. Stop manual control
        carController.enabled = false;

        // 2. Optional: If you want the AI to take over again, uncomment this:
        // if (aiWalker) aiWalker.isAIActive = true;

        // 3. Restore Player
        player.GetComponent<PlayerController2D>().enabled = true;
        player.GetComponent<PlayerShooter2D>()?.SetCanShoot(true);

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb) rb.simulated = true;

        // Move player to the side of the car
        player.transform.position = transform.position - (transform.right * exitOffset.x);

        // 4. Visuals and Camera
        SetPlayerVisible(player, true);
        CameraFollow2D cam = Camera.main.GetComponent<CameraFollow2D>();
        if (cam) cam.target = player.transform;
    }

    // Helper to hide/show player and all child parts (arms, guns, etc)
    void SetPlayerVisible(GameObject target, bool visible)
    {
        foreach (var r in target.GetComponentsInChildren<Renderer>())
        {
            r.enabled = visible;
        }
    }

    // Backup Triggers (in case you don't use the separate Trigger script)
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            player = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}