using UnityEngine;

public class Mosquito : MonoBehaviour
{
    public float speed = 5f;
    public float chargeDelay = 3f;
    public float retreatDistanceX = 5f;
    public float retreatDistanceY = 5f;
    public float retreatSpeed = 3f;
    public Rigidbody2D target;

    private Rigidbody2D rigid;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private float timeSinceLastAction = 0f;
    private Vector2 retreatPosition;

    private const float TARGET_PROXIMITY_THRESHOLD = 1.0f;
    private const float RETREAT_PROXIMITY_THRESHOLD = 0.5f;

    private enum MosquitoState
    {
        Oscillating,
        MovingToTarget,
        Retreating
    }

    private MosquitoState currentState = MosquitoState.Oscillating;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
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

    private void OscillateInPlace()
    {
        float oscillationX = Mathf.Sin(Time.time * 10.0f) * 0.3f;
        float oscillationY = Mathf.Cos(Time.time * 10.0f) * 0.3f;
        Vector2 oscillationOffset = new Vector2(oscillationX, oscillationY);
        rigid.MovePosition(rigid.position + oscillationOffset * Time.fixedDeltaTime);
    }

    private void MoveToTarget()
    {
        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);

        spriteRenderer.flipX = dirVec.x < 0;

        if (Vector2.Distance(rigid.position, target.position) <= TARGET_PROXIMITY_THRESHOLD)
        {
            retreatPosition = target.position + new Vector2(retreatDistanceX * (dirVec.x < 0 ? -1 : 1), retreatDistanceY);
            SetState(MosquitoState.Retreating);
        }
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
                    break;
                case MosquitoState.Retreating:
                    animator.SetTrigger("Retreat");
                    break;
            }
        }
    }
}
