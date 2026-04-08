using UnityEngine;

public class CarInteraction : MonoBehaviour
{
    [Header("Car References")]
    public SimpleCarController2D carController;
    public RandomWalker aiWalker;
    public CarLights carLights;

    [Header("Settings")]
    public KeyCode interactKey = KeyCode.E;
    public Vector2 exitOffset = new Vector2(1.5f, 0f);

    [Header("Status")]
    public bool playerInRange;
    public bool isPlayerDriving;

    private PlayerDriving playerDrivingScript;
    private GameObject player;

    void Start()
    {
        playerDrivingScript = FindFirstObjectByType<PlayerDriving>();
        if (carController == null) carController = GetComponent<SimpleCarController2D>();
        if (aiWalker == null) aiWalker = GetComponent<RandomWalker>();

        // Ensure car is off at start
        carController.enabled = false;
        carController.isDriving = false; // Added for the fix
    }

    void Update()
    {
        if (isPlayerDriving && Input.GetKeyDown(interactKey)) { ExitCar(); return; }
        if (!isPlayerDriving && playerInRange && Input.GetKeyDown(interactKey)) { EnterCar(); }

        if (isPlayerDriving && player != null)
        {
            player.transform.position = transform.position;
        }
    }

    public void ForceExit()
    {
        if (!isPlayerDriving) return;

        isPlayerDriving = false;
        if (playerDrivingScript) playerDrivingScript.isDriving = false;
        GetComponent<Speedbreaker>()?.ResetSpeedbreaker();

        // --- FIX: Kill car engine on force exit ---
        if (carController) carController.isDriving = false;

        if (player != null)
        {
            player.transform.SetParent(null);
            player.GetComponent<PlayerController2D>().enabled = true;

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb) { rb.simulated = true; }

            SetPlayerVisible(player, true);
            CameraFollow2D cam = Camera.main.GetComponent<CameraFollow2D>();
            if (cam) cam.target = player.transform;
        }
    }

    void EnterCar()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player");

        if (HeatManager.Instance != null)
        {
            HeatManager.Instance.ReportCrime(50f);
        }

        isPlayerDriving = true;
        if (playerDrivingScript) playerDrivingScript.isDriving = true;

        if (aiWalker) aiWalker.isAIActive = false;

        // --- FIX: Enable car engine control ---
        carController.enabled = true;
        carController.isDriving = true;

        player.GetComponent<PlayerController2D>().enabled = false;
        player.GetComponent<PlayerShooter2D>()?.SetCanShoot(false);

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb) { rb.simulated = false; }

        SetPlayerVisible(player, false);
        CameraFollow2D cam = Camera.main.GetComponent<CameraFollow2D>();
        if (cam) cam.target = transform;
    }

    void ExitCar()
    {
        isPlayerDriving = false;
        if (playerDrivingScript) playerDrivingScript.isDriving = false;

        // --- FIX: Disable car engine control ---
        carController.enabled = false;
        carController.isDriving = false;
        GetComponent<Speedbreaker>()?.ResetSpeedbreaker();

        player.GetComponent<PlayerController2D>().enabled = true;
        player.GetComponent<PlayerShooter2D>()?.SetCanShoot(true);

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb) rb.simulated = true;

        player.transform.position = transform.position + (Vector3)(transform.right * exitOffset.x);

        SetPlayerVisible(player, true);
        CameraFollow2D cam = Camera.main.GetComponent<CameraFollow2D>();
        if (cam) cam.target = player.transform;
    }

    void SetPlayerVisible(GameObject target, bool visible)
    {
        foreach (var r in target.GetComponentsInChildren<Renderer>()) { r.enabled = visible; }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) { playerInRange = true; player = other.gameObject; }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) { playerInRange = false; }
    }
}