using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    public Transform leftPoint;
    public Transform rightPoint;

    public float moveSpeed = 1f;
    public float minIdleTime = 1f;
    public float maxIdleTime = 4f;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isWalking;
    private bool isPaused;
    private bool isLeaving;
    private float idleTimer;
    private Vector3 target;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        ChooseNextPoint();
    }

    public void PauseForDialogue(Transform player)
    {
        isPaused = true;
        isWalking = false;
        animator.SetBool("isWalking", false);
        spriteRenderer.flipX = player.position.x < transform.position.x;
    }

    public void ResumeFromDialogue()
    {
        isPaused = false;
        idleTimer = Random.Range(minIdleTime, maxIdleTime);
    }

    void Update()
    {
        if (isPaused) return;

        if (isWalking)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                target,
                moveSpeed * Time.deltaTime
            );

            spriteRenderer.flipX = target.x < transform.position.x;

            if (Vector2.Distance(transform.position, target) < 0.05f)
            {
                isWalking = false;
                animator.SetBool("isWalking", false);
                if (isLeaving) { gameObject.SetActive(false); return; }
                idleTimer = Random.Range(minIdleTime, maxIdleTime);
            }
        }
        else
        {
            idleTimer -= Time.deltaTime;
            if (idleTimer <= 0)
                ChooseNextPoint();
        }
    }

    [SerializeField] private Transform exitPoint;

    public void Expel(Transform exit)
    {
        isPaused = false;
        isLeaving = true;
        target = exit.position;
        isWalking = true;
        animator.SetBool("isWalking", true);
    }

    public void Expel()
    {
        if (exitPoint != null) Expel(exitPoint);
    }

    void ChooseNextPoint()
    {
        float randomX = Random.Range(leftPoint.position.x, rightPoint.position.x);
        target = new Vector3(randomX, transform.position.y, transform.position.z);
        isWalking = true;
        animator.SetBool("isWalking", true);
    }
}
