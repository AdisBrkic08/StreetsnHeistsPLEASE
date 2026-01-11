using UnityEngine;

public class CarInteraction : MonoBehaviour
{
    public SimpleCarController2D carController;
    private bool playerNearby = false;
    private GameObject player;
    private bool isPlayerDriving = false;

    private CarLights headlights;

    void Start()
    {
        if (carController == null)
            carController = GetComponent<SimpleCarController2D>();

        // Get headlights from children
        headlights = GetComponentInChildren<CarLights>();

        carController.enabled = false;
        if (headlights) headlights.isDriving = false;
    }

    void Update()
    {
        if (!isPlayerDriving && playerNearby && Input.GetKeyDown(KeyCode.E))
            EnterCar();

        else if (isPlayerDriving && Input.GetKeyDown(KeyCode.E))
            ExitCar();
    }

    void EnterCar()
    {
        isPlayerDriving = true;

        // Disable player movement & sprite
        player.GetComponent<PlayerController2D>().enabled = false;
        var sr = player.GetComponent<SpriteRenderer>();
        if (sr) sr.enabled = false;

        // Disable the gun
        player.GetComponent<PlayerShooter2D>().SetCanShoot(false);

        player.transform.position = transform.position;

        // Enable car
        carController.enabled = true;

        // Enable headlights input
        if (headlights) headlights.isDriving = true;

        // Set camera
        var cam = Camera.main.GetComponent<CameraFollow2D>();
        if (cam) cam.target = transform;

        var shake = Camera.main.GetComponent<CameraSpeedShake>();
        if (shake) shake.SetCar(carController.GetComponent<Rigidbody2D>());

    }

    void ExitCar()
    {
        isPlayerDriving = false;

        // Disable car
        carController.enabled = false;

        // Disable headlight input
        if (headlights) headlights.isDriving = false;

        // Re-enable player
        var sr = player.GetComponent<SpriteRenderer>();
        if (sr) sr.enabled = true;
        player.GetComponent<PlayerController2D>().enabled = true;

        player.transform.position = transform.position - transform.right * 1.2f;
        player.GetComponent<PlayerShooter2D>().SetCanShoot(true);


        // Set camera
        var cam = Camera.main.GetComponent<CameraFollow2D>();
        if (cam) cam.target = player.transform;

        var shake = Camera.main.GetComponent<CameraSpeedShake>();
        if (shake) shake.ClearCar();

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            player = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }
}
