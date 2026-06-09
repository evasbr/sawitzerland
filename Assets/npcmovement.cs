using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    public Transform target;
    public float speed = 2f;
    public float stopDistance = 1.5f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;

    private Vector2 startPosition;

    private bool goingBack = false;
    private bool waiting = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        startPosition = transform.position;
    }

    void Update()
    {
        if (!goingBack)
        {
            float distance = Vector2.Distance(transform.position, target.position);

            if (distance > stopDistance)
            {
                MoveTo(target.position);
            }
            else
            {
                rb.velocity = Vector2.zero;
                animator.Play("idle_owi");

                if (!waiting)
                {
                    waiting = true;
                    Invoke("StartGoingBack", 2f);
                }
            }
        }
        else
        {
            float distance = Vector2.Distance(transform.position, startPosition);

            if (distance > 0.1f)
            {
                MoveTo(startPosition);
            }
            else
            {
                rb.velocity = Vector2.zero;
                animator.Play("idle_owi");
            }
        }
    }

    void MoveTo(Vector2 targetPos)
    {
        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;

        rb.velocity = direction * speed;

        animator.Play("owi_walk");

        if (direction.x > 0)
            sr.flipX = false;
        else if (direction.x < 0)
            sr.flipX = true;
    }

    void StartGoingBack()
    {
        goingBack = true;
    }
}