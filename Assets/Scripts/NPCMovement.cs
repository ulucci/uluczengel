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
    private float idleTimer;

    private Vector3 target;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        ChooseNextPoint();
    }

    void Update()
    {
        if (isWalking)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                target,
                moveSpeed * Time.deltaTime
            );

            // yön
            if (target.x > transform.position.x)
            {
                spriteRenderer.flipX = false;
            }
            else
            {
                spriteRenderer.flipX = true;
            }

            // hedefe ulaþtý
            if (Vector2.Distance(transform.position, target) < 0.05f)
            {
                isWalking = false;

                animator.SetBool("isWalking", false);

                idleTimer = Random.Range(minIdleTime, maxIdleTime);
            }
        }
        else
        {
            idleTimer -= Time.deltaTime;

            if (idleTimer <= 0)
            {
                ChooseNextPoint();
            }
        }
    }

    void ChooseNextPoint()
    {
        // Sol ve sað sýnýrlar arasýnda rastgele nokta
        float randomX = Random.Range(
            leftPoint.position.x,
            rightPoint.position.x
        );

        target = new Vector3(
            randomX,
            transform.position.y,
            transform.position.z
        );

        isWalking = true;

        animator.SetBool("isWalking", true);
    }
}