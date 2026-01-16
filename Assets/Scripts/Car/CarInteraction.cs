using UnityEngine;

public class CarInteraction : MonoBehaviour
{
    [Header("Car References")]
    public SimpleCarController2D carController;
    public CarBomb carBomb;

    [Header("Settings")]
    public KeyCode interactKey = KeyCode.E;
    public Vector2 exitOffset = new Vector2(1.2f, 0f);

    bool playerNearby;
    bool isPlayerDriving;

    GameObject player;

    void Start()
    {
        if (carController == null)
            carController = GetComponent<SimpleCarController2D>();

        if (carBomb == null)
            carBomb = GetComponent<CarBomb>();

        carController.enabled = false;
    }

    void Update()
    {
        // 🚗 EXIT — allowed anytime while driving
        if (isPlayerDriving && Input.GetKeyDown(interactKey))
        {
            ExitCar();
            return;
        }

        // 🚶 ENTER — only if nearby
        if (!isPlayerDriving && playerNearby && Input.GetKeyDown(interactKey))
        {
            EnterCar();
        }
    }

    void EnterCar()
    {
        if (player == null) return;

        isPlayerDriving = true;

        // Disable player movement
        PlayerController2D controller = player.GetComponent<PlayerController2D>();
        if (controller != null)
            controller.enabled = false;

        // Disable shooting
        PlayerShooter2D shooter = player.GetComponent<PlayerShooter2D>();
        if (shooter != null)
            shooter.canShoot = false;

        // Hide player sprite
        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.enabled = false;

        // Move player into car
        player.transform.position = transform.position;

        // Enable car
        carController.enabled = true;

        // Arm bomb if installed
        if (carBomb != null && carBomb.bombInstalled)
            carBomb.ArmBomb();

        // Camera to car
        CameraFollow2D cam = Camera.main.GetComponent<CameraFollow2D>();
        if (cam != null)
            cam.target = transform;
    }

    void ExitCar()
    {
        if (player == null) return;

        isPlayerDriving = false;

        // Disable car
        carController.enabled = false;

        // Restore player
        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.enabled = true;

        PlayerController2D controller = player.GetComponent<PlayerController2D>();
        if (controller != null)
            controller.enabled = true;

        PlayerShooter2D shooter = player.GetComponent<PlayerShooter2D>();
        if (shooter != null)
            shooter.canShoot = true;

        // Place player beside car
        Vector3 offset = transform.right * exitOffset.x + transform.up * exitOffset.y;
        player.transform.position = transform.position + offset;

        // Camera back to player
        CameraFollow2D cam = Camera.main.GetComponent<CameraFollow2D>();
        if (cam != null)
            cam.target = player.transform;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerNearby = true;
        player = other.gameObject;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // 🚫 Do NOT clear player reference if driving
        if (!isPlayerDriving)
        {
            playerNearby = false;
            player = null;
        }
    }
}
