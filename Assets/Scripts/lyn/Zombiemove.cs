using UnityEngine;

public class ZombieAI : BaseMonster
{
    public float speed; // 이동 속도
    public float attackRange; // 공격 범위
    public float backSpeed; // 후퇴 속도 (뒤로 물러날 속도)
    public float attackDelay; // 공격 대기 시간
    public float backDistanceX; // 후퇴할 X 거리
    public float backDistanceY; // 후퇴할 Y 거리
    public Rigidbody2D target; // 주인공 타겟

    //private Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;
    //private Animator animator; // Animator 컴포넌트

    private float timeSinceLastAction = 0f; // 마지막 동작 이후 경과 시간
    private bool isMovingToTarget = false; // 주인공에게 이동 중인지 여부
    private bool isAttacking = false; // 공격 중인지 여부
    private bool isBacking = false; // 후퇴 중인지 여부
    private bool hasAttacked = false; // 공격이 이미 이루어졌는지 여부
    private Vector2 backPosition; // 후퇴할 목표 위치

    protected override void Awake()
    {
        base.Awake();
        //rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        //animator = GetComponent<Animator>(); // Animator 컴포넌트 가져오기
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        timeSinceLastAction += Time.fixedDeltaTime;

        if (isBacking)
        {
            Back(); // 후퇴는 Back 상태에서만 이루어짐
        }
        else if (isAttacking)
        {
            // 공격 상태일 때는 `Back` 상태로 가지 않음
            Attack(); // 충돌 시 공격
        }
        /*else if (isMovingToTarget)
        {
            MoveToTarget(); // 주인공에게 이동
        }*/
        else
        {
            // Idle 상태에서 아무것도 하지 않음, 단지 애니메이션을 "Idle"로 설정
            animator.SetTrigger("Idle");

            // 후퇴 중이 아니면 공격 대기
            if (timeSinceLastAction >= attackDelay && !isBacking && !hasAttacked)
            {
                MoveToTarget();
                /*if (Vector2.Distance(transform.position, target.position) <= attackRange) // 충돌 시 공격
                {
                    isMovingToTarget = true;
                    timeSinceLastAction = 0f; // 타이머 초기화
                    animator.SetTrigger("Attack");
                    hasAttacked = true; // 공격이 이미 발생했음을 기록
                }*/
            }
        }
    }

    private void MoveToTarget()
    {
        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;

        transform.position += (Vector3)nextVec;

        spriteRenderer.flipX = dirVec.x < 0;

        if (Vector2.Distance(transform.position, target.position) <= 1.0f) // 충돌 시
        {
            // 주인공과 충돌하면 공격을 한 번만 실행
            isMovingToTarget = false;
            isAttacking = true;
            animator.SetTrigger("Attack");
        }
    }

    private void Attack()
    {
        // 공격 애니메이션 처리 (여기에서 주인공에게 피해를 주는 코드를 추가할 수 있음)

        // 후퇴 위치 계산 (주인공 방향 반대)
        backPosition = rigid.position + new Vector2(backDistanceX, backDistanceY);

        isAttacking = false;
        isBacking = true;

        // 후퇴 애니메이션 재생
        animator.SetTrigger("Back");

    }

    private void Back()
    {
        // 후퇴 (주인공의 방향과 반대 방향으로 이동)
        transform.position = Vector2.MoveTowards(transform.position, backPosition, backSpeed * Time.fixedDeltaTime);

        // 후퇴가 완료되면 다시 Idle 상태로 돌아가기
        if (Vector2.Distance(transform.position, backPosition) <= 0.1f)
        {
            isBacking = false;
            timeSinceLastAction = 0f; // 타이머 초기화

            // Idle 애니메이션 재생
            animator.SetTrigger("Idle");
        }
    }

    // 충돌 시 `Attack`을 한 번만 실행하도록 OnCollisionEnter2D 사용
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && !isAttacking)
        {
            // 주인공과 충돌 시에만 `Attack` 실행
            isAttacking = true;
            animator.SetTrigger("Attack");
            collision.gameObject.GetComponentInChildren<PlayerHealth>().TakeDamage(1);
        }
    }
}
