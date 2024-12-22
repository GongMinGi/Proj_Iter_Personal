using UnityEngine;

public class Mosquito : MonoBehaviour
{
    public float speed; //이동속도  
    public float chargeDelay; //딜레이
    public float retreatDistanceX; //물러나는 거리x
    public float retreatDistanceY; //물러나는 거리y
    public float retreatSpeed; //물러나는 속도
    public float activationDistance; //활성화 거리

    public Rigidbody2D target;

    private Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private float timeSinceLastAction = 0f;
    private Vector2 retreatPosition;
    private Vector2 targetPosition;
    private const float TARGET_PROXIMITY_THRESHOLD = 1.0f;
    private const float RETREAT_PROXIMITY_THRESHOLD = 0.5f;

    private enum MosquitoState
    {
        Oscillating,
        MovingToTarget,
        Retreating
    }

    private MosquitoState currentState = MosquitoState.Oscillating;
    private bool isActivated = false;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (!isActivated)
        {
            float distanceToTarget = Vector2.Distance(rigid.position, target.position);
            if (distanceToTarget <= activationDistance)
            {
                isActivated = true;
                targetPosition = target.position;
            }
            else
            {
                OscillateInPlace();
                return;
            }
        }

        timeSinceLastAction += Time.fixedDeltaTime;

        switch (currentState)
        {
            case MosquitoState.Oscillating:
                OscillateInPlace();
                if (timeSinceLastAction >= chargeDelay)
                {
                    SetState(MosquitoState.MovingToTarget);
                }
                break;
            case MosquitoState.MovingToTarget:
                MoveToTarget();
                break;
            case MosquitoState.Retreating:
                Retreat();
                break;
        }
    }

    private void MoveToTarget()
    {
        Vector2 dirVec = targetPosition - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);

        spriteRenderer.flipX = dirVec.x < 0;

        if (Vector2.Distance(rigid.position, targetPosition) <= TARGET_PROXIMITY_THRESHOLD)
        {
            retreatPosition = targetPosition + new Vector2(retreatDistanceX * (dirVec.x < 0 ? -1 : 1), retreatDistanceY);
            SetState(MosquitoState.Retreating);
        }
    }

    private void SetState(MosquitoState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            timeSinceLastAction = 0f;

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
            }
        }
    }

    private void OscillateInPlace()
    {
        float oscillationX = Mathf.Sin(Time.time * 10.0f) * 0.3f;
        float oscillationY = Mathf.Cos(Time.time * 10.0f) * 0.3f;
        Vector2 oscillationOffset = new Vector2(oscillationX, oscillationY);
        rigid.MovePosition(rigid.position + oscillationOffset * Time.fixedDeltaTime);
    }

    private void Retreat()
    {
        Vector2 dirVec = retreatPosition - rigid.position;
        rigid.MovePosition(Vector2.MoveTowards(rigid.position, retreatPosition, retreatSpeed * Time.fixedDeltaTime));
        spriteRenderer.flipX = dirVec.x < 0;

        if (Vector2.Distance(rigid.position, retreatPosition) <= RETREAT_PROXIMITY_THRESHOLD)
        {
            SetState(MosquitoState.Oscillating);
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        // 충돌한 대상이 플레이어일 경우
        if (other.CompareTag("Player"))
        {

            // 필요한 추가 동작이 있으면 여기에 추가
            // 예를 들어, 공격 애니메이션을 시작하거나, 데미지를 주는 등의 동작
            Debug.Log("Mosquito collided with Player!");
        }
    }

}
