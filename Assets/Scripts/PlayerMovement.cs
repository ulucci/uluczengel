using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 2f;
    public float runSpeed = 6f;

    public float doubleClickDelay = 0.3f;

    private Vector3 targetPosition;

    private bool moving;
    private bool running;

    private float lastClickTime;

    private bool touchingWall;
    private Vector2 wallNormal;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        targetPosition = transform.position;

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        HandleInput();
        MovePlayer();
        UpdateAnimations();
    }

    void HandleInput()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Vector3 newTarget = new Vector3(mousePos.x, transform.position.y, 0);

        if (touchingWall)
        {
            Vector2 newDir = (newTarget - transform.position).normalized;
            if (Vector2.Dot(newDir, wallNormal) < 0) return;
        }

        running = Time.time - lastClickTime <= doubleClickDelay;
        lastClickTime = Time.time;
        targetPosition = newTarget;
        moving = true;
    }

    void MovePlayer()
    {
        if (!moving) return;

        float speed = running ? runSpeed : walkSpeed;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        if (targetPosition.x > transform.position.x)
            spriteRenderer.flipX = false;
        else if (targetPosition.x < transform.position.x)
            spriteRenderer.flipX = true;

        if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            moving = false;
            running = false;
        }
    }

    void UpdateAnimations()
    {
        animator.SetBool("IsWalking", moving);
        animator.SetBool("IsRunning", moving && running);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        touchingWall = true;
        wallNormal = collision.contacts[0].normal;
        moving = false;
        running = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        wallNormal = collision.contacts[0].normal;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        touchingWall = false;
    }
}
