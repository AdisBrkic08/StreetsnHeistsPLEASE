using UnityEngine;

public class CarInteraction : MonoBehaviour
{
    [Header("Car References")]
    public SimpleCarController2D carController;
    public CarBomb carBomb;
    public CarLights carLights; // Reference to lights script

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

        if (carLights != null)
            carLights.lightsOn = false; // Ensure lights start OFF

        carController.enabled = false;
    }

    void Update()
    {
        // 🚗 Exit while driving
        if (isPlayerDriving && Input.GetKeyDown(interactKey))
        {
            ExitCar();
            return;
        }

        // 🚶 Enter car if nearby
        if (!isPlayerDriving && playerNearby && Input.GetKeyDown(interactKey))
        {
            EnterCar();
        }
    }

    void EnterCar()
    {
        if (player == null) return;

        isPlayerDriving = true;

        // Disable player controls
        PlayerController2D controller = player.GetComponent<PlayerController2D>();
        if (controller != null)
            controller.enabled = false;

        PlayerShooter2D shooter = player.GetComponent<PlayerShooter2D>();
        if (shooter != null)
            shooter.canShoot = false;

        // Hide player sprite
        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.enabled = false;

        // Move player into car
        player.transform.position = transform.position;

        // Enable car controller
        carController.enabled = true;

        // Arm bomb if installed
        if (carBomb != null && carBomb.bombInstalled)
            carBomb.ArmBomb();

        // Enable lights input
        if (carLights != null)
            carLights.isDriving = true;

        // Switch camera
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

        // Restore player controls
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

        // Switch camera back
        CameraFollow2D cam = Camera.main.GetComponent<CameraFollow2D>();
        if (cam != null)
            cam.target = player.transform;

        // Disable lights input
        if (carLights != null)
        {
            carLights.isDriving = false; // Prevent toggling when not driving
            carLights.lightsOn = false;  // Optionally turn lights off
        }
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

        if (!isPlayerDriving)
        {
            playerNearby = false;
            player = null;
        }
    }
}
