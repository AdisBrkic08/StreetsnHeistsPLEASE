using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 0, -10);

    [Header("Smoothing")]
    [Range(0, 1)] public float smoothTime = 0.15f; // Lower is faster/snappier
    private Vector3 currentVelocity = Vector3.zero;

    [Header("Driving Lead")]
    public float leadAmount = 4f;
    public float zoomSpeed = 2f;
    public float baseZoom = 5f;
    public float maxZoom = 8f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        // Ensure camera starts exactly at the target to prevent a big snap
        if (target != null) transform.position = target.position + offset;
    }

    // FixedUpdate is better for following physics-based cars
    void FixedUpdate()
    {
        if (target == null) return;

        Rigidbody2D rb = target.GetComponent<Rigidbody2D>();
        Vector3 targetLead = Vector3.zero;

        if (rb != null)
        {
            // Calculate lead based on velocity
            targetLead = (Vector3)rb.linearVelocity * 0.4f;
            targetLead = Vector3.ClampMagnitude(targetLead, leadAmount);

            // Smooth Zoom logic
            float speedFactor = rb.linearVelocity.magnitude / 20f;
            float targetZoom = Mathf.Lerp(baseZoom, maxZoom, speedFactor);
            cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, targetZoom, zoomSpeed * Time.fixedDeltaTime);
        }

        // The target position we want to be at
        Vector3 destination = target.position + offset + targetLead;

        // SmoothDamp is much smoother than Lerp for high-speed following
        transform.position = Vector3.SmoothDamp(transform.position, destination, ref currentVelocity, smoothTime);
    }
}