using UnityEngine;

public class SpiderMovement : BaseMonster
{
    public Transform[] waypoints;       // 천장 위의 이동할 웨이포인트 배열
    public float speed = 2f;           // 거미 이동 속도
    public float descendSpeed = 3f;    // 내려오는 속도
    public LayerMask playerLayer;      // 플레이어가 속한 레이어
    public float rayDistance = 20f;    // Ray의 길이
    public TrailRenderer webTrail;     // 거미줄 효과

    public Transform target;  // 주인공의 Transform

    private int currentWaypointIndex = 0; // 현재 이동 중인 웨이포인트
    private bool isDescending = false;    // 내려오는 중인지 확인
    private bool isOnGround = false;      // 거미가 바닥에 착지했는지 확인

    private SpriteRenderer spriteRenderer; // SpriteRenderer 컴포넌트

    private bool isZombieNow = false;      // 좀비 행동으로 전환 여부

    protected override void Awake()
    {
        base.Awake();

        spriteRenderer = GetComponent<SpriteRenderer>();

        webTrail.enabled = false;        // 거미줄 비활성화
        rigid.gravityScale = 0;         // 천장에서 중력 무시
    }

    private void Update()
    {
        if (isZombieNow)
        {
            // 좀비 동작으로 전환된 후, 거미 동작 비활성화
            return;
        }

        if (waypoints.Length == 0 || isDescending || isOnGround)
        {
            return;
        }

        // Waypoints 따라 이동
        MoveAlongWaypoints();

        // Ray를 아래로 발사하여 플레이어 감지
        DetectPlayer();
    }

    private void MoveAlongWaypoints()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; // 순환 이동
            Debug.Log($"Spider moved to waypoint {currentWaypointIndex}");
        }
    }

    private void DetectPlayer()
    {
        Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y - spriteRenderer.bounds.extents.y);
        Debug.DrawRay(rayOrigin, Vector2.down * rayDistance, Color.red); // Ray 시각적 디버그

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayDistance, playerLayer);

        if (hit.collider != null && hit.collider.CompareTag("Player") && !isDescending)
        {
            Debug.Log("Player detected!");
            StartDescending();
        }
    }

    private void StartDescending()
    {
        isDescending = true; // 내려오는 상태로 전환
        rigid.gravityScale = 1; // Rigidbody2D의 중력 활성화
        webTrail.enabled = true; // 거미줄 효과 활성화
        spriteRenderer.flipY = false; // 거미의 Sprite 방향 조정
        rigid.linearVelocity = Vector2.down * descendSpeed; // 아래 방향으로 속도 설정
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDescending && collision.collider.CompareTag("Ground"))
        {
            LandOnGround();
        }
    }

    private void LandOnGround()
    {
        isDescending = false;
        isOnGround = true;
        webTrail.enabled = false; // 거미줄 효과 비활성화
        rigid.linearVelocity = Vector2.zero; // 속도 초기화

        // 좀비 상태로 전환
        isZombieNow = true;

        Debug.Log("Spider transitioned to zombie behavior.");

        // ZombieMove 스크립트를 동적으로 추가
        ZombieMove zombieBehavior = gameObject.AddComponent<ZombieMove>();
        zombieBehavior.speed = speed;               // 거미 이동 속도 전달
        zombieBehavior.attackRange = 1.0f;          // 공격 범위 설정
        zombieBehavior.target = target.GetComponent<Rigidbody2D>();
        zombieBehavior.backSpeed = 2.0f;            // 후퇴 속도 설정
        zombieBehavior.backDistanceX = 1.0f;        // 후퇴 거리 X
        zombieBehavior.backDistanceY = 0.5f;        // 후퇴 거리 Y
        zombieBehavior.attackDelay = 1.5f;          // 공격 딜레이 설정

        // SpiderMovement 스크립트를 비활성화
        this.enabled = false;
    }
}
