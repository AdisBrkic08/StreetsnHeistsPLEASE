using UnityEngine;

public class CarInteraction : MonoBehaviour
{
    [Header("Car References")]
    public SimpleCarController2D carController;
    public RandomWalker aiWalker;

    [Header("Settings")]
    public KeyCode interactKey = KeyCode.E;
    public Vector2 exitOffset = new Vector2(1.5f, 0f);

    [Header("Status")]
    public bool playerInRange;
    public bool isPlayerDriving;

    private GameObject player;
    private GameObject weaponHolder; // This is now private and found via code

    void Start()
    {
        if (carController == null) carController = GetComponent<SimpleCarController2D>();
        if (aiWalker == null) aiWalker = GetComponent<RandomWalker>();

        // Ensure car is off at start
        carController.enabled = false;
        carController.isDriving = false;
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

    void EnterCar()
    {
        // 1. Find the Player if we don't have them yet
        if (player == null) player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // 2. Automatically find the WeaponHolder inside the player
            // This searches all children for an object named "WeaponHolder"
            if (weaponHolder == null)
            {
                weaponHolder = FindChildByName(player.transform, "WeaponHolder");
            }

            // 3. Disable the weapons
            if (weaponHolder != null) weaponHolder.SetActive(false);

            // 4. Disable Movement
            var controller = player.GetComponent<PlayerController2D>();
            if (controller != null) controller.enabled = false;

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb) rb.simulated = false;

            SetPlayerVisible(player, false);
        }

        // 5. Enable Car
        isPlayerDriving = true;
        carController.enabled = true;
        carController.isDriving = true;
        if (aiWalker) aiWalker.isAIActive = false;

        if (HeatManager.Instance != null) HeatManager.Instance.ReportCrime(50f);

        // Camera
        CameraFollow2D cam = Camera.main.GetComponent<CameraFollow2D>();
        if (cam) cam.target = transform;
    }

    void ExitCar()
    {
        isPlayerDriving = false;
        carController.isDriving = false;
        carController.enabled = false;

        // 1. RE-ENABLE WEAPON HOLDER
        if (weaponHolder != null) weaponHolder.SetActive(true);

        // 2. Restore Player Movement
        if (player != null)
        {
            var controller = player.GetComponent<PlayerController2D>();
            if (controller != null) controller.enabled = true;

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb) rb.simulated = true;

            player.transform.position = transform.position + (Vector3)(transform.right * exitOffset.x);
            SetPlayerVisible(player, true);
        }

        // 3. Camera
        CameraFollow2D cam = Camera.main.GetComponent<CameraFollow2D>();
        if (cam) cam.target = player.transform;
    }

    public void ForceExit()
    {
        if (isPlayerDriving) ExitCar();
    }

    // Helper to find the WeaponHolder even if it's deeply nested
    GameObject FindChildByName(Transform parent, string name)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == name) return child.gameObject;
        }
        return null;
    }

    void SetPlayerVisible(GameObject target, bool visible)
    {
        foreach (var r in target.GetComponentsInChildren<Renderer>())
        {
            if (visible && weaponHolder != null && r.transform.IsChildOf(weaponHolder.transform))
                continue;
            r.enabled = visible;
        }
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