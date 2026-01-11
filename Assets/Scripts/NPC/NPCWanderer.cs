using UnityEngine;

public class NPCWander : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float changeDirectionTime = 2f;

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private float timer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        PickNewDirection();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
            PickNewDirection();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * moveSpeed;
    }

    void PickNewDirection()
    {
        moveDirection = Random.insideUnitCircle.normalized;
        timer = changeDirectionTime;
    }
}
