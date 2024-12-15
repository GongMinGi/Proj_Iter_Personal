using UnityEngine;

public class Mosquito : MonoBehaviour
{
    public float speed; // 이동 속도
    public float chargeDelay = 2.0f; // 주인공에게 다가가기 전 대기 시간
    public Rigidbody2D target; // 주인공 타겟

    private Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;

    private LineRenderer lineRenderer; //line renderer추가

    private float oscillationFrequency = 10.0f; // 진동 주파수
    private float oscillationAmplitude = 0.3f; // 진동 폭

    private float timeSinceLastAction = 0f; // 마지막 동작 이후 경과 시간
    private bool isMovingToTarget = false; // 주인공에게 이동 중인지 여부

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        lineRenderer = GetComponent<LineRenderer>(); // Line Renderer 가져오기

        // Line Renderer 초기화
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2; // 두 점(시작점, 끝점)으로 구성된 선
            lineRenderer.startWidth = 0.1f; // 선의 시작 부분 두께
            lineRenderer.endWidth = 0.1f; // 선의 끝 부분 두께
            lineRenderer.startColor = Color.red; // 선의 시작 색
            lineRenderer.endColor = Color.red; // 선의 끝 색
        }
    }

    void FixedUpdate()
    {
        // 시간 업데이트
        timeSinceLastAction += Time.fixedDeltaTime;

        if (!isMovingToTarget)
        {
            // 진동 모드
            OscillateInPlace();

            // 일정 시간 경과 후 이동 상태로 전환
            if (timeSinceLastAction >= chargeDelay)
            {
                isMovingToTarget = true;
                timeSinceLastAction = 0f; // 타이머 초기화
                UpdateLineRenderer(false); // 이동 시작 시 점선 비활성화
            }
        }
        else
        {
            // 주인공에게 이동
            MoveToTarget();

            // 주인공에게 다가가는 동안 동작이 멈추지 않음
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
    }
    private void UpdateLineRenderer(bool isActive)
    {
        if (lineRenderer != null && target != null)
        {
            lineRenderer.enabled = isActive; // Line Renderer 활성화/비활성화
            if (isActive)
            {
                // 시작점: 모기의 현재 위치
                lineRenderer.SetPosition(0, transform.position);

                // 끝점: 주인공의 위치
                lineRenderer.SetPosition(1, target.position);
            }
        }
    }
}
