using System.Collections;
using UnityEngine;

public class BaseMonster : MonoBehaviour
{
    [Header("Monster Settings")]
    [SerializeField] public int health = 5;  //체력
    [SerializeField] public float knockbackTime = 0.2f;     // 넉백 지속 시간
    [SerializeField] public float hitDirectionForce = 10f;  // 히트 방향 힘
    [SerializeField] public float constantForce = 5f;       // 상수 힘
    [SerializeField] public float knockbackDistance = 3f;    //넉백 거리
    [SerializeField] public float knockbackSpeed = 20f;  //넉백 속도
    public float proximityThreshold = 0.2f; //넉백 위치 근접 판정

    protected Rigidbody2D rigid;        // Rigidbody2D 컴포넌트
    protected Animator animator;        // Animator 컴포넌트

    private Coroutine KnockbackCoroutine;

    [SerializeField] private GameObject deathEffect;     // 죽음 파티클


    [SerializeField] protected Vector2 knockbackPosition;  // 넉백 목표 위치
    [SerializeField] protected bool isKnockback = false;
    private Vector2 attackSource;

    protected virtual void Awake()
    {
        Collider2D myCollider = GetComponent<Collider2D>();

        GameObject[] objectsWithTargetTag = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject obj in objectsWithTargetTag)
        {
            // 태그가 맞는 오브젝트의 Collider와 충돌 무시 설정
            Collider2D targetCollider = obj.GetComponent<Collider2D>();
            if (targetCollider != null && myCollider != null)
            {
                Physics2D.IgnoreCollision(myCollider, targetCollider);
            }
        }


        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    protected virtual void FixedUpdate()
    {
        if(isKnockback)
        {
            
            Knockback(attackSource);

        }
    }

    public virtual void TakeDamage(int damage, Vector2 damageSourcePosition)
    {
        attackSource = damageSourcePosition;
        //체력감소
        health -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Remaining health: {health}");
        AudioManager.instance.PlaySfx(AudioManager.Sfx.attacksuccess);

        //죽음 처리
        if (health <= 0 )
        {
            Die();
            return;
        }



        //// 넉백 방향 계산 ( x축만)
        //Vector2 knockbackDir = new Vector2(rigid.position.x - damageSourcePosition.x, 0).normalized;

        //Debug.Log(knockbackDir);

        ////넉백 목표 위치 계산
        //knockbackPosition = rigid.position + knockbackDir * knockbackDistance;
        //Debug.Log(knockbackPosition);
        ////isKnockback = true;

        //isKnockback = true;

        Vector2 hitDirection = new Vector2(rigid.position.x - damageSourcePosition.x, 0 ).normalized;
        Vector2 constantForceDirection = Vector2.zero; // 상수방향 힘은 기본적으로 0
        float inputDirection = 0;   //입력 방향은 없으므로 0으로 설정

        if ( KnockbackCoroutine != null )
        {
            StopCoroutine( KnockbackCoroutine );
        }

        KnockbackCoroutine = StartCoroutine(KnockbackAction( hitDirection, constantForceDirection, inputDirection));    


    }

    //protected virtual void Die()        // 원래 사망 코드
    //{
    //    Debug.Log($"{gameObject.name} died!");
    //    Destroy(gameObject);
    //}

    /*
     protected virtual void Die()
{
    Debug.Log($"{gameObject.name} died!");
    
    // 스프라이트 뒤집기와 넉백을 동시에 처리
    StartCoroutine(DieAnimation());
}

private IEnumerator DieAnimation()
{
    // 넉백 관련 변수
    Vector2 hitDirection = new Vector2(rigid.position.x - attackSource.x, 0).normalized; 
    Vector2 constantForceDirection = Vector2.zero; 
    float inputDirection = 0f; 

    // 회전 관련 변수
    float rotationSpeed = 200f; // 회전 속도
    float targetRotation = 180f; // 목표 회전 각도
    float currentRotation = 0f;

    float elapsedTime = 0f; // 넉백 경과 시간 초기화

    while (elapsedTime < knockbackTime)
    {
        elapsedTime += Time.fixedDeltaTime;

        // 1. 회전 처리 (Z축 기준)
        float rotationStep = rotationSpeed * Time.fixedDeltaTime;
        if (currentRotation < targetRotation)
        {
            currentRotation += rotationStep;
            transform.Rotate(0f, 0f, rotationStep); // Z축 기준으로 회전
        }

        // 2. 넉백 처리
        Vector2 hitForce = hitDirection * hitDirectionForce;
        Vector2 constForce = constantForceDirection * constantForce;
        Vector2 knockbackForce = hitForce + constForce;

        if (inputDirection != 0)
        {
            knockbackForce += new Vector2(inputDirection * hitDirectionForce, 0);
        }

        rigid.linearVelocity = knockbackForce;

        yield return new WaitForFixedUpdate(); // FixedUpdate와 동기화
    }

    // 넉백 종료 후 정지
    rigid.linearVelocity = Vector2.zero;

    // 3. 약간의 시간 대기
    yield return new WaitForSeconds(0.5f); // 0.5초 대기

    // 4. GameObject 파괴
    Destroy(gameObject);
}

     */     // 넉백 및 회전 후에 destroy시키는 사망모션 (방향 및 회전 설정 필요, z회전을 푸는 방식도 해봤는데 생각대로 안 움직임)

    
    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} died!");

        if (deathEffect != null)
        {
            // Instantiate the death effect at the current position and rotation
            GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.identity);

            // Get the ParticleSystem component and explicitly play it
            ParticleSystem particleSystem = effect.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Play(); // Explicitly start the particle system
            }

            // Destroy the effect object after the particle system duration
            Destroy(effect, particleSystem.main.duration);
        }

        Destroy(gameObject); // Destroy the current game object
    }

          // 죽으면 파티클 재생하는 사망모션 (이미지는 아무거나 넣음)


    protected virtual void Knockback(Vector2 damageSourcePosition)
    {
        if (!isKnockback) return;



        rigid.MovePosition(Vector2.MoveTowards(rigid.position, knockbackPosition, knockbackSpeed * Time.fixedDeltaTime));

        // 목표 위치에 도달하면 넉백 중지
        if (Vector2.Distance(rigid.position, knockbackPosition) <= proximityThreshold)
        {
            isKnockback = false;

        }

    }


    private IEnumerator KnockbackAction(Vector2 hitDirection, Vector2 constantForceDirection, float inputDirection)
    {
        float elapsedTime = 0f; // 넉백 경과 시간 초기화

        while( elapsedTime < knockbackTime) 
        {
            elapsedTime += Time.fixedDeltaTime; // 시간 경과 업데이트

            // 넉백 힘 계산
            Vector2 hitForce = hitDirection * hitDirectionForce;
            Vector2 constForce = constantForceDirection * constantForce;
            Vector2 knockbackForce = hitForce + constForce;

            Vector2 combinedForce = knockbackForce;

            if(inputDirection != 0 )
            {
                combinedForce += new Vector2(inputDirection * hitDirectionForce, 0);
            }

            rigid.linearVelocity = combinedForce;

            yield return new WaitForFixedUpdate(); // FixedUpdate와 동기화

        }
        rigid.linearVelocity = Vector2.zero;
    }




    // Update is called once per frame
    void Update()
    {
        
    }
}
