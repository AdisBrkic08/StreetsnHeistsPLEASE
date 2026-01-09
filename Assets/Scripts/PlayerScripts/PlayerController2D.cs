// PlayerController2D.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float sprintMultiplier = 1.6f;
    public bool useAnalog = false; // set true if using gamepad
    Rigidbody2D rb;
    Vector2 input;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    void Update()
    {
        // Using UnityEngine.Input for simplicity. Replace with new InputSystem as desired.
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        input = new Vector2(h, v).normalized;

        if (useAnalog)
        {
            // if you'd read analog stick, do not normalize to preserve magnitude
            // input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
    }

    void FixedUpdate()
    {
        float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1f);
        Vector2 target = rb.position + input * speed * Time.fixedDeltaTime;
        rb.MovePosition(target);
        // Optional: rotate sprite to face movement
        if (input.sqrMagnitude > 0.001f)
            transform.up = input;
    }
}
