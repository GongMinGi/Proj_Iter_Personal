using UnityEngine;

public class CatAI : MonoBehaviour
{
    public float speed = 2.0f; // 이동 속도
    public float attackRange = 1.0f; // 공격 범위
    public float detectionRange = 5.0f; // 탐지 범위
    public float attackCooldown = 2.0f; // 공격 대기 시간
    public Transform target; // 주인공 타겟

    private Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;

    private float lastAttackTime = 0f; // 마지막 공격 시간
    private enum State { Idle, Chase, Attack }
    private State currentState = State.Idle;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        // 주인공이 탐지 범위 안에 들어오면 추적 상태로 전환
        if (distanceToTarget <= detectionRange)
        {
            currentState = State.Chase;
        }
    }

    private void HandleChaseState(float distanceToTarget)
    {
        // 주인공을 향해 이동
        Vector2 direction = (target.position - transform.position).normalized;
        rigid.MovePosition(rigid.position + direction * speed * Time.fixedDeltaTime);

        // 방향에 따라 스프라이트 좌우 반전
        spriteRenderer.flipX = direction.x < 0;

        // 공격 범위에 도달하면 공격 상태로 전환
        if (distanceToTarget <= attackRange)
        {
            currentState = State.Attack;
        }

        // 주인공이 탐지 범위를 벗어나면 다시 대기 상태로 전환
        if (distanceToTarget > detectionRange)
        {
            currentState = State.Idle;
        }
    }

    private void HandleAttackState(float distanceToTarget)
    {
        // 주인공 공격
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time; // 공격 시간 갱신
        }

        // 공격 범위를 벗어나면 다시 추적 상태로 전환
        if (distanceToTarget > attackRange)
        {
            currentState = State.Chase;
        }
    }

    private void Attack()
    {
        // 공격 로직 (예: 체력 감소)
        Debug.Log("고양이가 주인공을 공격했습니다!");
    }
}
