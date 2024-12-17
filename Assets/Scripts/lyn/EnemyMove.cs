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
    private bool isWaiting = false;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        if (isWaiting)
        {
            // Wait 상태 중에는 아무것도 하지 않음
            SetAnimatorState(2); // Wait
            return;
        }

        if (distanceToTarget > detectionRange)
        {
            SetAnimatorState(0); // Idle
            rigid.linearVelocity = Vector2.zero; // 속도 초기화
        }
        else if (distanceToTarget > 4.0f)
        {
            SetAnimatorState(1); // Walk
            ChaseTarget();
                    }
        else
        {
            // 공격 로직으로 이동
            TriggerWaitAndAttack(distanceToTarget);
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

    private void TriggerWaitAndAttack(float distanceToTarget)
    {
        if (isGrounded && Time.time - lastAttackTime >= attackCooldown)
        {
            // Wait 상태로 전환
            isWaiting = true;
            SetAnimatorState(2); // Wait
            rigid.linearVelocity = Vector2.zero; // 이동 정지
            //Invoke(nameof(PerformAttack), 2.0f); // 2초 후 공격 실행
            PerformAttack();
        }
    }

    private void PerformAttack()
    {
        isWaiting = false;
        lastAttackTime = Time.time;

        // Jump 공격 실행
        SetAnimatorState(3); // Attack
        JumpToTarget(target.position);
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

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player와 충돌: 고양이가 튕겨 나갑니다.");
            BounceBackFromPlayer(collision);

            // Walk -> Wait -> Attack 반복
            SetAnimatorState(0); // Idle
            rigid.linearVelocity = Vector2.zero; // 속도 초기화
            Invoke(nameof(StartWaitState), 1.0f); // 1초 후 Wait 상태로 전환
        }
    }

    private void BounceBackFromPlayer(Collision2D collision)
    {
        Vector2 bounceDirection = (transform.position - collision.transform.position).normalized;

        float bounceForceX = 700f;
        float bounceForceY = 200f;

        rigid.AddForce(new Vector2(bounceDirection.x * bounceForceX, Mathf.Clamp(bounceDirection.y, 0.1f, 1f) * bounceForceY));
    }

    private void StartWaitState()
    {
        isWaiting = true;
        SetAnimatorState(2); // Wait
        rigid.linearVelocity = Vector2.zero;
        Invoke(nameof(PerformAttack), 2.0f); // Wait 상태 후 공격 실행
    }
}
