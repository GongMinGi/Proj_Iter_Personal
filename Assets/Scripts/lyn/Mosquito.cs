using UnityEngine;


// 모기의 행동( 진동, 타겟으로 이동, 후퇴)를 관리하는 클래스
public class Mosquito : BaseMonster
{
    public float speed; //이동속도  
    public float chargeDelay; //타겟으로 돌진하기 전 대기 시간
    public float retreatDistanceX; //후퇴 시 x축 이동 거리
    public float retreatDistanceY; //후퇴 시 y축 이동 거리
    public float retreatSpeed; //후퇴 속도
    public float activationDistance; //타겟과의 거리 안에 들어왔을 때 모기 활성화 거리


    public Rigidbody2D target; // 플레이어의 Rigidbody2D (타겟 대상)
    //Vector2 dirVec;             // 타겟 방향

    //private Rigidbody2D rigid; // 모기의 Rigidbody2D
    private SpriteRenderer spriteRenderer; // 모기의 SpriteRenderer
    //private Animator animator; //모기의 애니메이터

    private float timeSinceLastAction = 0f; // 현재 상태에서 경과한 시간
    private Vector2 retreatPosition; //모기가 후퇴할 위치
    private Vector2 targetPosition; // 현재 목표 위치
    private const float TARGET_PROXIMITY_THRESHOLD = 1.0f; // 타겟 근접 판정거리
    private const float RETREAT_PROXIMITY_THRESHOLD = 0.5f; // 후퇴 위치 근접 판정거리


    //모기의 행동 상태
    private enum MosquitoState
    {
        Oscillating,        // 제자리에서 진동하는 상태
        MovingToTarget,     // 타겟으로 이동하는 상태   
        Retreating,          // 후퇴하는 상태
        Knockback           // 넉백
    }

    private MosquitoState currentState = MosquitoState.Oscillating; // 현재 상태(초기값: Oscillating)
    private bool isActivated = false;   // 활성화 여부(활성화 거리 안에 들어와야 활성화)

    protected override void Awake()
    {
        base.Awake();
        //rigidbody2D, SpriteRenderer, Animator 컴포넌트 초기화
        //rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        //animator = GetComponent<Animator>();
    }

    protected override void FixedUpdate()
    {
        //base.fixedUpdate();

        //활성화되지 않았다면 활성화 거리 확인
        if (!isActivated)
        {
            float distanceToTarget = Vector2.Distance(rigid.position, target.position);
            if (distanceToTarget <= activationDistance)
            {
                isActivated = true; //활성화
                targetPosition = target.position;    // 타겟의 현재 위치 저장
            }
            else
            {
                OscillateInPlace(); //활성화 되지 않은 상태에서 제자리 진동
                return;
            }
        }


        // 현재 상태에 따라 동작 수행
        timeSinceLastAction += Time.fixedDeltaTime;

        switch (currentState)
        {
            case MosquitoState.Oscillating:     //제자리 진동 상태
                OscillateInPlace();
                if (timeSinceLastAction >= chargeDelay) // 대기 시간이 지나면
                {
                    SetState(MosquitoState.MovingToTarget); // 타겟으로 이동 상태로 전환
                }
                break;
            case MosquitoState.MovingToTarget:  // 타겟으로 이동 상태
                MoveToTarget();
                break;
            case MosquitoState.Retreating:  // 후퇴 상태
                Retreat();
                break;

            case MosquitoState.Knockback:
                Knockback();
                break;
        }
    }


    private void MoveToTarget()
    {
        //타겟 방향 계산
        Vector2 dirVec = targetPosition - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;  // 이동 벡터 계산
        rigid.MovePosition(rigid.position + nextVec); //모기 이동


        // 타겟의 위치에 따라 스프라이트 방향 설정
        //spriteRenderer.flipX = dirVec.x < 0;
        transform.localScale = dirVec.x < 0 ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1); // 주인공 방향에 따라 스프라이트 반전

