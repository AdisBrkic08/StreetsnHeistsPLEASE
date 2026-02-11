using Mono.Cecil;
using UnityEngine;

public class CarInteraction : MonoBehaviour
{
    [Header("Car References")]
    public SimpleCarController2D carController;
    public CarBomb carBomb;
    public CarLights carLights; // Reference to lights script
    public DrivingStyleSystem styleSystem;


    [Header("Settings")]
    [SerializeField] GameObject playerChar;
    public KeyCode interactKey = KeyCode.E;
    public Vector2 exitOffset = new Vector2(1.2f, 0f);

    bool playerNearby;
    bool isPlayerDriving;
    [HideInInspector] public bool playerInRange;

    // Other
    private PlayerDriving playerDrivingScript;
    GameObject player;

    void Start()
    {
        playerDrivingScript = FindFirstObjectByType<PlayerDriving>();

        if (carController == null)
            carController = GetComponent<SimpleCarController2D>();

        if (carBomb == null)
            carBomb = GetComponent<CarBomb>();

        if (!styleSystem)
            styleSystem = GetComponent<DrivingStyleSystem>();

        if (carLights != null)
            carLights.lightsOn = false;

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

        // Always move the player to the vehicle (police pursuit purposes)
        if (isPlayerDriving)
        {
            player.transform.position = transform.position;
        }
    }

    void EnterCar()
    {
        isPlayerDriving = true;
        playerDrivingScript.isDriving = true;

        // Disable player scripts
        player.GetComponent<PlayerController2D>().enabled = false;
        player.GetComponent<PlayerShooter2D>().SetCanShoot(false);

        // Hide player sprite
        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
        if (sr) sr.enabled = false;

        // 🔒 Freeze player physics
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;   // ★ THIS kills the ghost clone
        }

        // Move player into car
        player.transform.position = transform.position;

        // Enable car
        carController.enabled = true;

        // Camera follows car
        CameraFollow2D cam = Camera.main.GetComponent<CameraFollow2D>();
        if (cam) cam.target = transform;

        void SetPlayerVisible(GameObject player, bool visible)
        {
            foreach (var r in player.GetComponentsInChildren<Renderer>())
            {
                r.enabled = visible;
            }
        }

        SetPlayerVisible(player, false);


    }


    void ExitCar()
    {
        isPlayerDriving = false;
        playerDrivingScript.isDriving = false;

        carController.enabled = false;

        // Restore player
        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
        if (sr) sr.enabled = true;

        PlayerController2D pc = player.GetComponent<PlayerController2D>();
        if (pc) pc.enabled = true;

        PlayerShooter2D shooter = player.GetComponent<PlayerShooter2D>();
        if (shooter) shooter.SetCanShoot(true);

        // 🔓 Restore physics
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb)
            rb.simulated = true;

        // Place player beside car
        player.transform.position = transform.position - transform.right * 1.2f;

        // Camera back to player
        CameraFollow2D cam = Camera.main.GetComponent<CameraFollow2D>();
        if (cam) cam.target = player.transform;

        void SetPlayerVisible(GameObject player, bool visible)
        {
            foreach (var r in player.GetComponentsInChildren<Renderer>())
            {
                r.enabled = visible;
            }
        }

        SetPlayerVisible(player, true);


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
