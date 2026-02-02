using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Vector2 moveInput;
    Rigidbody2D rb;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 720f; // degrees per second

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; // smooth physics
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        // Move player
        rb.linearVelocity = moveInput * moveSpeed;

        // Rotate player toward mouse smoothly
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 direction = (mousePos - transform.position);
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        float angle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(angle);

        // Keep player in screen bounds
        Vector3 screenBoundary = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        rb.position = new Vector2(
            Mathf.Clamp(rb.position.x, -screenBoundary.x, screenBoundary.x),
            Mathf.Clamp(rb.position.y, -screenBoundary.y, screenBoundary.y)
        );
    }
}
