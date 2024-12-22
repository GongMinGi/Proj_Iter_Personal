using UnityEngine;

public class Slugmove : MonoBehaviour
{
    public float speed = 2f;  // 이동 속도
    public float detectionRange = 5f;  // 타겟 탐지 범위
    public float stopDistance = 1.5f;  // 멈추는 거리
    public GameObject projectilePrefab;  // 발사체 프리팹
    public float projectileSpeed = 5f;  // 발사체 속도
    public float shootCooldown = 2f;  // 발사 간격 (초 단위)

    public Rigidbody2D target;  // 주인공 타겟

    private Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;
    private Animator animator;  // Animator 컴포넌트
    private float lastShootTime = 0f;  // 마지막 발사 시간

    private bool isMovingToTarget = false;  // 타겟에게 이동 중인지 여부
    private Vector3 lastSafePosition;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();  // Animator 컴포넌트 가져오기
        lastSafePosition = transform.position;
    }

    void FixedUpdate()
    {
        // 타겟이 탐지 범위 내에 들어오면 이동 시작
        if (Vector2.Distance(transform.position, target.position) <= detectionRange)
        {
            isMovingToTarget = true;
            animator.SetBool("IsWalking", true);  // Walk 상태로 전환
            MoveToTarget();
        }
        else
        {
            isMovingToTarget = false;
            animator.SetBool("IsWalking", false);  // Idle 상태로 전환
        }

        CheckForEdge();
    }

    private void MoveToTarget()
    {
        Vector2 dirVec = target.transform.position - transform.position;
        float distanceToTarget = dirVec.magnitude;

        if (distanceToTarget > stopDistance)
        {
            Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
            Vector3 nextPosition = transform.position + (Vector3)nextVec;

            if (!IsEdgeAhead(nextPosition))
            {
                transform.position = nextPosition;
                lastSafePosition = transform.position;
                spriteRenderer.flipX = dirVec.x < 0;
            }
            else
            {
                transform.position = lastSafePosition;
            }
        }

        animator.SetBool("IsWalking", true);  // 애니메이션 계속 유지

        if (distanceToTarget <= stopDistance && projectilePrefab != null)
        {
            ShootProjectile();
        }
    }

    private bool IsEdgeAhead(Vector3 position)
    {
        Vector2 frontVec = new Vector2(position.x + (spriteRenderer.flipX ? -0.5f : 0.5f), position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

        if (rayHit.collider == null)
        {
            Debug.Log("경고! 이 앞은 낭떨어지!!");
            return true;
        }
        return false;
    }

    private void CheckForEdge()
    {
        if (IsEdgeAhead(transform.position))
        {
            transform.position = lastSafePosition;
        }
    }

    private void ShootProjectile()
    {
        if (Time.time - lastShootTime >= shootCooldown)
        {
            lastShootTime = Time.time;  // 발사 시간 기록

            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                Vector2 direction = (target.transform.position - transform.position).normalized;
                direction.y += 0.5f;
                direction = direction.normalized;

                Vector2 force = direction * projectileSpeed;
                rb.AddForce(force, ForceMode2D.Impulse);
            }

            Projectile projectileScript = projectile.AddComponent<Projectile>();
            projectileScript.Initialize(target);
        }
    }
}

public class Projectile : MonoBehaviour
{
    private Rigidbody2D target;
    public float lifetime = 2f;

    private float creationTime;

    public void Initialize(Rigidbody2D target)
    {
        this.target = target;
        creationTime = Time.time;
    }

    void Update()
    {
        if (Time.time - creationTime >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Projectile hit the player!");
            Destroy(gameObject);
        }
    }
}
