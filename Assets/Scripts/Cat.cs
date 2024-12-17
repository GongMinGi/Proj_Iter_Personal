using UnityEngine;

public class Cat : MonoBehaviour
{
    public float moveSpeed = 2.0f; // 이동 속도
    public float jumpForceMultiplier = 1.0f; // 점프 힘 조정
    public float detectionRange = 5.0f; // 탐지 범위
    public float attackCooldown = 2.0f; // 공격 대기 시간
    public Transform target; // 주인공 타겟

    private Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;
    Animator animator;

    private float lastAttackTime = 0f; // 마지막 공격 시간
    private bool isGrounded = true; // 고양이가 바닥에 닿아있는지 확인
    private bool isJumping = false; // 현재 점프 중인지 확인
    private enum State { Idle, Chase, Attack }
    private State currentState = State.Idle;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        switch (currentState)
        {
            case State.Idle:
                HandleIdleState(distanceToTarget);
                break;

            case State.Chase:
                HandleChaseState(distanceToTarget);
                break;

            case State.Attack:
                HandleAttackState(distanceToTarget);
                break;
        }
    }

    private void HandleIdleState(float distanceToTarget)
    {
        if (distanceToTarget <= detectionRange)
        {
            currentState = State.Chase;
        }
    }

    private void HandleChaseState(float distanceToTarget)
    {
        if (isGrounded && !isJumping)
        {
            // 주인공을 향해 이동
            Vector2 direction = (target.position - transform.position).normalized;
            rigid.linearVelocity = new Vector2(direction.x * moveSpeed, rigid.linearVelocity.y);

            // 방향에 따라 스프라이트 좌우 반전
            spriteRenderer.flipX = direction.x < 0;

            // 주인공 가까이에 도달하면 공격 상태로 전환
            if (distanceToTarget <= 3.0f)
            {
                currentState = State.Attack;
            }
        }

        if (distanceToTarget > detectionRange)
        {
            currentState = State.Idle;
        }
    }

    private void HandleAttackState(float distanceToTarget)
    {
        if (isGrounded && Time.time - lastAttackTime >= attackCooldown)
        {
            // 주인공을 향한 점프
            JumpToTarget(target.position);

            Debug.Log("고양이가 주인공을 향해 점프했습니다!");
            lastAttackTime = Time.time;
        }

        // 공격 후 다시 추적 상태로 전환
        if (distanceToTarget > 3.0f)
        {
            currentState = State.Chase;
        }
    }

    private void JumpToTarget(Vector3 targetPosition)
    {
        if (isGrounded)
        {
            // 주인공까지의 거리 계산
            Vector2 jumpDirection = targetPosition - transform.position;

            // 수평 거리와 목표 착지 위치를 기준으로 속도 계산
            float horizontalDistance = jumpDirection.x;
            float verticalDistance = jumpDirection.y;

            float timeToReachTarget = Mathf.Abs(horizontalDistance) / moveSpeed;

            float verticalVelocity = (verticalDistance + 0.5f * Mathf.Abs(Physics2D.gravity.y) * timeToReachTarget * timeToReachTarget) / timeToReachTarget;
            float horizontalVelocity = horizontalDistance / timeToReachTarget;

            // 점프 힘 설정
            rigid.linearVelocity = new Vector2(horizontalVelocity, verticalVelocity);
            isGrounded = false;
            isJumping = true; // 점프 중 플래그 설정
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isJumping = false; // 점프 종료
        }
    }
}
