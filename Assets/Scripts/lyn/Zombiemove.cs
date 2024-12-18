using UnityEngine;

public class zombiemove : MonoBehaviour
{
    public float moveSpeed; // 이동 속도
    public float detectionRange; // 탐지 범위
    public float attackCooldown; // 공격 대기 시간
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
        else if (distanceToTarget > 1.0f)
        {
            SetAnimatorState(1); // Walk
            ChaseTarget();
        }
        else
        {
            PerformAttack(); // 플레이어가 가까우면 바로 공격
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

    private void PerformAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;

            // 공격 실행
            SetAnimatorState(3); // Attack
            Debug.Log("플레이어를 공격합니다.");
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
            Debug.Log("Player와 충돌: 공격!");
            PerformAttack(); // 플레이어와 충돌 시 공격
        }
    }
}
