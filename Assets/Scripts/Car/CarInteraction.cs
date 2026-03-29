using UnityEngine;

public class CarInteraction : MonoBehaviour
{
    public SimpleCarController2D carController;
    public Speedbreaker speedbreaker;
    public RandomWalker aiWalker;

    public KeyCode interactKey = KeyCode.E;
    public Vector2 exitOffset = new Vector2(1.5f, 0f);

    public bool playerInRange;
    public bool isPlayerDriving;

    private PlayerDriving playerDrivingScript;
    private GameObject player;

    void Start()
    {
        playerDrivingScript = FindFirstObjectByType<PlayerDriving>();
        if (!carController) carController = GetComponent<SimpleCarController2D>();
        if (!speedbreaker) speedbreaker = GetComponent<Speedbreaker>();

        carController.isDriving = false;
        if (speedbreaker) speedbreaker.enabled = false;
    }

    void Update()
    {
        if (isPlayerDriving && Input.GetKeyDown(interactKey)) ExitCar();
        else if (!isPlayerDriving && playerInRange && Input.GetKeyDown(interactKey)) EnterCar();

        if (isPlayerDriving && player) player.transform.position = transform.position;
    }
    public void ForceExit()
    {
        if (!isPlayerDriving) return;

        // Reset the car and player status
        isPlayerDriving = false;
        if (playerDrivingScript) playerDrivingScript.isDriving = false;

        // Disable car controls and speedbreaker immediately
        carController.isDriving = false;
        if (speedbreaker) speedbreaker.enabled = false;

        if (player != null)
        {
            player.transform.SetParent(null); // Ensure player isn't destroyed with car

            // Re-enable player controls
            player.GetComponent<PlayerController2D>().enabled = true;
            player.GetComponent<PlayerShooter2D>()?.SetCanShoot(true);

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb) { rb.simulated = true; }

            // Make player visible again
            SetPlayerVisible(player, true);

            // Move camera back to player
            CameraFollow2D cam = Camera.main.GetComponent<CameraFollow2D>();
            if (cam) cam.target = player.transform;
        }
    }

    // You also need this helper function if you don't have it:
    void SetPlayerVisible(GameObject target, bool visible)
    {
        foreach (var r in target.GetComponentsInChildren<Renderer>())
        {
            r.enabled = visible;
        }
    }
    void EnterCar()
    {
        if (!player) player = GameObject.FindGameObjectWithTag("Player");
        isPlayerDriving = true;
        if (playerDrivingScript) playerDrivingScript.isDriving = true;

        // --- HEAT SYSTEM INTEGRATION ---
        // Reporting "Grand Theft Auto"
        if (HeatManager.Instance != null)
        {
            HeatManager.Instance.ReportCrime(50f); // 150 points for stealing a vehicle
            Debug.Log("Crime Reported: Grand Theft Auto");
        }

        if (aiWalker) aiWalker.isAIActive = false;
        carController.isDriving = true;
        if (speedbreaker) speedbreaker.enabled = true;

        TogglePlayerPhysics(false);
        Camera.main.GetComponent<CameraFollow2D>().target = transform;
    }

    void ExitCar()
    {
        isPlayerDriving = false;
        if (playerDrivingScript) playerDrivingScript.isDriving = false;

        carController.isDriving = false;
        if (speedbreaker) speedbreaker.enabled = false;

        TogglePlayerPhysics(true);
        player.transform.position = transform.position + (Vector3)(transform.right * exitOffset.x);
        Camera.main.GetComponent<CameraFollow2D>().target = player.transform;
    }

    void TogglePlayerPhysics(bool state)
    {
        player.GetComponent<PlayerController2D>().enabled = state;
        player.GetComponent<PlayerShooter2D>()?.SetCanShoot(state);
        player.GetComponent<Rigidbody2D>().simulated = state;
        foreach (var r in player.GetComponentsInChildren<Renderer>()) r.enabled = state;
    }


}