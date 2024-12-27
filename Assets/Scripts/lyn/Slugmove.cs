using UnityEngine;

public class Slugmove : BaseMonster
{
    public float speed = 2f;  // 슬러그의 이동 속도
    public float detectionRange = 5f;  //슬러그의 타겟 탐지 범위
    public float stopDistance = 1.5f;  // 타겟과의 거리가 이 값보다 작아지면 멈춤
    public GameObject projectilePrefab;  // 발사체 프리팹 (슬러그가 발사할 오브젝트)
    public float projectileSpeed = 5f;  // 발사체 속도
    public float shootCooldown = 2f;  // 발사 간격 (초 단위)

    public Rigidbody2D target;  // 타겟으로 설정된 플레이어의 Rigidbody

    //private Rigidbody2D rigid;  // 슬러그 자신의 rigid body
    private SpriteRenderer spriteRenderer;
    //private Animator animator;  // Animator 컴포넌트
    private float lastShootTime = 0f;  // 마지막 발사 시간

    private bool isMovingToTarget = false;  // 타겟에게 이동 중인지 여부
    private bool isAttacking = false;
    private Vector3 lastSafePosition;       // 낭떠러지에 떨어지지 않은 마지막 안전한 위치 저장

    protected override void Awake()
    {
        base.Awake();
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        lastSafePosition = transform.position;
    }

    protected override void FixedUpdate()
    {

        if( isKnockback)
        {
            base.FixedUpdate();
            return;
        }

        // 타겟이 탐지 범위 내에 들어오면
        if (Vector2.Distance(transform.position, target.position) <= detectionRange)
        {
            isMovingToTarget = true;    // 타겟으로 이동 플래그 활성화
            animator.SetBool("IsWalking", true);  // Walk 상태로 전환
            MoveToTarget();             // 타겟으로 이동
        }
        else
        {
            isMovingToTarget = false;    // 타겟으로 이동 플래그 비활성화   
            animator.SetBool("IsWalking", false);  // Idle 상태로 전환
        }

        CheckForEdge(); // 낭떠러지 감지 및 처리
    }

    private void MoveToTarget()
    {
        Vector2 dirVec = target.transform.position - transform.position;    //타겟 방향 계산
        float distanceToTarget = dirVec.magnitude;      //타겟과의 거리 계산

        //transform.localScale = dirVec.x > 0 ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1); // 주인공 방향에 따라 스프라이트 반전


        if (distanceToTarget > stopDistance)    // 타겟과의 거리가 멈추는 거리보다 크면 이동
        {
            Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;  // 이동 벡터 계산
            Vector3 nextPosition = transform.position + (Vector3)nextVec;   // 다음 이동 위치 계산

            if (!IsEdgeAhead(nextPosition)) // 낭떠러지가 아닌 경우
            {
                transform.position = nextPosition;  // 이동
                lastSafePosition = transform.position;  //마지막 안전 위치 갱신
                spriteRenderer.flipX = dirVec.x > 0;    // 타겟 방향에 따라 스프라이트 뒤집기
            }
            else
            {
                transform.position = lastSafePosition;  // 낭떠러지인 경우 안전 위치로 복귀
            }
        }

        animator.SetBool("IsWalking", true);  // 애니메이션 계속 유지

        if (distanceToTarget <= stopDistance && projectilePrefab != null) //멈추는 거리 안에 들어오면
        {
            StartAttack(); //발사체 발사 모션 스타트 
        }
    }

    private bool IsEdgeAhead(Vector3 position)
    {
        Vector2 frontVec = new Vector2(position.x + (spriteRenderer.flipX ? -0.5f : 0.5f), position.y); //스프라이트 앞쪽의 위치 계산
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));  //디버그 레이를 아래로 쏨
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform")); // "platform" 레이어로 레이케스트 실행

        if (rayHit.collider == null)    // 레이케스트가 충돌하지 않으면 낭떠러지임
        {
            Debug.Log("경고! 이 앞은 낭떠러지!!");
            return true;
        }
        return false;   //낭떠러지가 아님
    }

    private void CheckForEdge()
    {
        if (IsEdgeAhead(transform.position))  // 현재 위치에서 낭떠러지를 감지하면
        {
            transform.position = lastSafePosition;  // 마지막 안전한 위치로 복귀
        }
    }


    private void StartAttack()
    {
        if(Time.time - lastShootTime < shootCooldown) // 쿨타임 아직 남았으면 리턴
        {
            return;
        }

        isAttacking = true;
        animator.SetBool("isAttacking", true);


    }


    private void ShootProjectile()
    {
        if (Time.time - lastShootTime >= shootCooldown) // 발사 간격이 지나면
        {
            lastShootTime = Time.time;  // 마지막 발사 시간 갱신

            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity); // 발사체 생성
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>(); // 발사체의 Rigidbody2D 가져오기

            if (rb != null) // Rigidbody2D가 있으면
            {
                Vector2 direction = (target.transform.position - transform.position).normalized; // 타겟 방향 계산
                direction.y += 0.5f; // 발사 각도를 조금 위로 조정
                direction = direction.normalized; // 방향 벡터 정규화

                Vector2 force = direction * projectileSpeed; // 발사 속도를 곱한 힘 계산
                rb.AddForce(force, ForceMode2D.Impulse); // 발사체에 힘 가하기
            }

            Projectile projectileScript = projectile.AddComponent<Projectile>(); // 발사체에 Projectile 스크립트 추가
            projectileScript.Initialize(target); //발사체 초기화 
        }
        isAttacking = false;
        animator.SetBool("isAttacking", false);

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.gameObject.GetComponentInChildren<PlayerHealth>().TakeDamage(1);
            Debug.Log("Projectile hit the player!");
            //Destroy(gameObject);
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
            collision.gameObject.GetComponentInChildren<PlayerHealth>().TakeDamage(1);
            Debug.Log("Projectile hit the player!");
            Destroy(gameObject);
        }
        else Destroy(gameObject);
    }
}
