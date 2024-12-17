using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public float moveSpeed = 2.0f; // 이동 속도
    public float detectionRange = 5.0f; // 탐지 범위
    public float attackCooldown = 2.0f; // 공격 대기 시간
    public Transform target; // 타겟(Player)

    private Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private float lastAttackTime = 0f;
    private bool isGrounded = true;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        if (distanceToTarget > detectionRange)
        {
            SetAnimatorState(0); // Idle
            rigid.linearVelocity = Vector2.zero; // 속도 초기화
        }
        else if (distanceToTarget > 3.0f)
        {
            SetAnimatorState(1); // Walk
            ChaseTarget();
        }
        else
        {
            SetAnimatorState(2); // Attack
            AttackTarget(distanceToTarget);
        }
    }

    private void SetAnimatorState(int state)
    {
        animator.SetInteger("State", state);
    }

    private void ChaseTarget()
    {
        if (isGrounded)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            rigid.linearVelocity = new Vector2(direction.x * moveSpeed, rigid.linearVelocity.y);
            spriteRenderer.flipX = direction.x < 0; // 방향에 따라 스프라이트 반전
        }
    }

    private void AttackTarget(float distanceToTarget)
    {
        if (isGrounded && Time.time - lastAttackTime >= attackCooldown)
        {
            JumpToTarget(target.position);
            lastAttackTime = Time.time;
        }
    }

    private void JumpToTarget(Vector3 targetPosition)
    {
        if (isGrounded)
        {
            Vector2 jumpDirection = targetPosition - transform.position;
            float horizontalDistance = jumpDirection.x;
            float verticalDistance = jumpDirection.y;

            float timeToReachTarget = Mathf.Abs(horizontalDistance) / moveSpeed;

            float verticalVelocity = (verticalDistance + 0.5f * Mathf.Abs(Physics2D.gravity.y) * timeToReachTarget * timeToReachTarget) / timeToReachTarget;
            float horizontalVelocity = horizontalDistance / timeToReachTarget;

            rigid.linearVelocity = new Vector2(horizontalVelocity, verticalVelocity);

            isGrounded = false; // 점프 상태
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        // Player와 충돌 시 고양이가 튕겨나감
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player와 충돌: 고양이가 튕겨 나갑니다.");
            BounceBackFromPlayer(collision);

            // Idle 상태로 전환 후 다시 Walk로 복귀
            SetAnimatorState(0); // Idle
            rigid.linearVelocity = Vector2.zero; // 속도 초기화
            Invoke(nameof(ReturnToChase), 1.0f); // 1초 후 Walk 상태로 복귀
        }
    }

    private void BounceBackFromPlayer(Collision2D collision)
    {
        Vector2 bounceDirection = (transform.position - collision.transform.position).normalized;
        rigid.AddForce(bounceDirection * 300f); // 튕겨 나가는 힘 (300f는 임의의 값으로 조절 가능)
    }

    private void ReturnToChase()
    {
        SetAnimatorState(1); // Walk
        ChaseTarget(); // Player 추적 재개
    }
}
