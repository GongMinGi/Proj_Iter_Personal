using UnityEngine;

public class Mosquito : MonoBehaviour
{
    public float speed = 2.0f; // 이동 속도
    public float chargeDelay = 2.0f; // 주인공에게 다가가기 전 대기 시간
    public float retreatDistanceX = 2.5f; // 주인공으로부터 물러나는 X 거리
    public float retreatDistanceY = 2.0f; // 주인공으로부터 물러나는 Y 거리
    public Rigidbody2D target; // 주인공 타겟

    private Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;

    private float oscillationFrequency = 10.0f; // 진동 주파수
    private float oscillationAmplitude = 0.3f; // 진동 폭

    private float timeSinceLastAction = 0f; // 마지막 동작 이후 경과 시간
    private bool isMovingToTarget = false; // 주인공에게 이동 중인지 여부
    private bool isRetreating = false; // 주인공으로부터 물러나는 중인지 여부
    private Vector2 retreatPosition; // 물러날 위치
    private float contactcheck = 0.2f;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        // 시간 업데이트
        timeSinceLastAction += Time.fixedDeltaTime;

        if (isRetreating)
        {
            // 물러나는 중
            Retreat();
        }
        else if (isMovingToTarget)
        {
            // 주인공에게 이동
            MoveToTarget();
        }
        else
        {
            // 진동 모드
            OscillateInPlace();

            // 일정 시간 경과 후 이동 상태로 전환
            if (timeSinceLastAction >= chargeDelay)
            {
                Debug.Log("Switching to move-to-target state.");
                isMovingToTarget = true;
                timeSinceLastAction = 0f; // 타이머 초기화
            }
        }
    }

    private void OscillateInPlace()
    {
        // 제자리에서 진동
        float oscillationX = Mathf.Sin(Time.time * oscillationFrequency) * oscillationAmplitude;
        float oscillationY = Mathf.Cos(Time.time * oscillationFrequency) * oscillationAmplitude;

        Vector2 oscillationOffset = new Vector2(oscillationX, oscillationY);

        rigid.MovePosition(rigid.position + oscillationOffset * Time.fixedDeltaTime);
        rigid.linearVelocity = Vector2.zero; // 속도 초기화
    }

    private void MoveToTarget()
    {
        // 주인공을 향해 이동
        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;

        rigid.MovePosition(rigid.position + nextVec);

        // 방향에 따라 스프라이트 좌우 반전
        spriteRenderer.flipX = dirVec.x < 0;

        // 주인공과의 거리 체크
        if (Vector2.Distance(rigid.position, target.position) <= contactcheck) // 접촉했을 때
        {
            Debug.Log("Contact with target. Calculating retreat position...");
            retreatPosition = target.position + new Vector2(retreatDistanceX, retreatDistanceY);
            isMovingToTarget = false;
            isRetreating = true;
        }
    }

    private void Retreat()
    {
        // 물러날 위치로 이동
        Vector2 dirVec = retreatPosition - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;

        rigid.MovePosition(rigid.position + nextVec);

        Debug.Log($"Retreating to {retreatPosition}, current position: {rigid.position}");

        // 물러날 위치에 도달했는지 확인
        if (Vector2.Distance(rigid.position, retreatPosition) <= 0.1f)
        {
            Debug.Log("Reached retreat position. Returning to oscillation mode.");
            isRetreating = false;
            timeSinceLastAction = 0f; // 타이머 초기화
        }
    }
}
