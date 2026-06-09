using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator animator;
    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        animator.SetFloat("MoveX", movement.x);
        animator.SetFloat("MoveY", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        // flip kanan kiri
        if (movement.x > 0)
        {
            sr.flipX = false;
        }
        else if (movement.x < 0)
        {
            sr.flipX = true;
        }

        // arah animasi
        if (movement.x != 0)
        {
            animator.Play(
                movement.sqrMagnitude > 0
                ? "Walk_Side"
                : "Idle_Side"
            );
        }
        else
        {
            animator.Play(
                movement.sqrMagnitude > 0
                ? "Walk_Front"
                : "Idle_Front"
            );
        }
    }

    void FixedUpdate()
    {
        rb.velocity = movement.normalized * speed;
    }
}