        //타겟 근처에 도달하면 후퇴 상태로 전환
        if (Vector2.Distance(rigid.position, targetPosition) <= TARGET_PROXIMITY_THRESHOLD)
        {
            //후퇴 위치 설정
            retreatPosition = targetPosition + new Vector2(retreatDistanceX * (dirVec.x < 0 ? -1 : 1), retreatDistanceY);
            SetState(MosquitoState.Retreating);
        }
    }

    private void SetState(MosquitoState newState)
    {
        // 상태 변경
        if (currentState != newState)
        {
            currentState = newState;
            timeSinceLastAction = 0f;   // 상태 변경 시 시간 초기화

            switch (newState)
            {
                case MosquitoState.Oscillating:
                    animator.SetTrigger("Fly");
                    break;
                case MosquitoState.MovingToTarget:
                    animator.SetTrigger("Attack");
                    targetPosition = target.position;
                    break;
                case MosquitoState.Retreating:
                    animator.SetTrigger("Retreat");
                    break;
                case MosquitoState.Knockback:   //넉백 시에도 그냥fly로 돌아감
                    animator.SetTrigger("Fly");
                    break;

            }
        }
    }

    private void OscillateInPlace()
    {
        //모기가 제자리에서 진동하는 효과
        float oscillationX = Mathf.Sin(Time.time * 10.0f) * 0.3f;   // x축 진동
        float oscillationY = Mathf.Cos(Time.time * 10.0f) * 0.3f;   // y축 진동
        Vector2 oscillationOffset = new Vector2(oscillationX, oscillationY);
        rigid.MovePosition(rigid.position + oscillationOffset * Time.fixedDeltaTime);   // 진동 적용
    }

    private void Retreat()
    {
        // 후퇴 방향 계산
        Vector2 dirVec = retreatPosition - rigid.position;
        rigid.MovePosition(Vector2.MoveTowards(rigid.position, retreatPosition, retreatSpeed * Time.fixedDeltaTime)); // 후퇴 이동
        //spriteRenderer.flipX = dirVec.x < 0; // 후퇴 방향에 따라 스프라이트 방향 설정
        transform.localScale = dirVec.x > 0 ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1); // 주인공 방향에 따라 스프라이트 반전


        //후퇴 위치 근처에 도달하면 상태를 진동 상태로 전환
        if (Vector2.Distance(rigid.position, retreatPosition) <= RETREAT_PROXIMITY_THRESHOLD)
        {
            SetState(MosquitoState.Oscillating);
        }
    }

    private void Knockback()
    {
        //넉백 이동
        //Vector2 dirVec = knockbackPosition - rigid.position;
        rigid.MovePosition(Vector2.MoveTowards(rigid.position, knockbackPosition, knockbackSpeed * Time.fixedDeltaTime));

        if (Vector2.Distance(rigid.position, knockbackPosition) <= RETREAT_PROXIMITY_THRESHOLD)
        {
            SetState(MosquitoState.Oscillating);
        }
    }

    public override void TakeDamage(int damage, Vector2 damageSourcePosition)
    {
        //체력감소
        health -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Remaining health: {health}");

        //죽음 처리
        if (health <= 0)
        {
            Die();
        }

        // 넉백 방향 계산 (x 축 방향으로만)
        Vector2 dirVec = new Vector2(rigid.position.x - damageSourcePosition.x, 0).normalized;

        // 넉백 위치 계산 (현재 위치에서 넉백 거리만큼 이동)
        knockbackPosition = rigid.position + dirVec.normalized * knockbackDistance;

        // 상태를 Knockback으로 전환
        SetState(MosquitoState.Knockback);
    }




    void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한 대상이 플레이어일 경우
        if (other.CompareTag("Player"))
        {

            // 필요한 추가 동작이 있으면 여기에 추가
            // 예를 들어, 공격 애니메이션을 시작하거나, 데미지를 주는 등의 동작
            Debug.Log("Mosquito collided with Player!");

            other.GetComponentInChildren<PlayerHealth>().TakeDamage(1);
        }
    }
}
