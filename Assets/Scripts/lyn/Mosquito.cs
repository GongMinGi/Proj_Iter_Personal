using UnityEngine;

public class Mosquito : MonoBehaviour
{
    public float speed; // 이동 속도
    public float chargeDelay; // 주인공에게 다가가기 전 대기 시간
    public float retreatDistanceX; // 주인공으로부터 물러나는 X 거리
    public float retreatDistanceY; // 주인공으로부터 물러나는 Y 거리
    public float retreatSpeed; // 후퇴 속도
    public Rigidbody2D target; // 주인공 타겟

    private Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;
    private Animator animator; // Animator 컴포넌트

    private float timeSinceLastAction = 0f; // 마지막 동작 이후 경과 시간
    private bool isMovingToTarget = false; // 주인공에게 이동 중인지 여부
    private bool isRetreating = false; // 주인공으로부터 물러나는 중인지 여부
    private Vector2 retreatPosition; // 물러날 위치

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>(); // Animator 컴포넌트 가져오기
    }

    void FixedUpdate()
    {
        timeSinceLastAction += Time.fixedDeltaTime;

        if (isRetreating)
        {
            Retreat();
        }
        else if (isMovingToTarget)
        {
            MoveToTarget();
        }
        else
        {
            OscillateInPlace();

            if (timeSinceLastAction >= chargeDelay)
            {
                Debug.Log("Switching to move-to-target state.");
                isMovingToTarget = true;
                timeSinceLastAction = 0f; // 타이머 초기화

                // Attack 애니메이션 재생
                animator.SetTrigger("Attack");
            }
        }
    }

    private void OscillateInPlace()
    {
        float oscillationX = Mathf.Sin(Time.time * 10.0f) * 0.3f;
        float oscillationY = Mathf.Cos(Time.time * 10.0f) * 0.3f;

        Vector2 oscillationOffset = new Vector2(oscillationX, oscillationY);

        transform.position += (Vector3)(oscillationOffset * Time.fixedDeltaTime);

        // Fly 애니메이션 재생
        animator.SetTrigger("Retreat");
        //animator.SetTrigger("Fly");
    }

    private void MoveToTarget()
    {
        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;

        transform.position += (Vector3)nextVec;

        spriteRenderer.flipX = dirVec.x < 0;

        if (Vector2.Distance(transform.position, target.position) <= 1.0f)
        {
            Debug.Log("Contact with target. Calculating retreat position...");
            retreatPosition = target.position + new Vector2(retreatDistanceX, retreatDistanceY);
            isMovingToTarget = false;
            isRetreating = true;

            // Retreat 애니메이션 재생
            animator.SetTrigger("Retreat");
        }
    }

    private void Retreat()
    {
        Vector2 dirVec = target.position - rigid.position;
        transform.position = Vector2.MoveTowards(transform.position, retreatPosition, retreatSpeed * Time.fixedDeltaTime);
        spriteRenderer.flipX = dirVec.x > 0;

        Debug.Log($"Retreating to {retreatPosition}, current position: {transform.position}");


        if (Vector2.Distance(transform.position, retreatPosition) <= 0.5f)
        {
            spriteRenderer.flipX = dirVec.x < 0;
            Debug.Log("Reached retreat position. Returning to oscillation mode.");
            isRetreating = false;
            timeSinceLastAction = 0f; // 타이머 초기화

            // Fly 애니메이션 재생
            animator.SetTrigger("Fly");
        }
    }
}
