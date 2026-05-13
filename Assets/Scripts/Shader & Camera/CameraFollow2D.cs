using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Targeting")]
    public Transform target;
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("Look Ahead")]
    public float lookAheadDistance = 6f;
    public float lookAheadSmooth = 3f;

    [Header("Dynamic Zoom")]
    public float minSize = 5f;
    public float maxSize = 10f;
    public float maxSpeedForZoom = 20f;

    private Camera cam;
    private SimpleCarController2D currentCarController;
    private Rigidbody2D carRb;
    private Vector3 lookAheadPos;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        // 1. Check if we are driving or on foot
        FindActivePlayer();

        if (target == null) return;

        // 2. Decide if we should look ahead
        // Only shift the camera if we are currently driving a car
        bool isDriving = (currentCarController != null && currentCarController.isDriving);

        Vector3 targetLookAhead = Vector3.zero;
        if (isDriving)
        {
            // Pushes camera in the direction the car is facing
            targetLookAhead = target.up * lookAheadDistance;
        }
        // If not driving, targetLookAhead stays (0,0,0), keeping camera centered on the person

        lookAheadPos = Vector3.Lerp(lookAheadPos, targetLookAhead, Time.deltaTime * lookAheadSmooth);

        // 3. Move Camera
        Vector3 desiredPosition = target.position + offset + lookAheadPos;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);

        // 4. Zoom Logic
        HandleZoom(isDriving);
    }

    void HandleZoom(bool isDriving)
    {
        float targetSize = minSize;

        if (isDriving && carRb != null)
        {
            float currentSpeed = carRb.linearVelocity.magnitude;
            float speedFactor = Mathf.InverseLerp(0, maxSpeedForZoom, currentSpeed);
            targetSize = Mathf.Lerp(minSize, maxSize, speedFactor);
        }

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime);
    }

    void FindActivePlayer()
    {
        // First, check if there is a car being driven
        SimpleCarController2D[] cars = FindObjectsByType<SimpleCarController2D>(FindObjectsSortMode.None);
        foreach (SimpleCarController2D car in cars)
        {
            if (car.isDriving)
            {
                target = car.transform;
                currentCarController = car;
                carRb = car.GetComponent<Rigidbody2D>();
                return; // Found a car, stop looking
            }
        }

        // If no car is being driven, find the player character
        // Make sure your character object is tagged "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
            currentCarController = null;
            carRb = null;
        }
    }
}