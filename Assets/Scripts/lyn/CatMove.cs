using UnityEngine;

public class CatMove : BaseMonster
{
    public Transform[] waypoints;       // zz
    public float rayDistance = 20f;    // zz
    public LayerMask playerLayer;      // zz
    private int currentWaypointIndex = 0; // zz

    public float moveSpeed; // 이동 속도
    public float detectionRange; // 플레이어 탐지 범위
    public float attackCooldown; // 공격 대기 시간 (쿨타임) 
    public Transform target; // 타겟(Player)
    public float bounceForceX; // 충돌 후 튕겨나가는 x 축 힘
    public float bounceForceY; // 충돌 후 튕겨나가는 y 축 힘
    [SerializeField] public float jumpForce;    // 공격할 때 도약하는 힘
    [SerializeField] public float attackWaitTime; //도약 직전 대기하는 시간 (자세잡는시간)
    [SerializeField] public float horizontalJumpForce;

    //private Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;
    //private Animator animator;

    private float lastAttackTime = 0f;  // 마지막 공격 시간이 저장된다.
    private bool isGrounded = true;  // 고양이가 바닥에 있는지 여부
    private bool isWaiting = false;  //wait 상태(공격 준비 상태) 여부
    private bool hasFlippedOnce = false;
    //private bool isAttacking = false;


    protected override void Awake()
    {
        base.Awake();
        //rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        //animator = GetComponent<Animator>();

        SetAnimatorState(1);     // zz
        rigid.gravityScale = 0;         // zz
    }

    protected override void FixedUpdate()
    {

        if (waypoints.Length == 0)      // zz
        {
            return;
        }
        float distanceToTarget = Vector2.Distance(transform.position, target.position); // 타겟과의 거리 계산

        if (isWaiting)
        {
            // Wait 상태 중에는 아무것도 하지 않음
            SetAnimatorState(2); // 애니메이션을 wait으로 설정 
            return;
        }

        if (distanceToTarget > detectionRange)
        {
            //타겟이 탐지 범위 밖에 있는 경우
            MoveAlongWaypoints();       // zz

            // SetAnimatorState(0); // Idle 애니메이션 실행
            rigid.linearVelocity = Vector2.zero; // 속도 초기화
        }
        else if (distanceToTarget > 4.0f)
        {
            // 타겟이 탐지 범위 안이지만 가까이 있지 않은 경우
            SetAnimatorState(1); // Walk 애니메이션 실행
            ChaseTarget();  // 타겟을 추적
        }
        else
        {
            // 타겟이 가까이 있는 경우 공격 상태로 전환
            TriggerWaitAndAttack(distanceToTarget);
           
        }
    }

