// PlayerController2D.cs
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float sprintMultiplier = 1.6f;
    public bool useAnalog = false; // set true if using gamepad

    Rigidbody2D rb;
    Vector2 input;
    Animator anim;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }


    void Update()
    {
        // Movement input
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        input = new Vector2(h, v).normalized;
      
        bool isMoving = input.sqrMagnitude > 0.01f;

        anim.SetBool("isWalking", isMoving);


        if (useAnalog)
        {
            // Optional analog input
            // input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }

        RotateTowardsMouse();
    }

    void FixedUpdate()
    {
        float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1f);
        Vector2 target = rb.position + input * speed * Time.fixedDeltaTime;
        rb.MovePosition(target);
    }

    // --------------------------------
    // Mouse-based player rotation
    // --------------------------------
    void RotateTowardsMouse()
    {
        if (Mouse.current == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 direction = mousePos - transform.position;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
    }
}
