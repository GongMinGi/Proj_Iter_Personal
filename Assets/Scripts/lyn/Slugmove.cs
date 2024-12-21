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

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();  // Animator 컴포넌트 가져오기
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
    }

    private void MoveToTarget()
    {
        // 타겟 방향으로 이동
        Vector2 dirVec = target.transform.position - transform.position; // 수정된 부분
        float distanceToTarget = dirVec.magnitude;  // 타겟과의 거리 계산

        if (distanceToTarget > stopDistance)  // 멈추기 전까지 이동
        {
            Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
            transform.position += (Vector3)nextVec;  // 타겟 방향으로 이동

            spriteRenderer.flipX = dirVec.x < 0;  // 타겟의 방향에 맞게 스프라이트 반전
        }

        // 계속 Walk 애니메이션 유지
        animator.SetBool("IsWalking", true);

        // 타겟과의 거리가 일정 거리 이하로 가까워지면 발사체를 날린다
        if (distanceToTarget <= stopDistance && projectilePrefab != null)
        {
            ShootProjectile();
        }
    }

    // 발사체를 발사하는 함수
    private void ShootProjectile()
    {
        // 마지막 발사 시간이 shootCooldown 이상 지난 경우에만 발사
        if (Time.time - lastShootTime >= shootCooldown)
        {
            lastShootTime = Time.time;  // 발사 시간 기록

            // 발사체를 생성하고 주인공을 향해 날리기
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                // 주인공을 향하는 기본 방향 계산
                Vector2 direction = (target.transform.position - transform.position).normalized;

                // y값을 추가하여 발사체가 위로 향하도록 수정
                direction.y += 0.5f;  // y값을 증가시켜 더 위로 향하게

                // 방향을 다시 정규화하여 벡터 크기를 1로 조정
                direction = direction.normalized;

                // 발사체에 적용할 힘 계산
                Vector2 force = direction * projectileSpeed;

                // 즉각적인 힘을 발사체에 적용
                rb.AddForce(force, ForceMode2D.Impulse);
            }

            // 발사체가 주인공에게 충돌하면 디버그 메시지 출력
            Projectile projectileScript = projectile.AddComponent<Projectile>();
            projectileScript.Initialize(target);  // 타겟 전달
        }
    }
    
}


public class Projectile : MonoBehaviour
{
    private Rigidbody2D target;
    public float lifetime = 2f;  // 발사체의 생명 주기 (2초 후에 삭제)

    private float creationTime;

    // 발사체 초기화 함수
    public void Initialize(Rigidbody2D target)
    {
        this.target = target;
        creationTime = Time.time;  // 발사체가 생성된 시간 기록
    }

    // Update는 매 프레임마다 호출되므로, 이곳에서 시간이 경과한 후 발사체를 삭제할 수 있음
    void Update()
    {
        if (Time.time - creationTime >= lifetime)  // lifetime이 지나면 삭제
        {
            Destroy(gameObject);
        }
    }

    // 충돌 처리
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            // 주인공과 충돌 시 디버그 메시지 출력
            Debug.Log("Projectile hit the player!");

            // 충돌 후 발사체 삭제
            Destroy(gameObject);
        }
    }
}