    private void MoveAlongWaypoints()       //zz
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime);

        Vector2 dirVec = targetWaypoint.position - transform.position;
        UpdateDirection(dirVec);  // 방향 업데이트

        if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;   // 순환 이동
        }
    }

    private void UpdateDirection(Vector2 direction)     //zz
    {

        if (Mathf.Abs(direction.x) > 0.01f)  // 매우 작은 움직임 방지
        {
            spriteRenderer.flipX = direction.x > 0 ? true : false;
        }
    }

    private void SetAnimatorState(int state)
    {
        // 애니메이션 상태를 변경
        animator.SetInteger("State", state);
    }

    private void ChaseTarget()
    {

        rigid.gravityScale = 1; // zz
        if (currentWaypointIndex == 1 && !hasFlippedOnce)       //zz
        {

            spriteRenderer.flipX = !spriteRenderer.flipX;
            hasFlippedOnce = true;

        }
        //타겟을 추적하는 동작
        if (isGrounded)
        {
            Vector2 direction = (target.position - transform.position).normalized;  // 타겟 방향 계산
            rigid.linearVelocity = new Vector2(direction.x * moveSpeed, rigid.linearVelocity.y); // 속도 설정
            //spriteRenderer.flipX = direction.x < 0; // 방향에 따라 스프라이트 반전
            transform.localScale = direction.x < 0 ?  new Vector3 ( 1, 1, 1) :  new Vector3 (-1, 1,1); // 주인공 방향에 따라 스프라이트 반전

        }
    }

    private void TriggerWaitAndAttack(float distanceToTarget)
    {
        if (isGrounded && Time.time - lastAttackTime >= attackCooldown)
        {
            // Wait 상태로 전환
            isWaiting = true;
            SetAnimatorState(2); // Wait
            //rigid.linearVelocity = Vector2.zero; // 이동 정지
            Invoke(nameof(PerformAttack), attackWaitTime); // 1.5초 후 공격 실행
            //PerformAttack();
        }
    }

    private void PerformAttack()
    {
        // 공격 실행 
        isWaiting = false; // wait 상태 해제 
        //isAttacking = true;
        lastAttackTime = Time.time; // 마지막 공격 시간 업데이트

        // Jump 공격 실행
        SetAnimatorState(3); // Attack 애니메이션 실행
        JumpToTarget(target.position); // 타겟 위치로 점프
    }

    private void JumpToTarget(Vector3 targetPosition)
    {
        //타겟을 향해 점프
        if (isGrounded)
        {
            Vector2 jumpDirection = targetPosition - transform.position; // 타겟 방향 계산
            float horizontalDistance = jumpDirection.x; // 수평거리
            float verticalDistance = jumpDirection.y; // 수직 거리


            // 타겟에 도달하기 위한 시간 계산
            float timeToReachTarget = Mathf.Abs(horizontalDistance) / moveSpeed;

            // 수직 및 수평 속도 계산
            //float verticalVelocity = (verticalDistance + 0.5f * Mathf.Abs(Physics2D.gravity.y) * timeToReachTarget * timeToReachTarget) / timeToReachTarget;
            float verticalVelocity = (verticalDistance + (jumpForce * timeToReachTarget)) / timeToReachTarget;
            float horizontalVelocity = (horizontalDistance / timeToReachTarget) * horizontalJumpForce;

            rigid.linearVelocity = new Vector2(horizontalVelocity, verticalVelocity);

            isGrounded = false; // 점프 상태로 전환
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //충돌 처리
        if (collision.gameObject.CompareTag("Ground"))
        {
            //바닥과 충돌할 경우
            isGrounded = true; // 바닥 상태로 전환
            SetAnimatorState(0); // Idle 애니메이션 실행
            rigid.linearVelocity = Vector2.zero; // 속도 초기화
        }

        if (collision.gameObject.CompareTag("Player") )
        {
            //플레이어와 충돌한 경우 
            Debug.Log("Player와 충돌: 고양이가 튕겨 나갑니다.");
            collision.gameObject.GetComponentInChildren<PlayerHealth>().TakeDamage(1, transform.position);
            // Walk -> Wait -> Attack 반복


            BounceBackFromPlayer(collision); // 플레이어로부터 튕겨나감

            //isAttacking = false;

            SetAnimatorState(0); // Idle 애니메이션 실행 
            rigid.linearVelocity = Vector2.zero; // 속도 초기화



            //ChaseTarget();
            Invoke(nameof(ChaseTarget), attackWaitTime); // 1초 후 타겟 추적( walk 상태로 전환) 
        }
    }

    private void BounceBackFromPlayer(Collision2D collision)
    {
        // 플레이어와 충돌 시 튕겨나가는 동작
        Vector2 bounceDirection = (transform.position - collision.transform.position).normalized;
        Debug.Log($"바운스 방향:{bounceDirection}");

        rigid.AddForce(new Vector2(bounceDirection.x * bounceForceX, Mathf.Clamp(bounceDirection.y, 0.1f, 1f) * bounceForceY));
    }

    private void StartWaitState()
    {
        //wait 상태 시작
        isWaiting = true;
        SetAnimatorState(2); // Wait 상태 활성화
        //rigid.linearVelocity = Vector2.zero;
        Invoke(nameof(PerformAttack), attackWaitTime); // Wait 상태 후 공격 실행
    }

    public override void TakeDamage(int damage, Vector2 damageSourcePosition)
    {

        base.TakeDamage(damage, damageSourcePosition);

        animator.SetTrigger("Damaged");

    }


}
