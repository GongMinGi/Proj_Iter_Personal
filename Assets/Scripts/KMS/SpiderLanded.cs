using UnityEngine;

public class SpiderLanded : BaseMonster
{

    public float speed; // 이동 속도
    public float attackRange; // 공격 범위
    public float attackCooldown; // 공격 쿨타임

    public Rigidbody2D target; // 주인공 타겟

    private SpriteRenderer spriteRenderer; // 스프라이트 렌더러: 좀비 이미지 반전 및 표시
    private float lastAttackTime = 0f; // 마지막 공격 시간이 저장됨

    public Transform attackBoxPos;

    public Vector2 boxSize;

    private bool isAttack = false;

    protected override void Awake()
    {

        this.enabled = false;

        base.Awake();

        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    protected override void FixedUpdate()
    {

        if (isAttack)
        {

            return;

        }

        float distanceToTarget = Vector2.Distance(transform.position, target.position); // 주인공과의 거리 계산

        if (distanceToTarget <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {

            StartAttack(); // 공격 실행

        }
        else if (distanceToTarget > attackRange && !isAttack)
        {

            MoveToTarget(); // 주인공을 향해 이동

        }
        else
        {

            Idle(); // Idle 상태로 전환

        }

    }

    private void MoveToTarget()
    {

        Vector2 dirVec = target.position - (Vector2)transform.position; // 주인공과의 방향 벡터 계산
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime; // 이동할 위치 계산 (속도 * 시간)

        animator.SetTrigger("Walk");

        transform.position += (Vector3)nextVec; // 좀비의 위치 업데이트

        transform.localScale = dirVec.x > 0 ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1); // 주인공 방향에 따라 스프라이트 반전

    }


    public void StartAttack()
    {

        isAttack = true;

        lastAttackTime = Time.time; // 마지막 공격 시간 갱신

        animator.SetTrigger("Attack"); // 공격 애니메이션 실행

    }

    public void Attack()
    {

        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(attackBoxPos.position, boxSize, 0);

        foreach (Collider2D collider in collider2Ds)
        {

            Debug.Log(collider.tag);

            if (collider.CompareTag("Player")) 
            {

                collider.GetComponentInChildren<PlayerHealth>().TakeDamage(1, transform.position);

            }

        }

    }

    public void EndAttack()
    {

        isAttack = false;

        Idle();

    }

    private void Idle()
    {

        animator.SetTrigger("Idle"); 

    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.CompareTag("Player"))
        {

            Debug.Log("플레이어와 충돌");
            // 충돌한 객체가 주인공일 경우 (필요 시 추가 로직 구현 가능)

        }

    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(attackBoxPos.position, boxSize);

    }

}
