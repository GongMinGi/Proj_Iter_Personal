using UnityEngine;

public class Spidermove : BaseMonster
{
    public float speed = 3f;  // 이동 속도
    public float detectionRange = 5f;  // 주인공 탐지 범위
    public float oscillationRange = 3f;  // 주인공 주변에서 왔다갔다할 범위

    public Transform target;  // 주인공의 Transform

    //private Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;
    //private Animator animator;
    private Vector2 centerPosition; // 주인공 중심 위치
    private float currentOffset; // 현재 중심점으로부터의 오프셋
    private int direction = 1; // 이동 방향 (1 또는 -1)
    private bool isPlayerDetected = false; // 주인공 탐지 여부
    private bool isOscillating = false; // 오실레이션 여부

    protected override void Awake()
    {
        base.Awake();
        //rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        //animator = GetComponent<Animator>();
    }

    protected override void FixedUpdate()
    {
        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        if (distanceToTarget <= detectionRange)
        {
            if (!isPlayerDetected)
            {
                isPlayerDetected = true;
                InitializeOscillation();
            }

            animator.SetBool("IsWalking", true);  // Walk 상태로 전환

            // 주인공이 오실레이션 범위 안에 들어오면 오실레이션 시작
            if (distanceToTarget <= oscillationRange)
            {
                isOscillating = true;
            }

            if (isOscillating)
            {
                OscillateAroundTarget();
            }
            else
            {
                MoveTowardsTarget();
            }
        }
        else
        {
            isPlayerDetected = false;
            isOscillating = false;
            animator.SetBool("IsWalking", false);  // Idle 상태로 전환
        }
    }

    private void MoveTowardsTarget()
    {
        // 타겟 방향으로 이동
        Vector2 dirVec = target.position - transform.position; // 타겟과의 방향 벡터 계산
        dirVec.Normalize();  // 방향 벡터를 정규화하여 이동

        // 타겟 방향으로 이동
        Vector2 nextVec = dirVec * speed * Time.fixedDeltaTime;  // 이동 벡터 계산
        rigid.MovePosition(rigid.position + nextVec);  // 타겟 방향으로 이동
    }

    private void InitializeOscillation()
    {
        // 주인공을 중심으로 설정
        centerPosition = target.position;

        // 거미의 현재 위치를 기준으로 초기 오프셋 계산
        currentOffset = transform.position.x - centerPosition.x;

        // 현재 오프셋에 따라 방향 설정
        direction = (currentOffset >= 0) ? -1 : 1;
    }

    private void OscillateAroundTarget()
    {
        // 주인공 위치를 중심으로 이동
        centerPosition = target.position;

        // 오프셋 업데이트
        currentOffset += speed * Time.fixedDeltaTime * direction;

        // 범위를 벗어나면 방향 전환
        if (Mathf.Abs(currentOffset) >= oscillationRange)
        {
            direction *= -1; // 방향 반전
            currentOffset = Mathf.Sign(currentOffset) * oscillationRange; // 범위 유지
        }

        // 새로운 위치 계산
        Vector2 newPosition = centerPosition + new Vector2(currentOffset, 0);

        // 이동 처리
        rigid.MovePosition(newPosition);

        // 스프라이트 방향 설정
        spriteRenderer.flipX = direction < 0;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {

            collider.gameObject.GetComponentInChildren<PlayerHealth>().TakeDamage(1, transform.position);
            Debug.Log("Spider passed through the player!");

        }
    }
}
