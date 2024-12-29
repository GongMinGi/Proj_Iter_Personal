using UnityEngine;

public class ZombieMoveTemp : BaseMonster
{
    public float speed; // 이동 속도
    public float attackRange; // 공격 범위
    public float attackCooldown; // 공격 쿨타임
    public Rigidbody2D target; // 주인공 타겟

    private SpriteRenderer spriteRenderer; // 스프라이트 렌더러: 좀비 이미지 반전 및 표시
    private float lastAttackTime = 0f; // 마지막 공격 시간이 저장됨
    private bool isMovingToTarget = false; // 주인공에게 이동 중인지 여부

    public Transform attackBoxPos;
    public Vector2 boxSize;

    private bool isAttack = false;

    protected override void Awake()
    {
        base.Awake(); // BaseMonster의 Awake 메서드 호출
        spriteRenderer = GetComponent<SpriteRenderer>(); // SpriteRenderer 컴포넌트 가져오기
    }

    protected override void FixedUpdate()
    {
        //base.FixedUpdate(); // 부모 클래스의 FixedUpdate 호출
        if ( isAttack)
        {
            return;
        }


        float distanceToTarget = Vector2.Distance(transform.position, target.position); // 주인공과의 거리 계산

        if (distanceToTarget <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            StartAttack(); // 공격 실행

            AudioManager.instance.PlaySfx(AudioManager.Sfx.Zombie2);
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

        //spriteRenderer.flipX = dirVec.x > 0 ? true : false ; // 주인공 방향에 따라 스프라이트 반전
        transform.localScale = dirVec.x > 0 ?  new Vector3 ( 1, 1, 1) :  new Vector3 (-1, 1,1); // 주인공 방향에 따라 스프라이트 반전

    }


    public void StartAttack()
    {
        isAttack = true;
        // 공격 처리
        lastAttackTime = Time.time; // 마지막 공격 시간 갱신
        animator.SetTrigger("Attack"); // 공격 애니메이션 실행
    }

    public void Attack()
    {

        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(attackBoxPos.position, boxSize, 0);
        foreach (Collider2D collider in collider2Ds)
        {
            Debug.Log(collider.tag);

            if (collider.CompareTag("Player")) //태그가 Monster인 경우
            {
                //collider.GetComponent<EnemyHealth>().Damage(atk, collider.transform.position - transform.position);
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
        // Idle 상태 처리
        animator.SetTrigger("Idle"); // Idle 애니메이션 실행
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.gameObject.GetComponentInChildren<PlayerHealth>().TakeDamage(1, transform.position);


            Debug.Log("플레이어와 충돌");
            // 충돌한 객체가 주인공일 경우 (필요 시 추가 로직 구현 가능)
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackBoxPos.position, boxSize);
    }

    public override void TakeDamage(int damage, Vector2 damageSourcePosition)
    {

        base.TakeDamage(damage, damageSourcePosition);

        animator.SetTrigger("Damaged");

    }

}
