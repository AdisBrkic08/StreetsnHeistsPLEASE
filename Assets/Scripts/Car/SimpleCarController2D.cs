using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SimpleCarController2D : MonoBehaviour
{
    public float acceleration = 30f;
    public float maxSpeed = 8f;
    public float turnSpeed = 200f;
    public float driftFactor = 0.98f;

    private Rigidbody2D rb;
    private float inputV;
    private float inputH;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        inputV = Input.GetAxis("Vertical");   // W/S or Up/Down
        inputH = Input.GetAxis("Horizontal"); // A/D or Left/Right
    }

    void FixedUpdate()
    {
        // Apply forward/backward acceleration
        Vector2 forward = transform.up;
        rb.AddForce(forward * (inputV * acceleration));

        // Cap top speed
        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;

        // Steering responsiveness scaling
        float speedFactor = Mathf.Lerp(0.5f, 1f, 1f - (rb.linearVelocity.magnitude / maxSpeed));
        float rotationAmount = inputH * turnSpeed * speedFactor * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation - rotationAmount);

        // Drift smoothing
        rb.linearVelocity =
            transform.up * Vector2.Dot(rb.linearVelocity, transform.up) +
            transform.right * Vector2.Dot(rb.linearVelocity, transform.right) * driftFactor;

        // Speed-based angular damping (replaces angularDrag)
        if (rb.linearVelocity.magnitude > maxSpeed * 0.7f)
            rb.angularDamping = 3f;
        else
            rb.angularDamping = 0.5f;
    }
}
